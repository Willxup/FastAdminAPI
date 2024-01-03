using FastAdminAPI.Business.PrivateFunc.Applications.Models;
using FastAdminAPI.Common.BASE;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.PrivateFunc.Applications
{
    /// <summary>
    /// 申请处理器
    /// </summary>
    public interface IApplicationProcessor
    {
        /// <summary>
        /// 完成申请
        /// </summary>
        /// <param name="applicationCategory">申请类别</param>
        /// <param name="applicationType">申请类型</param>
        /// <param name="data">完成申请所需数据</param>
        /// <returns></returns>
        Task<ResponseModel> CompleteApplication(byte applicationCategory, long applicationType, CompleteApplicationModel data);
        /// <summary>
        /// 拒绝申请
        /// </summary>
        /// <param name="applicationCategory">申请类别</param>
        /// <param name="applicationType">申请类型</param>
        /// <param name="data">完成申请所需数据</param>
        /// <returns></returns>
        Task<ResponseModel> RejectApplication(byte applicationCategory, long applicationType, CompleteApplicationModel data);
    }
}
