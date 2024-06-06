using FastAdminAPI.Business.Interfaces;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Converters;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.JsonTree;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Framework.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.Implements
{
    /// <summary>
    /// 数据权限
    /// </summary>
    internal class DataPermissionService : IDataPermissionService
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        protected readonly long _userId;
        /// <summary>
        /// 账号
        /// </summary>
        protected readonly string _account;
        /// <summary>
        /// 员工Id
        /// </summary>
        protected readonly long _employeeId;
        /// <summary>
        /// 员工名称
        /// </summary>
        protected readonly string _employeeName;
        /// <summary>
        /// 头像
        /// </summary>
        protected readonly string _avatar;

        /// <summary>
        /// 数据权限KEY
        /// </summary>
        private readonly string DATA_PERMISSION_KEY;

        /// <summary>
        /// Redis锁前缀
        /// </summary>
        private readonly string REDIS_LOCK_PREFIX = "Lock:DataPermit_";

        /// <summary>
        /// SugarScope
        /// </summary>
        protected SqlSugarScope _dbContext;
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="redis"></param>
        public DataPermissionService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext, IConfiguration configuration, IRedisHelper redis)
        {
            _dbContext = dbContext as SqlSugarScope;
            _redis = redis;

            _userId = Convert.ToInt64(httpContext.HttpContext.User.Claims.First(c => c.Type == "UserId").Value);
            _account = httpContext.HttpContext.User.Claims.First(c => c.Type == "Account").Value;
            _employeeId = Convert.ToInt64(httpContext.HttpContext.User.Claims.First(c => c.Type == "EmployeeId").Value);
            _employeeName = httpContext.HttpContext.User.Claims.First(c => c.Type == "EmployeeName").Value;
            _avatar = httpContext.HttpContext.User.Claims.First(c => c.Type == "Avatar").Value;

            DATA_PERMISSION_KEY = configuration.GetValue<string>("Redis.DataPermit.Key");

        }

        #region 内部方法
        /// <summary>
        /// Json树转列表
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        private static List<JsonTree> JsonTreeToList(List<JsonTree> dataSource)
        {
            List<JsonTree> list = new();
            if (dataSource?.Count > 0)
            {
                foreach (var item in dataSource)
                {
                    if (item.Children?.Count > 0)
                    {
                        list.AddRange(JsonTreeToList(item.Children));
                    }
                    item.Children = null;
                    list.Add(item);
                }
            }
            return list;
        }
        #endregion

        /// <summary>
        /// 获取数据权限
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<List<long>> Get()
        {
            if (_employeeId <= 0)
                throw new UserOperationException("无效的员工Id!");

            string hashKey = "EmployeeId_" + _employeeId.ToString();

            //从缓存中获取数据权限
            if (await _redis.HashExistAsync(DATA_PERMISSION_KEY, hashKey))
            {
                return await _redis.HashGetAsync<List<long>>(DATA_PERMISSION_KEY, hashKey);
            }

            //redis锁名
            string lockName = REDIS_LOCK_PREFIX + hashKey;

            //redis锁令牌
            string token = GuidConverter.GenerateShortGuid();

            //如果获取不到锁，就返回失败
            if (!await _redis.GetLockAsync(lockName, token))
            {
                throw new UserOperationException("请求次数过多，请稍后再试!");
            }

            try
            {
                //员工可见岗位权限
                List<long> postIds = new();

                //获取全部部门
                var departs = await _dbContext.Queryable<S05_Department>()
                    .Where(S05 => S05.S05_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                    .Select(S05 => new JsonTree
                    {
                        Id = S05.S05_DepartId,
                        Name = S05.S05_DepartName,
                        ParentId = S05.S05_ParentDepartId
                    })
                    .ToListAsync();

                //获取全部岗位
                var posts = await _dbContext.Queryable<S06_Post>()
                    .Where(S06 => S06.S06_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                    .Select(S06 => new JsonTree
                    {
                        Id = S06.S06_PostId,
                        Name = S06.S06_PostName,
                        ParentId = S06.S06_ParentPostId,
                        Data = S06.S05_DepartId.ToString()
                    })
                    .ToListAsync();

                //获取员工的岗位信息
                var employeePosts = await _dbContext.Queryable<S08_EmployeePost>()
                    .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S08.S07_EmployeeId == _employeeId)
                    .Select(S08 => new { S08.S06_PostId, S08.S05_DepartId }).ToListAsync();

                if (!employeePosts?.Any() ?? true)
                {
                    throw new UserOperationException("获取员工信息失败!");
                }

                //对员工部门进行循环查找下级部门
                foreach (var item in employeePosts.Select(c => c.S05_DepartId).Distinct())
                {
                    //创建部门树
                    var departTree = JsonTree.CreateCustomTrees(departs.Where(d => d.Id == item).ToList(), departs.Where(p => p.ParentId != null).ToList());
                    //将树转为列表 
                    var departIds = JsonTreeToList(departTree).Select(d => d.Id).ToList();
                    //去除当前员工的部门，防止同级员工数据交错
                    departIds.Remove(item);
                    //添加部门下的岗位
                    postIds.AddRange(posts.Where(p => departIds.Contains(Convert.ToInt64(p.Data))).Select(p => p.Id).ToList());
                }

                //对员工岗位进行循环查找下级岗位
                foreach (var item in employeePosts.Select(c => c.S06_PostId).Distinct())
                {
                    //创建岗位树，根为当前员工的岗位
                    var postTree = JsonTree.CreateCustomTrees(posts.Where(p => p.Id == item).ToList(), posts.Where(p => p.ParentId != null).ToList());
                    //将json树转为列表 
                    postIds.AddRange(JsonTreeToList(postTree).Select(p => p.Id).ToList());
                    //去除当前用户的岗位，防止同级用户数据交错
                    postIds.Remove(item);
                }

                //获取数据权限设置
                var dataPermissionSettings = await _dbContext.Queryable<S10_DataPermission>()
                    .Where(S10 => S10.S07_EmployeeId == _employeeId)
                    .Select(S10 => S10.S05_DepartIds).FirstAsync();

                //如果设置了数据权限，就获取设置的值
                if (dataPermissionSettings != null && !string.IsNullOrEmpty(dataPermissionSettings))
                {
                    //将数据权限设置值转为部门Ids
                    var ids = dataPermissionSettings.Split(",").Select(c => Convert.ToInt64(c)).Distinct().ToList();
                    if (ids?.Count > 0)
                    {
                        //数据权限设置的部门
                        List<long> employeeDepartIds = departs.Where(c => ids.Contains(c.Id)).Select(c => c.Id).ToList();

                        //创建部门树，根为所设置部门
                        var departTree = JsonTree.CreateCustomTrees(departs.Where(d => employeeDepartIds.Contains(d.Id)).ToList(), departs.Where(d => d.ParentId != null).ToList());

                        //将部门树进行循环，寻找岗位
                        foreach (var item in JsonTreeToList(departTree)) //将json树转为列表
                        {
                            //将部门下的岗位放入集合中
                            postIds.AddRange(posts.Where(p => Convert.ToInt64(p.Data) == item.Id).Select(p => p.Id).ToList());
                        }
                    }
                }

                //添加自身
                List<long> dataPermit = new() { _employeeId };

                //判断岗位是否存在
                if (postIds?.Count > 0)
                {
                    //获取所有岗位下的员工Id
                    var dataPermission = await _dbContext.Queryable<S08_EmployeePost>()
                        .InnerJoin<S07_Employee>((S08, S07) => S08.S07_EmployeeId == S07.S07_EmployeeId)
                        .Where((S08, S07) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False && 
                                             postIds.Contains(S08.S06_PostId) && 
                                             S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                        .Select((S08, S07) => S08.S07_EmployeeId).ToListAsync();

                    //去重
                    dataPermit.AddRange(dataPermission.Distinct().ToList());
                }

                //将数据权限保存在Redis缓存中
                bool isSuccess = await _redis.HashSetAsync(DATA_PERMISSION_KEY, hashKey, dataPermit);
                if (isSuccess)
                {
                    return await _redis.HashGetAsync<List<long>>(DATA_PERMISSION_KEY, hashKey);
                }
                else
                {
                    throw new UserOperationException("数据权限缓存出错，请稍后再试!");
                }
            }
            catch (UserOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"获取员工【{_employeeId}-{_employeeName}】数据权限失败! {ex.Message}", ex);
                throw new UserOperationException($"获取数据权限失败，请稍后再试!");
            }
            finally
            {
                //释放Redis锁
                await _redis.ReleaseLockAsync(lockName, token);
            }
        }

        /// <summary>
        /// 释放数据权限
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Release()
        {
            return await _redis.KeyDeleteAsync(DATA_PERMISSION_KEY);
        }

        /// <summary>
        /// 释放数据权限
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<bool> Release(long employeeId)
        {
            if (employeeId <= 0)
                throw new UserOperationException("无效的用户Id!");

            string hashKey = "EmployeeId_" + employeeId.ToString();

            //判断是否存在该缓存
            if (await _redis.HashExistAsync(DATA_PERMISSION_KEY, hashKey))
                return await _redis.HashDeleteAsync(DATA_PERMISSION_KEY, hashKey);
            else
                return true;
        }
    }
}
