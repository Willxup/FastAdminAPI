using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Common.SystemUtilities;
using FastAdminAPI.Network.Config;
using FastAdminAPI.Network.QyWechat.Common;
using FastAdminAPI.Network.QyWechat.Config;
using FastAdminAPI.Network.QyWechat.Model;
using Refit;
using System;
using System.Threading.Tasks;

namespace FastAdminAPI.Network.QyWechat
{
    public class QyWechatClient
    {
        /// <summary>
        /// Token
        /// </summary>
        private readonly string ACCESS_TOKEN;
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;
        /// <summary>
        /// 企业微信API
        /// </summary>
        private readonly IQyWechatProvider _qyWechatProvider;
        /// <summary>
        /// 企业微信配置
        /// </summary>
        private readonly BaseQyWechatConfiguration _qyWechatConfig;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="httpClient"></param>
        /// <exception cref="Exception"></exception>
        public QyWechatClient(IRedisHelper redis)
        {
            if (EnvironmentHelper.IsDevelop)
                _qyWechatConfig = new DevQyWechatConfiguration();
            else
                throw new Exception("当前环境暂不支持企业微信!");

            _redis = redis;
            _qyWechatProvider = RestService.For<IQyWechatProvider>(BaseQyWechatConfiguration.QYWECHAT_DOMAIN_ADDRESS, RefitConfigExtension.REFIT_SETTINGS);
            
            ACCESS_TOKEN = GetAccessTokenAsync().Result;
        }

        #region 内部使用
        /// <summary>
        /// 从redis中获取AccessToken
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var key = "QyWechat:AccessToken";
                if (_redis.KeyExists(key))
                {
                    return await _redis.StringGetAsync(key);
                }
                else
                {
                    await _redis.StringSetAsync(key, GetAccessToken(), TimeSpan.FromSeconds(7200));
                    return await _redis.StringGetAsync(key);
                }
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"获取企业微信AccessToken失败,{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 获取企业微信的访问令牌
        /// </summary>
        /// <returns></returns>
        private string GetAccessToken()
        {
            try
            {
                var result = _qyWechatProvider.GetAccessToken(_qyWechatConfig.Corpid, _qyWechatConfig.CorpSecret);

                if (result?.errcode == 0)
                    return result.access_token;
                else
                    throw new Exception($"获取访问令牌失败：{result.errmsg}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 获取企业微信UserId
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> GetUserId(string code)
        {
            try
            {
                var result = await _qyWechatProvider.GeUserId(ACCESS_TOKEN, code);

                if (result.errcode == 0)
                {
                    return result.UserId;

                }
                else
                    throw new Exception($"[{result.errcode}:{result.errmsg}]");
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"获取企业微信用户信息失败，{ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 发送应用消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SendMessage(MessageSendBaseModel model)
        {
            ResponseModel response = ResponseModel.Success();
            model.agentid = _qyWechatConfig.Agentid;
            try
            {
                var result = await _qyWechatProvider.SendMessage(ACCESS_TOKEN, model);

                if (result.errcode != 0)
                {
                    NLogHelper.Error($"企业微信应用消息发送失败，原因：{result.errmsg}");
                    throw new Exception($"企业微信应用消息发送失败，原因：{result.errmsg}");
                }
                response.Data = result;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"发送失败，开始重试，错误原因：{ex.Message}");
            }
            return response;
        }
    }
}
