using System.Threading.Tasks;
using FastAdminAPI.Network.Wechat.Config;
using FastAdminAPI.Network.Wechat.Models;
using Refit;

namespace FastAdminAPI.Network.Wechat.Common
{
    internal interface IWechatProvider
    {
        #region 通用
        /// <summary>
        /// 获取微信网页授权
        /// </summary>
        /// <param name="appid">AppID</param>
        /// <param name="secret">App密钥</param>
        /// <param name="code">Code</param>
        /// <param name="grant_type">授权类型(默认authorization_code)</param>
        /// <returns></returns>
        [Get(BaseWechatConfiguration.GET_WECHAT_WEB_AUTHORIZATION)]
        Task<WechatWebAuthorizationModel> GetWechatWebAuthorization([Query] string appid, [Query] string secret, [Query] string code, [Query] string grant_type = "authorization_code");
        #endregion

        #region 公众号
        /// <summary>
        /// 获取微信公众号Token
        /// </summary>
        /// <param name="appid">AppID</param>
        /// <param name="secret">App密钥</param>
        /// <returns></returns>
        [Get(BaseWechatConfiguration.GET_WECHAT_TOKEN)]
        Task<WechatTokenModel> GetWechatToken([Query] string appid, [Query] string secret);
        /// <summary>
        /// 获取微信公众号Ticket
        /// </summary>
        /// <param name="access_token">访问令牌</param>
        /// <param name="type">类型(默认jsapi)</param>
        [Get(BaseWechatConfiguration.GET_WECHAT_TICKET)]
        Task<WechatTicketModel> GetWechatTicket([Query] string access_token, [Query] string type = "jsapi");
        #endregion
    }
}
