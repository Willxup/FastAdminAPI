using FastAdminAPI.Business.Statics;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.JsonTree;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Modules;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 模块
    /// </summary>
    public class ModuleService : BaseService, IModuleService
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        public ModuleService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext) : base(dbContext, httpContext) { }

        /// <summary>
        /// 获取模块树
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public async Task<string> GetModuleTree(string moduleName = null)
        {
            return SortedJsonTree.CreateJsonTrees(await _dbContext.Queryable<S02_Module>()
                .Where(S02 => S02.S02_IsValid == (byte)BaseEnums.IsValid.Valid)
                .Select(S02 => new ModuleInfoModel
                {
                    Id = S02.S02_ModuleId,
                    Name = S02.S02_ModuleName,
                    ParentId = S02.S02_ParentModuleId,
                    Priority = S02.S02_Priority ?? 0,
                    //Priority = (int)SqlFunc.IsNull(S02.S02_Priority, 0),
                    Kind = S02.S02_Kind,
                    Depth = S02.S02_Depth,
                    FrontRoute = S02.S02_FrontRoute,
                    Logo = S02.S02_Logo,
                    BackInterface = S02.S02_BackInterface,
                    CornerMark = S02.S02_CornerMark
                }).ToListAsync(), moduleName);
        }
        /// <summary>
        /// 新增模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddModule(AddModuleModel model)
        {
            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();
            model.CornerMark = await CornerMarkGenerator.GetCornerMark(_dbContext, "S02_Module", "S02_ModuleId",
                "S02_CornerMark", "S02_ParentModuleId", model.ParentModuleId.ToString());
            return await _dbContext.InsertResultAsync<AddModuleModel, S02_Module>(model);
        }
        /// <summary>
        /// 编辑模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditModule(EditModuleModel model)
        {
            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();
            return await _dbContext.UpdateResultAsync<EditModuleModel, S02_Module>(model);
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> DelModule(long moduleId)
        {
            //校验
            bool isExistChild = await _dbContext.Queryable<S02_Module>()
                .Where(S02 => S02.S02_IsValid == (byte)BaseEnums.IsValid.Valid && S02.S02_ParentModuleId == moduleId)
                .AnyAsync();
            if (isExistChild)
                throw new UserOperationException("当前模块下存在子模块，请删除子模块后再进行删除!");

            return await _dbContext.Deleteable<S02_Module>()
                .Where(S02 => S02.S02_ModuleId == moduleId)
                .SoftDeleteAsync(S02 => new S02_Module
                {
                    S02_IsValid = (byte)BaseEnums.IsValid.InValid,
                    S02_DeleteId = _employeeId,
                    S02_DeleteBy = _employeeName,
                    S02_DeleteTime = SqlFunc.GetDate()
                });
        }
        /// <summary>
        /// 按模块Id获取员工列表
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public async Task<List<long>> GetEmployeeListByModuleId(long moduleId)
        {
            //用户Ids
            List<long> userIds = new();

            //获取拥有当前模块的角色
            var roleIds = await _dbContext.Queryable<S04_RolePermission>()
                .Where(S04 => S04.S02_ModuleId == moduleId)
                .Select(S04 => S04.S03_RoleId).ToListAsync();
            //通过角色获取拥有这些角色的用户
            if (roleIds?.Count > 0)
            {
                var userIdsByRole = await _dbContext.Queryable<S09_UserPermission>()
                    .Where(S09 => S09.S09_PermissionType == (byte)BusinessEnums.PermissionType.Role && roleIds.Contains(S09.S09_CommonId))
                    .Select(S09 => S09.S01_UserId).ToListAsync();
                if (userIdsByRole?.Count > 0)
                {
                    userIds.AddRange(userIdsByRole);
                }
            }

            //获取拥有当前模块的用户
            var userIdsByUser = await _dbContext.Queryable<S09_UserPermission>()
                .Where(S09 => S09.S09_PermissionType == (byte)BusinessEnums.PermissionType.User && S09.S09_CommonId == moduleId)
                .Select(S09 => S09.S01_UserId).ToListAsync();
            if (userIdsByUser?.Count > 0)
            {
                userIds.AddRange(userIdsByUser);
            }

            return userIds;
        }
    }
}
