using System.Threading.Tasks;
using FastAdminAPI.Business.PrivateFunc.Applications.Models;
using FastAdminAPI.Common.BASE;

namespace FastAdminAPI.Business.Interfaces
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
        Task<ResponseModel> Apply(SetApplicationModel model, bool isSendApprovalNotice = true);
        /// <summary>
        /// 审批
        /// 所需条件：1.开启事务 2.是否需要消息通知
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSendApporvalNotice">是否开启审批通知(默认是)</param>
        /// <param name="isSendApprovalCCNotice">是否开启审批抄送通知(默认是)</param>
        /// <returns></returns>
        Task<ResponseModel> Approve(ProcessingApplicationModel model, bool isSendApporvalNotice = true, bool isSendApprovalCCNotice = true);
        /// <summary>
        /// 设置申请并通过
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> Accept(SetApplicationModel model);
    }
}
