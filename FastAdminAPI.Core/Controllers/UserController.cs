using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Modules;
using FastAdminAPI.Core.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserController : BaseController
    {
        /// <summary>
        /// 用户Service
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="userService"></param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region 权限
        /// <summary>
        /// 获取功能权限(当前登录用户所有权限)
        /// 【角色权限+用户权限】
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ModuleInfoModel>), 200)]
        public async Task<ResponseModel> GetPermissions()
        {
            return Success(await _userService.GetPermissions());
        }
        /// <summary>
        /// 获取菜单权限树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ModuleInfoModel>), 200)]
        public async Task<ResponseModel> GetMenuPermissionsTree()
        {
            return Success(await _userService.GetMenuPermissionsTree());
        }
        #endregion

        #region 通用审批
        /// <summary>
        /// 获取我的审批列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<CheckPageResult>), 200)]
        public async Task<ResponseModel> GetCheckList([FromBody] CheckPageSearch pageSearch)
        {
            return await _userService.GetCheckList(pageSearch);
        }
        /// <summary>
        /// 审批申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> ProcessApplication([FromBody] ApprovalModel model)
        {
            return await _userService.ProcessApplication(model);
        }
        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> CancelApplication([FromQuery][Required(ErrorMessage = "审批Id不能为空!")] long? checkId)
        {
            return await _userService.CancelApplication((long)checkId);
        }
        /// <summary>
        /// 获取我的审批记录列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<CheckRecordPageResult>), 200)]
        public async Task<ResponseModel> GetCheckRecordList([FromBody] CheckRecordPageSearch pageSearch)
        {
            return await _userService.GetCheckRecordList(pageSearch);
        }
        /// <summary>
        /// 获取申请审批记录列表
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CheckRecordModel>), 200)]
        public async Task<ResponseModel> GetApplicationCheckRecords([FromQuery][Required(ErrorMessage = "审批Id不能为空!")] long? checkId)
        {
            return Success(await _userService.GetApplicationCheckRecords((long)checkId));
        }
        #endregion
    }
}
