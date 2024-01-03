using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Models.Modules;
using FastAdminAPI.Core.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 用户
    /// </summary>
    public interface IUserService
    {
        #region 权限
        /// <summary>
        /// 获取功能权限(当前登录用户所有权限)
        /// 【角色权限+用户权限】
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleInfoModel>> GetPermissions();
        /// <summary>
        /// 获取菜单权限树
        /// </summary>
        /// <returns></returns>
        Task<string> GetMenuPermissionsTree();
        #endregion

        #region 通用审批
        /// <summary>
        /// 获取我的审批列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        Task<ResponseModel> GetCheckList(CheckPageSearch pageSearch);
        /// <summary>
        /// 审批申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> ApprovalApplication(ApprovalModel model);
        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <returns></returns>
        Task<ResponseModel> CancelApplication(long checkId);
        /// <summary>
        /// 获取我的审批记录列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        Task<ResponseModel> GetCheckRecordList(CheckRecordPageSearch pageSearch);
        /// <summary>
        /// 获取申请审批记录列表
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <returns></returns>
        Task<List<CheckRecordModel>> GetApplicationCheckRecords(long checkId);
        #endregion
    }
}
