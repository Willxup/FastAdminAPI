﻿using System.Linq;
using System.Threading.Tasks;
using FastAdminAPI.Business.Interfaces;
using FastAdminAPI.Business.Utilities;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Tree;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Posts;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 岗位设置
    /// </summary>
    public class PostService : BaseService, IPostService
    {
        /// <summary>
        /// 数据权限Service
        /// </summary>
        private readonly IDataPermissionService _dataPermissionService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        /// <param name="dataPermissionService"></param>
        public PostService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext,
            IDataPermissionService dataPermissionService) : base(dbContext, httpContext)
        {
            _dataPermissionService = dataPermissionService;
        }

        /// <summary>
        /// 获取岗位树
        /// </summary>
        /// <param name="departId">部门Id</param>
        /// <returns></returns>
        public async Task<string> GetPostTree(long departId)
        {
            var postList = await _dbContext.Queryable<S06_Post>()
                .Where(S06 => S06.S06_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S06.S05_DepartId == departId)
                .Select(S06 => new PostInfoModel
                {
                    Id = S06.S06_PostId,
                    Name = S06.S06_PostName,
                    ParentId = S06.S06_ParentPostId,

                    DepartId = S06.S05_DepartId,
                    CornerMark = S06.S06_CornerMark,
                    CurrentEmployeeNums = SqlFunc.Subqueryable<S08_EmployeePost>()
                        .InnerJoin<S07_Employee>((S08, S07) => S08.S07_EmployeeId == S07.S07_EmployeeId)
                        .Where((S08, S07) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                             S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                             S08.S06_PostId == S06.S06_PostId)
                        .Count(),
                    MaxEmployeeNums = S06.S06_MaxEmployeeNums,
                    Responsibility = S06.S06_Responsibility,
                    AbilityDemand = S06.S06_AbilityDemand
                }).ToListAsync();

            return BaseTree<PostInfoModel>.BuildJsonTree(postList);
            
        }
        /// <summary>
        /// 获取多个岗位树
        /// </summary>
        /// <param name="departIds">部门Ids</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<string> GetMultiPostTree(long[] departIds)
        {
            if (departIds?.Length > 0)
            {
                var postList = await _dbContext.Queryable<S06_Post>()
                    .Where(S06 => S06.S06_IsDelete == (byte)BaseEnums.TrueOrFalse.False && departIds.Contains(S06.S05_DepartId))
                    .Select(S06 => new PostInfoModel
                    {
                        Id = S06.S06_PostId,
                        Name = S06.S06_PostName,
                        ParentId = S06.S06_ParentPostId,

                        DepartId = S06.S05_DepartId,
                        CornerMark = S06.S06_CornerMark,
                        CurrentEmployeeNums = SqlFunc.Subqueryable<S08_EmployeePost>()
                            .InnerJoin<S07_Employee>((S08, S07) => S08.S07_EmployeeId == S07.S07_EmployeeId)
                            .Where((S08, S07) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                 S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                 S08.S06_PostId == S06.S06_PostId)
                            .Count(),
                        MaxEmployeeNums = S06.S06_MaxEmployeeNums,
                        Responsibility = S06.S06_Responsibility,
                        AbilityDemand = S06.S06_AbilityDemand
                    }).ToListAsync();
                
                return BaseTree<PostInfoModel>.BuildJsonTree(postList);
            }
            else
                throw new UserOperationException("请选择部门!");
            
        }
        /// <summary>
        /// 获取岗位信息
        /// </summary>
        /// <param name="postId">岗位Id</param>
        /// <returns></returns>
        public async Task<PostInfoModel> GetPostById(long postId)
        {
            return await _dbContext.Queryable<S06_Post>()
                .Where(S06 => S06.S06_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S06.S06_PostId == postId)
                .Select(S06 => new PostInfoModel
                {
                    Id = S06.S06_PostId,
                    Name = S06.S06_PostName,
                    ParentId = S06.S06_ParentPostId,

                    DepartId = S06.S05_DepartId,
                    CornerMark = S06.S06_CornerMark,
                    CurrentEmployeeNums = SqlFunc.Subqueryable<S08_EmployeePost>()
                                                     .InnerJoin<S07_Employee>((S08, S07) => S08.S07_EmployeeId == S07.S07_EmployeeId)
                                                     .Where((S08, S07) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                                          S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                                          S08.S06_PostId == S06.S06_PostId)
                                                     .Count(),
                    MaxEmployeeNums = S06.S06_MaxEmployeeNums,
                    Responsibility = S06.S06_Responsibility,
                    AbilityDemand = S06.S06_AbilityDemand
                }).FirstAsync();
        }
        /// <summary>
        /// 新增岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddPost(AddPostModel model)
        {
            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            model.CornerMark = await CornerMarkGenerator.GetCornerMark(_dbContext, "S06_Post", "S06_PostId",
                "S06_CornerMark", "S06_ParentPostId", model.ParentPostId.ToString());

            var result = await _dbContext.InsertResultAsync<AddPostModel, S06_Post>(model);

            if (result?.Code == ResponseCode.Success)
            {
                //释放数据权限
                await _dataPermissionService.Release();
            }

            return result;
        }
        /// <summary>
        /// 编辑岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditPost(EditPostModel model)
        {
            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            var result = await _dbContext.UpdateResultAsync<EditPostModel, S06_Post>(model);

            if (result?.Code == ResponseCode.Success)
            {
                //释放数据权限
                await _dataPermissionService.Release();
            }

            return result;
        }
        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="postId">岗位Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> DelPost(long postId)
        {
            //校验子岗位
            bool isExistChild = await _dbContext.Queryable<S06_Post>()
                .Where(S06 => S06.S06_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S06.S06_ParentPostId == postId)
                .AnyAsync();
            if (isExistChild)
                throw new UserOperationException("该岗位有子岗位存在，无法删除!");

            //校验员工岗位
            bool isExistEmployeePost = await _dbContext.Queryable<S08_EmployeePost>()
                .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S08.S06_PostId == postId)
                .AnyAsync();
            if (isExistEmployeePost)
                throw new UserOperationException("该岗位已存在员工，请移除员工后删除!");

            var result = await _dbContext.Deleteable<S06_Post>()
                .Where(S06 => S06.S06_PostId == postId)
                .SoftDeleteAsync(S06 => new S06_Post
                {
                    S06_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                    S06_DeleteId = _employeeId,
                    S06_DeleteBy = _employeeName,
                    S06_DeleteTime = SqlFunc.GetDate()
                });

            if (result?.Code == ResponseCode.Success)
            {
                //释放数据权限
                await _dataPermissionService.Release();
            }

            return result;
        }
    }
}
