using FastAdminAPI.Business.Statics;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.JsonTree;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.RolePermission;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using SqlSugar.Attributes.Extension.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public class RolePermissionService : BaseService, IRolePermissionService
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        public RolePermissionService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext) : base(dbContext, httpContext){ }

        #region 角色
        /// <summary>
        /// 获取角色树
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        public async Task<string> GetRoleTree(string roleName)
        {
            //用户角色Ids
            var roleIds = await _dbContext.Queryable<S09_UserPermission>()
                .Where(S09 => S09.S09_PermissionType == (byte)BusinessEnums.PermissionType.Role && 
                              S09.S01_UserId == _userId)
                .Select(S09 => S09.S09_CommonId)
                .ToListAsync();
            if(roleIds?.Count > 0)
            {
                List<long> parentRoles = new();

                //全部角色
                var roles = await _dbContext.Queryable<S03_Role>()
                    .Where(S03 => S03.S03_IsValid == (byte)BaseEnums.IsValid.Valid)
                    .Select(S03 => new RoleInfoModel
                    {
                        Id = S03.S03_RoleId,
                        Name = S03.S03_RoleName,
                        ParentId = S03.S03_ParentRoleId
                    }).ToListAsync();

                //获取用户角色
                var rolesByUser = roles.Where(c => roleIds.Contains(c.Id)).ToList();

                //角色为超级管理员
                if(rolesByUser.Where(c => c.ParentId == null).Any()) 
                {
                    return JsonTree.CreateJsonTrees(roles, roleName);
                }
                //其他角色
                else
                {
                    return JsonTree.CreateCustomJsonTrees(rolesByUser, roles, roleName);
                }
            }
            return null;
        }
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddRole(AddRoleModel model)
        {
            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();
            model.CornerMark = await CornerMarkGenerator.GetCornerMark(_dbContext, "S03_Role", "S03_RoleId",
                "S03_CornerMark", "S03_ParentRoleId", model.ParentRoleId.ToString());
            return await _dbContext.InsertResultAsync<AddRoleModel, S03_Role>(model);
        }
        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditRole(EditRoleModel model)
        {
            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();
            return await _dbContext.UpdateResultAsync<EditRoleModel, S03_Role>(model);
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> DelRole(long roleId)
        {
            bool isExistChild = await _dbContext.Queryable<S03_Role>()
                .Where(S03 => S03.S03_IsValid == (byte)BaseEnums.IsValid.Valid && S03.S03_ParentRoleId == roleId)
                .AnyAsync();
            if (isExistChild)
                throw new UserOperationException("该角色下有子角色，无法删除!");

            return await _dbContext.Deleteable<S03_Role>()
                .Where(S03 => S03.S03_RoleId == roleId)
                .SoftDeleteAsync(S03 => new S03_Role
                {
                    S03_IsValid = (byte)BaseEnums.IsValid.InValid,
                    S03_DeleteId = _employeeId,
                    S03_DeleteBy = _employeeName,
                    S03_DeleteTime = SqlFunc.GetDate()
                });
        }
        /// <summary>
        /// 复制角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> CopyRole(CopyRoleModel model)
        {
            //校验源角色是否可用
            bool isValidRole = await _dbContext.Queryable<S03_Role>()
                .Where(S03 => S03.S03_IsValid == (byte)BaseEnums.IsValid.Valid && S03.S03_RoleId == model.SourceRoleId)
                .AnyAsync();
            if (!isValidRole)
                throw new UserOperationException("找不到源角色，请重试!");
            
            //获取源角色的权限
            var rolePermissions = await _dbContext.Queryable<S04_RolePermission>()
                .Where(S04 => S04.S03_RoleId == model.SourceRoleId)
                .Select(S04 => S04.S02_ModuleId)
                .ToListAsync();
            if(rolePermissions?.Count > 0)
            {
                return await _dbContext.TransactionAsync(async () => 
                {
                    //创建新的角色
                    S03_Role role = new()
                    {
                        S03_RoleName = model.RoleName,
                        S03_ParentRoleId = model.ParentRoleId,
                        S03_CornerMark = await CornerMarkGenerator.GetCornerMark(_dbContext, "S03_Role", "S03_RoleId",
                            "S03_CornerMark", "S03_ParentRoleId", model.ParentRoleId.ToString()),
                        S03_IsValid = (byte)BaseEnums.IsValid.Valid,
                        S03_CreateId = _employeeId,
                        S03_CreateBy = _employeeName,
                        S03_CreateTime = _dbContext.GetDate()
                    };
                    var result = await _dbContext.Insertable(role).ExecuteAsync();

                    if(result?.Code == ResponseCode.Success)
                    {
                        long roleId = Convert.ToInt64(result.Data);

                        //批量插入时获取统一时间
                        DateTime dbTime = _dbContext.GetDate();

                        //批量插入权限
                        List<S04_RolePermission> permissions = new();
                        rolePermissions.ForEach(item =>
                        {
                            permissions.Add(new S04_RolePermission
                            {
                                S03_RoleId = roleId,
                                S02_ModuleId = item,
                                S04_CreateId = _employeeId,
                                S04_CreateBy = _employeeName,
                                S04_CreateTime = dbTime
                            });
                        });

                        result = await _dbContext.Insertable(permissions).ExecuteAsync();
                    }

                    return result;

                });
            }
            throw new UserOperationException("源角色未配置权限，请先配置源角色的权限!");
        }
        #endregion

        #region 权限
        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public async Task<List<long>> GetRolePermission(long roleId)
        {
            return await _dbContext.Queryable<S04_RolePermission>()
                .Where(S04 => S04.S03_RoleId == roleId)
                .Select(S04 => S04.S02_ModuleId)
                .ToListAsync();
        }
        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> SaveRolePermission(SaveRolePermissionModel model)
        {
            if (model.ModuleIds?.Count > 0)
            {
                return await _dbContext.TransactionAsync(async () => 
                {
                    //先删除旧的角色权限
                    var result = await _dbContext.Deleteable<S04_RolePermission>()
                    .Where(S04 => S04.S03_RoleId == model.RoleId)
                    .ExecuteAsync();

                    if(result?.Code == ResponseCode.Success)
                    {
                        //新增新的角色权限
                        List<S04_RolePermission> permissions = new();
                        foreach (var item in model.ModuleIds)
                        {
                            //如果模块Id小于等于0，就跳过，因为不存在这种模块
                            if (item <= 0)
                                continue;

                            //模块
                            S04_RolePermission permission = new()
                            {
                                S03_RoleId = (long)model.RoleId,
                                S02_ModuleId = item,
                                S04_CreateId = _employeeId,
                                S04_CreateBy = _employeeName,
                                S04_CreateTime = _dbContext.GetDate()
                            };
                            permissions.Add(permission);
                        }
                        result = await _dbContext.Insertable(permissions).ExecuteAsync();
                    }

                    return result;
                });
            }
            else
                throw new UserOperationException("请选择需要设置的权限!");
        }
        #endregion
    }
}
