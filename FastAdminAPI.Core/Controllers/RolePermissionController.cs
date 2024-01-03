using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Modules;
using FastAdminAPI.Core.Models.RolePermission;
using FastAdminAPI.Common.BASE;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public class RolePermissionController : BaseController
    {
        /// <summary>
        /// 角色权限Service
        /// </summary>
        private readonly IRolePermissionService _rolePermissionService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="rolePermissionService"></param>
        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        #region 角色
        /// <summary>
        /// 获取角色树
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<RoleInfoModel>), 200)]
        public async Task<ResponseModel> GetRoleTree([FromQuery] string roleName = null)
        {
            return Success(await _rolePermissionService.GetRoleTree(roleName));
        }
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddRole([FromBody] AddRoleModel model)
        {
            return await _rolePermissionService.AddRole(model);
        }
        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditRole([FromBody] EditRoleModel model)
        {
            return await _rolePermissionService.EditRole(model);
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelRole([FromBody] DelRoleModel model)
        {
            return await _rolePermissionService.DelRole(model);
        }
        /// <summary>
        /// 复制角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> CopyRole([FromBody] CopyRoleModel model)
        {
            return await _rolePermissionService.CopyRole(model);
        }
        #endregion

        #region 权限
        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="RoleId">角色Id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<long>), 200)]
        public async Task<ResponseModel> GetRolePermission([FromQuery][Required(ErrorMessage = "角色Id不能为空!")] long? RoleId) 
        {
            return Success(await _rolePermissionService.GetRolePermission((long)RoleId));
        }
        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> SaveRolePermission([FromBody] SaveRolePermissionModel model)
        {
            return await _rolePermissionService.SaveRolePermission(model);
        }
        #endregion
    }
}
