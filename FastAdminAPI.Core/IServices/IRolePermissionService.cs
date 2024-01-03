using FastAdminAPI.Core.Models.Modules;
using FastAdminAPI.Core.Models.RolePermission;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public interface IRolePermissionService
    {
        #region 角色
        /// <summary>
        /// 获取角色树
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        Task<string> GetRoleTree(string roleName);
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddRole(AddRoleModel model);
        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> EditRole(EditRoleModel model);
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> DelRole(DelRoleModel model);
        /// <summary>
        /// 复制角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        Task<ResponseModel> CopyRole(CopyRoleModel model);
        #endregion

        #region 权限
        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        Task<List<long>> GetRolePermission(long roleId);
        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> SaveRolePermission(SaveRolePermissionModel model);
        #endregion
    }
}
