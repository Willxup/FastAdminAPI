using FastAdminAPI.Network.QyWechat.Config;
using FastAdminAPI.Network.QyWechat.Model;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Threading.Tasks;

namespace FastAdminAPI.Network.QyWechat.Common
{
    public interface IQyWechatProvider
    {
        /// <summary>
        /// 获取企业微信访问令牌
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <returns></returns>
        [Get(BaseQyWechatConfiguration.GetAccessToken)]
        AccessTokenModel GetAccessToken([Query] string corpid, [Query] string corpsecret);
        /// <summary>
        /// 获取企业微信UserId
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [Get(BaseQyWechatConfiguration.GetQyWechatUserId)]
        Task<QyUserInfoModel> GeUserId([Query] string access_token, [Query] string code);
        /// <summary>
        /// 发送企业微信信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [Post(BaseQyWechatConfiguration.SendMessage)]
        Task<MessageSendResultModel> SendMessage([FromQuery] string access_token, [Body] MessageSendBaseModel message);
    }
}
