using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Network.Models.Wechat;
using FastAdminAPI.Network.Wechat;
using System;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 微信API
    /// </summary>
    public class WechatApiService : BaseService, IWechatApiService
    {
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="redis"></param>
        public WechatApiService(IRedisHelper redis)
        {
            _redis = redis;
        }

        /// <summary>
        /// 获取微信接口调用权限签名
        /// </summary>
        /// <param name="appId">微信公众号AppId</param>
        /// <param name="url">请求完整地址</param>
        /// <returns></returns>
        public async Task<ResponseModel> GetWechatSign(string appId, string url)
        {
            ResponseModel result = ResponseModel.Success();

            try
            {

                WeChatOfficialAccountsClient share = new(_redis, appId);

                result.Data = await share.GetSign(url);
            }
            catch (Exception ex)
            {
                throw new UserOperationException($"获取微信公众号签名失败：{ex.Message}");
            }

            return result;
        }
        /// <summary>
        /// 获取微信公众号用户OpenId
        /// </summary>
        /// <param name="appId">微信公众号AppId</param>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        public async Task<ResponseModel> GetWechatUserOpenId(string appId, string code)
        {
            ResponseModel result = ResponseModel.Success();

            try
            {

                WeChatOfficialAccountsClient share = new(_redis, appId);

                result.Data = await share.GetWechatUserOpenId(code);
            }
            catch (Exception ex)
            {
                throw new UserOperationException($"{ex.Message}");
            }

            return result;
        }
    }
}
