using FastAdminAPI.Business.Interfaces;
using FastAdminAPI.Business.PrivateFunc.Applications.Models;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Converters;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.JsonTree;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Modules;
using FastAdminAPI.Core.Models.Users;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.Attributes.Extension.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        /// <summary>
        /// 审批处理接口
        /// </summary>
        private readonly IApplicationHandler _applicationHandler;
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;
        /// <summary>
        /// Redis锁前缀
        /// </summary>
        private readonly string REDIS_LOCK_PREFIX = "lock:Application:Process_";

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        /// <param name="applicationHandler"></param>
        /// <param name="redis"></param>
        public UserService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext,
            IApplicationHandler applicationHandler, IRedisHelper redis) : base(dbContext, httpContext)
        {
            _applicationHandler = applicationHandler;
            _redis = redis;
        }

        #region 权限
        /// <summary>
        /// 获取模块功能
        /// 【角色权限+用户权限】
        /// </summary>
        /// <returns></returns>
        private async Task<List<long>> GetUserModuleIds()
        {
            List<long> modules = new();

            //用户角色Ids
            var roleIds = await _dbContext.Queryable<S09_UserPermission>()
                .Where(S09 => S09.S01_UserId == _userId &&
                              S09.S09_PermissionType == (byte)BusinessEnums.PermissionType.Role)
                .Select(S09 => S09.S09_CommonId)
                .ToListAsync();
            if (roleIds?.Count > 0)
            {
                //获取用户角色权限
                var moduleIdsByRole = await _dbContext.Queryable<S04_RolePermission>()
                .Where(S04 => roleIds.Contains(S04.S03_RoleId))
                .Select(S04 => S04.S02_ModuleId)
                .ToListAsync();
                if (moduleIdsByRole?.Count > 0)
                {
                    modules.AddRange(moduleIdsByRole);
                }
            }

            //用户权限
            var moduleIdsByUser = await _dbContext.Queryable<S09_UserPermission>()
                .Where(S09 => S09.S01_UserId == _userId &&
                              S09.S09_PermissionType == (byte)BusinessEnums.PermissionType.User)
                .Select(S09 => S09.S09_CommonId)
                .ToListAsync();
            if (moduleIdsByUser?.Count > 0)
            {
                modules.AddRange(moduleIdsByUser);
            }

            //去重
            modules = modules.Distinct().ToList();
            return modules;
        }
        /// <summary>
        /// 获取功能权限(当前登录用户所有权限)
        /// 【角色权限+用户权限】
        /// </summary>
        /// <returns></returns>
        public async Task<List<ModuleInfoModel>> GetPermissions()
        {
            //获取当前登录者权限查询
            List<long> modules = await GetUserModuleIds();

            //获取权限信息
            if (modules?.Count > 0)
            {
                return await _dbContext.Queryable<S02_Module>()
                .Where(S02 => S02.S02_IsDelete == (byte)BaseEnums.TrueOrFalse.False && modules.Contains(S02.S02_ModuleId))
                .Select(S02 => new ModuleInfoModel
                {
                    Id = S02.S02_ModuleId,
                    Name = S02.S02_ModuleName,
                    ParentId = S02.S02_ParentModuleId,
                    Priority = S02.S02_Priority ?? 0,
                    Kind = S02.S02_Kind,
                    Depth = S02.S02_Depth,
                    FrontRoute = S02.S02_FrontRoute,
                    Logo = S02.S02_Logo,
                    BackInterface = S02.S02_BackInterface,
                    CornerMark = S02.S02_CornerMark
                }).ToListAsync();
            }
            return null;
        }
        /// <summary>
        /// 获取菜单权限树
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetMenuPermissionsTree()
        {
            //获取当前登录者权限查询
            List<long> modules = await GetUserModuleIds();

            //获取菜单信息
            if (modules?.Count > 0)
            {
                var result = await _dbContext.Queryable<S02_Module>()
                .Where(S02 => S02.S02_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              modules.Contains(S02.S02_ModuleId) &&
                              S02.S02_Kind == (byte)BusinessEnums.ModuleKind.Menu)
                .Select(S02 => new ModuleInfoModel
                {
                    Id = S02.S02_ModuleId,
                    Name = S02.S02_ModuleName,
                    ParentId = S02.S02_ParentModuleId,
                    Priority = S02.S02_Priority ?? 0,
                    Kind = S02.S02_Kind,
                    Depth = S02.S02_Depth,
                    FrontRoute = S02.S02_FrontRoute,
                    Logo = S02.S02_Logo,
                    BackInterface = S02.S02_BackInterface,
                    CornerMark = S02.S02_CornerMark
                }).ToListAsync();

                return SortedJsonTree.CreateJsonTrees(result);
            }
            return string.Empty;
        }
        #endregion

        #region 通用审批
        /// <summary>
        /// 获取我的审批列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCheckList(CheckPageSearch pageSearch)
        {
            return await _dbContext.Queryable<S12_Check>()
                .Where(S12 => S12.S12_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S12.S12_IsFinishCheck == (byte)BaseEnums.IsFinish.Unfinish &&
                              S12.S07_ApproverId == _employeeId)
                .ToListResultAsync(pageSearch, new CheckPageResult());
        }
        /// <summary>
        /// 审批申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> ProcessApplication(ApprovalModel model)
        {
            if (model?.CheckIds?.Count > 0)
            {
                var checks = await _dbContext.Queryable<S12_Check>()
                    .Where(S12 => S12.S12_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                  S12.S12_IsFinishCheck == (byte)BaseEnums.IsFinish.Unfinish)
                    .ToListAsync();
                if (checks?.Count > 0)
                {
                    int successCount = 0;
                    int failCount = 0;
                    StringBuilder failInfo = new();
                    foreach (var checkId in model.CheckIds)
                    {
                        #region 数据获取及校验
                        //获取审批信息
                        var check = checks.Where(c => c.S12_CheckId == checkId).FirstOrDefault();
                        if (check == null)
                        {
                            failCount++;
                            failInfo.Append($"获取[{checkId}]审批信息失败，当前审批不存在或已完成审批!");
                            continue;
                        }
                        #endregion

                        #region Redis锁
                        //redis锁名
                        string lockName = REDIS_LOCK_PREFIX + check.S12_CheckId;

                        //redis锁令牌
                        string token = GuidConverter.GenerateShortGuid();

                        //如果获取不到锁，就返回失败
                        if (!await _redis.GetLockAsync(lockName, token))
                        {
                            failCount++;
                            failInfo.Append($"审批[{checkId}]请求频繁，请稍后再试!");
                            continue;
                        }
                        #endregion

                        try
                        {
                            #region 数据库操作
                            //开启事务进行审批
                            var result = await _dbContext.TransactionAsync(async () =>
                            {
                                return await _applicationHandler.Approve(new ProcessingApplicationModel
                                {
                                    Check = check,
                                    IsApprove = model.IsApprove,
                                    Reason = model.Reason,
                                    OperationId = _employeeId,
                                    OperationName = _employeeName
                                });
                            });

                            //判断数据库操作是否成功
                            if (result?.Code == ResponseCode.Success)
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                                failInfo.Append($"[{checkId}]审批失败，{result?.Message}");
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            throw new UserOperationException(ex.Message);
                        }
                        finally
                        {
                            //释放Redis锁
                            await _redis.ReleaseLockAsync(lockName, token);
                        }
                    }

                    //返回结果
                    ResponseModel response = ResponseModel.Success();
                    response.Message = $"操作成功!成功{successCount}条，失败{failCount}条!失败原因:{failInfo}";
                    return response;
                }
                else
                    throw new UserOperationException("获取审批信息失败!");
            }
            else
                throw new UserOperationException("请选择要审批的数据!");
        }
        /// <summary>
        /// 通用撤销申请
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> CancelApplication(long checkId)
        {
            var isExist = await _dbContext.Queryable<S12_Check>()
                .Where(S12 => S12.S12_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S12.S12_IsFinishCheck == (byte)BaseEnums.IsFinish.Unfinish && 
                              S12.S12_CheckId == checkId)
                .AnyAsync();
            if (!isExist)
                throw new UserOperationException("找不到该申请或该申请已完成审批!");

            return await _dbContext.Updateable<S12_Check>()
                .SetColumns(S12 => S12.S12_IsDelete == (byte)BaseEnums.TrueOrFalse.True)
                .SetColumns(S12 => S12.S12_DeleteId == _employeeId)
                .SetColumns(S12 => S12.S12_DeleteBy == _employeeName + " [撤销操作]")
                .SetColumns(S12 => S12.S12_DeleteTime == SqlFunc.GetDate())
                .Where(S12 => S12.S12_CheckId == checkId)
                .ExecuteAsync();
        }
        /// <summary>
        /// 获取我的审批记录列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCheckRecordList(CheckRecordPageSearch pageSearch)
        {
            return await _dbContext.Queryable<S13_CheckRecords>()
                .InnerJoin<S12_Check>((S13, S12) => S13.S12_CheckId == S12.S12_CheckId)
                .Where((S13, S12) => S12.S12_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                     S13.S07_ApproverId == _employeeId)
                .ToListResultAsync(pageSearch, new CheckRecordPageResult());
        }
        /// <summary>
        /// 获取申请审批记录列表
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <returns></returns>
        public async Task<List<CheckRecordModel>> GetApplicationCheckRecords(long checkId)
        {
            try
            {
                var check = await _dbContext.Queryable<S12_Check>()
                    .Where(S12 => S12.S12_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                  S12.S12_CheckId == checkId)
                    .FirstAsync() ?? throw new UserOperationException("找不到该申请审批记录!");

                var approvers = JsonConvert.DeserializeObject<List<ApproverInfoModel>>(check.S12_ApproversData);

                List<CheckRecordModel> list = new();
                if (approvers?.Count > 0)
                {
                    var records = await _dbContext.Queryable<S13_CheckRecords>()
                        .Where(S13 => S13.S12_CheckId == checkId)
                        .OrderBy(S13 => S13.S13_CheckRecordId)
                        .Select(new CheckRecordModel())
                        .ToListAsync();
                    approvers.OrderBy(c => c.Priority).ToList().ForEach(item =>
                    {
                        var record = records.Where(c => c.ApproverId == item.EmployeeId).FirstOrDefault();
                        if (record != null)
                        {
                            list.Add(record);
                        }
                        else
                        {
                            list.Add(new CheckRecordModel()
                            {
                                CheckId = checkId,
                                ApproverId = item.EmployeeId,
                                ApproverName = item.EmployeeName,
                                IsApprove = -1 //待审批
                            });
                        }
                    });
                    return list;
                }
                else
                    throw new UserOperationException("找不到审批人!");
            }
            catch (Exception)
            {
                throw new UserOperationException("该申请有误!");
            }
        }
        #endregion
    }
}
