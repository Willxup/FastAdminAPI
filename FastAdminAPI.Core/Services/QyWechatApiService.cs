using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Network.QyWechat;
using FastAdminAPI.Network.QyWechat.Model;
using System;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 企业微信API
    /// </summary>
    public class QyWechatApiService : BaseService, IQyWechatApiService
    {
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="redis"></param>
        public QyWechatApiService(IRedisHelper redis) : base()
        {
            _redis = redis;
        }

        /// <summary>
        /// 获取企业微信UserId
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetUserId(string code)
        {
            try
            {
                QyWechatClient client = new(_redis);
                string result = await client.GetUserId(code);
                return ResponseModel.Success(result);
            }
            catch (Exception ex)
            {
                throw new UserOperationException("获取企业微信UserId失败! " + ex.Message);
            }
        }
        /// <summary>
        /// 发送企业微信文本消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Id"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SendTextMessage(MessageSendModel model, long Id = 0, string task = "")
        {
            try
            {
                QyWechatClient client = new(_redis);
                return await client.SendMessage(model);
            }
            catch (Exception ex)
            {
                throw new UserOperationException("发送企业微信应用消息失败! " + ex.Message);
            }
        }
        /// <summary>
        /// 发送企业微信卡片消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Id"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SendCardMessage(CardMsgSendModel model, long Id = 0, string task = "")
        {
            try
            {
                QyWechatClient client = new(_redis);
                return await client.SendMessage(model);
            }
            catch (Exception ex)
            {
                throw new UserOperationException("发送企业微信应用消息失败! " + ex.Message);
            }
        }

    }

}
