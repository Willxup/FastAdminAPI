using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.BasicSettings;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 基础设置
    /// </summary>
    public class BasicSettingsController : BaseController
    {
        /// <summary>
        /// 基础设置Service
        /// </summary>
        private readonly IBasicSettingsService _basicSettingsService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="basicSettingsService"></param>
        public BasicSettingsController(IBasicSettingsService basicSettingsService)
        {
            _basicSettingsService = basicSettingsService;
        }

        #region 字典
        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<CodePageResult>), 200)]
        public async Task<ResponseModel> GetCodeList([FromBody] CodePageSearch pageSearch)
        {
            return await _basicSettingsService.GetCodeList(pageSearch);
        }
        /// <summary>
        /// 获取字典分组列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<GroupCodeModel>), 200)]
        public async Task<ResponseModel> GetGroupCodeList()
        {
            return Success(await _basicSettingsService.GetGroupCodeList());
        }
        /// <summary>
        /// 新增字典
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddCode([FromBody] AddCodeModel model)
        {
            return await _basicSettingsService.AddCode(model);
        }
        /// <summary>
        /// 编辑字典
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditCode([FromBody] EditCodeModel model)
        {
            return await _basicSettingsService.EditCode(model);
        }
        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="CodeId">字典Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelCode([FromQuery][Required(ErrorMessage = "字典Id不能为空!")] long? CodeId)
        {
            return await _basicSettingsService.DelCode((long)CodeId);
        }
        #endregion

        #region 数据权限设置
        /// <summary>
        /// 获取数据权限设置列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<DataPermissionSettingsPageResult>), 200)]
        public async Task<ResponseModel> GetDataPermissionSettingList([FromBody] DataPermissionSettingsPageSearch pageSearch)
        {
            return await _basicSettingsService.GetDataPermissionSettingList(pageSearch);
        }
        /// <summary>
        /// 新增数据权限设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddDataPermissionSetting([FromBody] AddDataPermissionSettingModel model)
        {
            return await _basicSettingsService.AddDataPermissionSetting(model);
        }
        /// <summary>
        /// 编辑数据权限设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditDataPermissionSetting([FromBody] EditDataPermissionSettingModel model)
        {
            return await _basicSettingsService.EditDataPermissionSetting(model);
        }
        /// <summary>
        /// 删除数据权限设置
        /// </summary>
        /// <param name="DataPermissionId">数据权限设置Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelDataPermissionSetting([FromQuery][Required(ErrorMessage = "数据权限设置Id不能为空!")] long? DataPermissionId)
        {
            return await _basicSettingsService.DelDataPermissionSetting((long)DataPermissionId);
        }
        #endregion

        #region 审批流程设置
        /// <summary>
        /// 获取审批流程列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<CheckProcessPageResult>), 200)]
        public async Task<ResponseModel> GetCheckProcessList([FromBody] CheckProcessPageSearch pageSearch)
        {
            return await _basicSettingsService.GetCheckProcessList(pageSearch);
        }
        /// <summary>
        /// 新增审批流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddCheckProcess([FromBody] AddCheckProcessModel model)
        {
            return await _basicSettingsService.AddCheckProcess(model);
        }
        /// <summary>
        /// 编辑审批流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditCheckProcess([FromBody] EditCheckProcessModel model)
        {
            return await _basicSettingsService.EditCheckProcess(model);
        }
        /// <summary>
        /// 删除审批流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelCheckProcess([FromBody] DelCheckProcessModel model)
        {
            return await _basicSettingsService.DelCheckProcess(model);
        }
        #endregion

    }
}
