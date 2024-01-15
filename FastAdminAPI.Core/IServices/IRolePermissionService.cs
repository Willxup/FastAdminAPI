using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Models.RolePermission;
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
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        Task<ResponseModel> DelRole(long roleId);
        /// <summary>
        /// 复制角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
