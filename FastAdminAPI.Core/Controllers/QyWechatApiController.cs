using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Network.Models.QyWechat;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 企业微信API
    /// 可由EventBus跨服务调用
    /// </summary>
    public class QyWechatApiController : BaseController
    {
        /// <summary>
        /// 企业微信API Service
        /// </summary>
        private readonly IQyWechatApiService _qyWechatApiService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="qyWechatApiService"></param>
        public QyWechatApiController(IQyWechatApiService qyWechatApiService)
        {
            _qyWechatApiService = qyWechatApiService;
        }

        /// <summary>
        /// 获取企业微信UserId
        /// </summary>
        /// <param name="code">企业微信code</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> GetUserId([FromQuery] string code)
        {
            return await _qyWechatApiService.GetUserId(code);
        }
        /// <summary>
        /// 发送企业微信文本消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> SendTextMessage([FromBody] MessageSendModel model, [FromQuery] long id = 0, [FromQuery] string task = "")
        {
            return await _qyWechatApiService.SendTextMessage(model, id, task);
        }
        /// <summary>
        /// 发送企业微信卡片消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> SendCardMessage([FromBody] CardMsgSendModel model, [FromQuery] long id = 0, [FromQuery] string task = "")
        {
            return await _qyWechatApiService.SendCardMessage(model, id, task);
        }
    }

}
