using System.Collections.Generic;

namespace FastAdminAPI.Network.Wechat.Config
{
    internal class BaseWechatConfiguration
    {
        /// <summary>
        /// 微信公众号AppId与密钥
        /// </summary>
        internal static readonly Dictionary<string, string> WECHAT_APP_DIC = new()
        {
            { "appid1", "ed115694d204edsadasdwillxup4334e" },
            { "appid2", "ed11sdsad204edsadasdwillxup4011c" },
            { "appid3", "ed11sacs4d204edsadasdwillxup976a" },
        };

        /// <summary>
        /// 微信域名地址
        /// </summary>
        internal const string WECHAT_DOMAIN_ADDRESS = "https://api.weixin.qq.com";

        /// <summary>
        /// 获取微信token地址
        /// </summary>
        internal const string GET_WECHAT_TOKEN = "/cgi-bin/token?grant_type=client_credential";
        /// <summary>
        /// 微信token Redis缓存键
        /// </summary>
        internal const string WECHAT_TOKEN_REDIS_KEY = "WeChat_Share:Token_";
        /// <summary>
        /// 微信token Redis过期时间
        /// </summary>
        internal const int WECHAT_TOKEN_REDIS_EXPIRES = 7200;

        /// <summary>
        /// 获取微信Ticket地址
        /// </summary>
        internal const string GET_WECHAT_TICKET = "/cgi-bin/ticket/getticket";
        /// <summary>
        /// 获取微信Ticket Redis缓存键
        /// </summary>
        internal const string WECHAT_TICKET_REDIS_KEY = "WeChat_Share:Ticket_";
        /// <summary>
        /// 获取微信Ticket Redis过期时间
        /// </summary>
        internal const int WECHAT_TICKET_REDIS_EXPIRES = 7200;

        /// <summary>
        /// 获取微信签名 Redis缓存键
        /// </summary>
        internal const string WECHAT_SIGN_REDIS_KEY = "WeChat_Share:Sign_";
        /// <summary>
        /// 获取微信签名 Redis过期时间
        /// </summary>
        internal const int WECHAT_SIGN_REDIS_EXPIRES = 7200;

        /// <summary>
        /// 获取微信网页授权
        /// </summary>
        internal const string GET_WECHAT_WEB_AUTHORIZATION = "/sns/oauth2/access_token";
    }
}
