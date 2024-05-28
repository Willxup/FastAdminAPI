using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Config;
using FastAdminAPI.Network.Models.Wechat;
using FastAdminAPI.Network.Wechat.Common;
using FastAdminAPI.Network.Wechat.Config;
using FastAdminAPI.Network.Wechat.Models;
using Newtonsoft.Json;
using Refit;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FastAdminAPI.Network.Wechat
{
    /// <summary>
    /// 微信公众号Client
    /// </summary>
    public class WeChatOfficialAccountsClient
    {
        /// <summary>
        /// 微信公众号AppId
        /// </summary>
        private readonly string WECHAT_APP_ID;
        /// <summary>
        /// 微信公众号App密钥
        /// </summary>
        private readonly string WECHAT_APP_SECRET;
        /// <summary>
        /// 随机字符串
        /// </summary>
        private static readonly string _randomStr = "ticketKeygdfewghsaef1234willxup";

        /// <summary>
        /// redis
        /// </summary>
        private readonly IRedisHelper _redis;
        /// <summary>
        /// 微信公众号
        /// </summary>
        private readonly IWechatOfficialAccountsProvider _wechatOfficialAccountsProvider;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="redis">redis缓存</param>
        public WeChatOfficialAccountsClient(IRedisHelper redis, string appId)
        {
            // 获取AppId和App密钥
            if (BaseWechatConfiguration.WECHAT_APP_DIC.TryGetValue(appId, out string appSecret))
            {
                WECHAT_APP_ID = appId;
                WECHAT_APP_SECRET = appSecret;
            }
            else
                throw new UserOperationException("获取微信公众号信息失败!");

            _redis = redis;

            _wechatOfficialAccountsProvider = RestService.For<IWechatOfficialAccountsProvider>(BaseWechatConfiguration.WECHAT_DOMAIN_ADDRESS, RefitConfigExtension.REFIT_SETTINGS);
        }


        #region 内部调用
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessToken()
        {
            try
            {
                var result = await _wechatOfficialAccountsProvider.GetWechatToken(WECHAT_APP_ID, WECHAT_APP_SECRET);

                if (result.errcode == 0)
                {
                    return result.access_token;
                }
                else
                {
                    NLogHelper.Error($"获取微信公众号Token失败:{JsonConvert.SerializeObject(result)}");
                    throw new UserOperationException($"获取微信公众号Token失败:{result.errmsg}");
                }
            }
            catch (UserOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"获取微信公众号Token出错，{ex.Message}", ex);
                throw new UserOperationException($"获取微信公众号Token失败!");
            }
        }
        /// <summary>
        /// 获取JsApiTicket
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetJsApiTicket()
        {
            try
            {
                var result = await _wechatOfficialAccountsProvider.GetWechatTicket(await GetToken(), "jsapi");

                if (result.errcode == 0)
                {
                    return result.ticket;
                }
                else
                {
                    NLogHelper.Error($"获取微信公众号Ticket失败:{JsonConvert.SerializeObject(result)}");
                    throw new UserOperationException($"获取微信公众号Ticket失败:{result.errmsg}");
                }
            }
            catch (UserOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"获取微信公众号Ticket出错:{ex.Message}", ex);
                throw new UserOperationException($"获取微信公众号Ticket失败!");
            }
        }
        /// <summary>
        /// 获取微信网页授权
        /// </summary>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        private async Task<WechatWebAuthorizationModel> GetWechatWebAuthorization(string code)
        {
            try
            {
                var result = await _wechatOfficialAccountsProvider.GetWechatWebAuthorization(WECHAT_APP_ID, WECHAT_APP_SECRET, code, "authorization_code");

                // success
                if (result.errcode == 0)
                {
                    return result;
                }
                // code has been used
                else if (result.errcode == 40163)
                {
                    throw new UserOperationException("请重新进入此页面!");
                }
                else
                {
                    NLogHelper.Error($"获取微信公众号网页授权失败:{JsonConvert.SerializeObject(result)}");
                    throw new UserOperationException($"获取微信公众号网页授权失败:{result.errmsg}");
                }
            }
            catch (UserOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"获取微信公众号网页授权出错，{ex.Message}", ex);
                throw new UserOperationException($"获取微信公众号网页授权失败!");
            }
        }
        /// <summary>
        /// Sha1加密签名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string Sha1Sign(string str)
        {
#pragma warning disable SYSLIB0021 // 类型或成员已过时
            SHA1 sha1 = new SHA1CryptoServiceProvider();
#pragma warning restore SYSLIB0021 // 类型或成员已过时

            byte[] bytes_sha1_in = Encoding.Default.GetBytes(str);

            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);

            string signature = BitConverter.ToString(bytes_sha1_out);

            signature = signature.Replace("-", "").ToLower();

            return signature;
        }
        /// <summary>
        /// 获取微信JS-JDK时间戳
        /// </summary>
        /// <returns></returns>
        private static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);

            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// JS-JDK 创建随机字符串
        /// </summary>
        /// <returns></returns>
        private static string CreatenNonce_str()
        {
            Random r = new();
            var sb = new StringBuilder();

            var length = _randomStr.Length;

            for (int i = 0; i < 15; i++)
            {
                sb.Append(_randomStr[r.Next(length - 1)]);
            }

            return sb.ToString();
        }
        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="jsapi_ticket"></param>
        /// <param name="noncestr">随机字符串(必须与wx.config中的nonceStr相同)</param>
        /// <param name="timestamp">时间戳(必须与wx.config中的timestamp相同)</param>
        /// <param name="url">当前网页的URL，不包含#及其后面部分(必须是调用JS接口页面的完整URL)</param>
        /// <returns></returns>
        private static string GetSignature(string jsapi_ticket, string noncestr, string timestamp, string url)
        {
            if (string.IsNullOrEmpty(jsapi_ticket) || string.IsNullOrEmpty(noncestr) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(url))
                return null;

            var string1Builder = new StringBuilder();

            string1Builder.Append("jsapi_ticket=").Append(jsapi_ticket).Append('&')
                          .Append("noncestr=").Append(noncestr).Append('&')
                          .Append("timestamp=").Append(timestamp).Append('&')
                          .Append("url=").Append(url.Contains('#', StringComparison.CurrentCulture) ? url[..url.IndexOf("#")] : url);

            return Sha1Sign(string1Builder.ToString());
        }
        #endregion

        /// <summary>
        /// 获取token
        /// </summary>       
        /// <returns></returns>
        public async Task<string> GetToken()
        {
            string token;

            string key = BaseWechatConfiguration.WECHAT_TOKEN_REDIS_KEY + "_" + WECHAT_APP_ID;

            if (_redis.KeyExists(key))
            {
                token = await _redis.StringGetAsync(key);
            }
            else
            {
                await _redis.StringSetAsync(key, await GetAccessToken(), TimeSpan.FromSeconds(BaseWechatConfiguration.WECHAT_TOKEN_REDIS_EXPIRES));
                token = await _redis.StringGetAsync(key);
            }

            return token;
        }
        /// <summary>
        /// 获取ticket
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTicket()
        {
            string ticket;

            string key = BaseWechatConfiguration.WECHAT_TICKET_REDIS_KEY + "_" + WECHAT_APP_ID;

            if (_redis.KeyExists(key))
            {
                ticket = await _redis.StringGetAsync(key);
            }
            else
            {
                await _redis.StringSetAsync(key, await GetJsApiTicket(), TimeSpan.FromSeconds(BaseWechatConfiguration.WECHAT_TICKET_REDIS_EXPIRES));
                ticket = await _redis.StringGetAsync(key);
            }

            return ticket;
        }
        /// <summary>
        /// 获取微信分享签名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<WeChatSignModel> GetSign(string url)
        {
            try
            {
                string key = BaseWechatConfiguration.WECHAT_SIGN_REDIS_KEY + "_" + url;

                if (_redis.KeyExists(key))
                {
                    return _redis.StringGet<WeChatSignModel>(key);
                }
                else
                {
                    WeChatSignModel signResult = new()
                    {
                        AppId = WECHAT_APP_ID,
                        Noncestr = CreatenNonce_str(),
                        Timestamp = GetTimeStamp(),
                        JsApiTicket = await GetTicket()
                    };

                    signResult.Sign = GetSignature(signResult.JsApiTicket, signResult.Noncestr, signResult.Timestamp, url);

                    await _redis.StringSetAsync(key, signResult, TimeSpan.FromSeconds(BaseWechatConfiguration.WECHAT_SIGN_REDIS_EXPIRES));

                    return _redis.StringGet<WeChatSignModel>(key);
                }
            }
            catch (Exception ex)
            {

                NLogHelper.Error($"获取微信公众号权限签名失败!{ex.Message}");
                throw new Exception($"获取微信公众号权限签名失败!");
            }
        }
        /// <summary>
        /// 获取微信公众号用户OpenId
        /// </summary>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        public async Task<string> GetWechatUserOpenId(string code)
        {
            var result = await GetWechatWebAuthorization(code);

            return result.openid;
        }
    }
}
