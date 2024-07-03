using FastAdminAPI.Common.BASE;
using FastAdminAPI.Network.Models.QyWechat;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    public interface IQyWechatApiService
    {
        /// <summary>
        /// 获取企业微信UserId
        /// </summary>
        /// <param name="code">企业微信code</param>
        /// <returns></returns>
        Task<ResponseModel> GetUserId(string code);
        /// <summary>
        /// 发送企业微信文本消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Id"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<ResponseModel> SendTextMessage(MessageSendModel model, long Id = 0, string task = "");
        /// <summary>
        /// 发送企业微信卡片消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Id"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<ResponseModel> SendCardMessage(CardMsgSendModel model, long Id = 0, string task = "");
    }
}
