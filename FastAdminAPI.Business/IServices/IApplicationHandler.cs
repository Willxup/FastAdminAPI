using FastAdminAPI.Business.PrivateFunc.Applications.Models;
using FastAdminAPI.Common.BASE;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.IServices
{
    /// <summary>
    /// 申请处理者
    /// </summary>
    public interface IApplicationHandler
    {

        /// <summary>
        /// 设置申请
        /// 所需条件：1.开启事务 2.是否需要消息通知
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSendApprovalNotice">是否开启审批通知(默认是)</param>
        /// <returns></returns>
        Task<ResponseModel> SetApplication(SetApplicationModel model, bool isSendApprovalNotice = true);
        /// <summary>
        /// 审批
        /// 所需条件：1.开启事务 2.是否需要消息通知
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSendApporvalNotice">是否开启审批通知(默认是)</param>
        /// <param name="isSendApprovalCCNotice">是否开启审批抄送通知(默认是)</param>
        /// <returns></returns>
        Task<ResponseModel> ProcessingApplication(ProcessingApplicationModel model, bool isSendApporvalNotice = true, bool isSendApprovalCCNotice = true);
        /// <summary>
        /// 设置申请并通过
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AcceptApplication(SetApplicationModel model);
    }
}
