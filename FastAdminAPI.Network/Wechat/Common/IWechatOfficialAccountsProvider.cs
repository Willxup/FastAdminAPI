using FastAdminAPI.Network.Wechat.Config;
using FastAdminAPI.Network.Wechat.Models;
using Refit;
using System.Threading.Tasks;

namespace FastAdminAPI.Network.Wechat.Common
{
    internal interface IWechatOfficialAccountsProvider
    {
        /// <summary>
        /// 获取微信Token
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        [Get(BaseWechatConfiguration.GET_WECHAT_TOKEN)]
        Task<WechatTokenModel> GetWechatToken([Query] string appid, [Query] string secret);
        /// <summary>
        /// 获取微信Ticket
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="type"></param>
        [Get(BaseWechatConfiguration.GET_WECHAT_TICKET)]
        Task<WechatTicketModel> GetWechatTicket([Query] string access_token, [Query] string type = "jsapi");
        /// <summary>
        /// 获取微信网页授权
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="code"></param>
        /// <param name="grant_type"></param>
        /// <returns></returns>
        [Get(BaseWechatConfiguration.GET_WECHAT_WEB_AUTHORIZATION)]
        Task<WechatWebAuthorizationModel> GetWechatWebAuthorization([Query] string appid, [Query] string secret, [Query] string code, [Query] string grant_type = "authorization_code");

    }
}
