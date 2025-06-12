using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Network.Models.QyWechat;
using Refit;

namespace FastAdminAPI.Network.Interfaces
{
    /// <summary>
    /// 企业微信API
    /// </summary>
    public interface IQyWechatApi
    {
        /// <summary>
        /// 获取企业微信UserId
        /// </summary>
        /// <param name="code">企业微信code</param>
        /// <returns></returns>
        [Get("/api/QyWechatApi/GeUserId")]
        Task<ResponseModel> GetUserId([Query] string code);
        /// <summary>
        /// 发送企业微信文本消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Post("/api/QyWechatApi/SendTextMessage")]
        Task<ResponseModel> SendTextMessage([Body] MessageSendModel model);
        /// <summary>
        /// 发送企业微信卡片消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Post("/api/QyWechatApi/SendCardMessage")]
        Task<ResponseModel> SendCardMessage([Body] CardMsgSendModel model);
    }
}
