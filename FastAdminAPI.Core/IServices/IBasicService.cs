using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Models.BasicSettings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 基础设置
    /// </summary>
    public interface IBasicService
    {
        #region 字典
        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        Task<ResponseModel> GetCodeList(CodePageSearch pageSearch);
        /// <summary>
        /// 获取字典分组列表
        /// </summary>
        /// <returns></returns>
        Task<List<GroupCodeModel>> GetGroupCodeList();
        /// <summary>
        /// 新增字典
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddCode(AddCodeModel model);
        /// <summary>
        /// 编辑字典
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> EditCode(EditCodeModel model);
        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="codeId">字典Id</param>
        /// <returns></returns>
        Task<ResponseModel> DelCode(long codeId);
        #endregion

        #region 数据权限设置
        /// <summary>
        /// 获取数据权限设置列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        Task<ResponseModel> GetDataPermissionSettingList(DataPermissionSettingsPageSearch pageSearch);
        /// <summary>
        /// 新增数据权限设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        Task<ResponseModel> AddDataPermissionSetting(AddDataPermissionSettingModel model);
        /// <summary>
        /// 编辑数据权限设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        Task<ResponseModel> EditDataPermissionSetting(EditDataPermissionSettingModel model);
        /// <summary>
        /// 删除数据权限设置
        /// </summary>
        /// <param name="dataPermissionId"></param>
        /// <returns></returns>
        Task<ResponseModel> DelDataPermissionSetting(long dataPermissionId);
        #endregion

        #region 审批流程设置
        /// <summary>
        /// 获取审批流程列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        Task<ResponseModel> GetCheckProcessList(CheckProcessPageSearch pageSearch);
        /// <summary>
        /// 新增审批流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddCheckProcess(AddCheckProcessModel model);
        /// <summary>
        /// 编辑审批流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> EditCheckProcess(EditCheckProcessModel model);
        /// <summary>
        /// 删除审批流程
        /// </summary>
        /// <param name="checkProcessId">审批流程Id</param>
        /// <returns></returns>
        Task<ResponseModel> DelCheckProcess(long checkProcessId);
        #endregion

    }
}
