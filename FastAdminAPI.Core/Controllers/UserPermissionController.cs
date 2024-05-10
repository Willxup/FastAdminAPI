using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.UserPermission;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{

    /// <summary>
    /// 用户功能权限
    /// </summary>
    public class UserPermissionController : BaseController
    {
        /// <summary>
        /// 用户权限处理接口
        /// </summary>
        private readonly IUserPermissionService _userPermissionService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userPermissionService"></param>
        public UserPermissionController(IUserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }

        /// <summary>
        /// 获取所有用户功能权限列表
        /// 【角色权限+用户权限】
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<UserPermssionPageResult>), 200)]
        public async Task<ResponseModel> GetUserPermissions([FromBody] UserPermssionPageSearch pageSearch)
        {
            return await _userPermissionService.GetUserPermissions(pageSearch);
        }
    }
}
