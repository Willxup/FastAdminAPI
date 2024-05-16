using FastAdminAPI.Common.BASE;
using FastAdminAPI.EventBus.Controllers.BASE;
using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.Models.Wechat;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.EventBus.Controllers
{
    /// <summary>
    /// 跨服务API
    /// </summary>
    public class CrossApiController : BaseController
    {
        /// <summary>
        /// 邮件API
        /// </summary>
        private readonly IEmailApi _emailApi;
        /// <summary>
        /// 微信API
        /// </summary>
        private readonly IWechatApi _wechatApi;

        /// <summary>
        /// 跨服务API
        /// </summary>
        public CrossApiController(IEmailApi emailApi, IWechatApi wechatApi)
        {
            _emailApi = emailApi;
            _wechatApi = wechatApi;
        }

        /// <summary>
        /// 获取微信接口调用权限签名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(WeChatSignModel), 200)]
        public async Task<ResponseModel> GetWeChatAccessToken([FromBody] WeChatSignRequestModel model)
        {
            return await _wechatApi.GetWechatSign(model);
        }
        /// <summary>
        /// 获取微信公众号用户OpenId
        /// </summary>
        /// <param name="appId">微信公众号AppId</param>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> GetWechatUserOpenId([FromQuery][Required(ErrorMessage = "微信公众号AppId不能为空!")] string appId,
            [FromQuery][Required(ErrorMessage = "授权code不能为空!")] string code)
        {
            return await _wechatApi.GetWechatUserOpenId(appId, code);
        }
        /// <summary>
        /// 发送Smtp邮件
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="body">内容</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> SendEmailByDefault([FromQuery][Required(ErrorMessage = "标题不能为空!")] string title, [FromQuery] string body)
        {
            await _emailApi.SendEmailByDefault(title, body);
            return Success();
        }
    }
}
