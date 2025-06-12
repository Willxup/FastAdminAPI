using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.EventBus.Controllers.BASE;
using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.Models.Email;
using FastAdminAPI.Network.Models.QyWechat;
using FastAdminAPI.Network.Models.Wechat;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.EventBus.Controllers
{
    /// <summary>
    /// 跨服务API
    /// 使用Refit服务间调用接口示例
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
        /// 企业微信API
        /// </summary>
        private readonly IQyWechatApi _qyWechatApi;

        /// <summary>
        /// 跨服务API
        /// </summary>
        public CrossApiController(IEmailApi emailApi, IWechatApi wechatApi, IQyWechatApi qyWechatApi)
        {
            _emailApi = emailApi;
            _wechatApi = wechatApi;
            _qyWechatApi = qyWechatApi;
        }

        #region 邮件
        /// <summary>
        /// 发送Smtp邮件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> SendSmtpEmail([FromBody] SendSmtpEmailModel model)
        {
            try
            {
                await _emailApi.SendSmtpEmail(model);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return Success();
        }
        /// <summary>
        /// 发送smtp邮件(支持多个收件人及抄送人)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> SendMultipleSmtpEmail([FromBody] SendMultipleSmtpEmailModel model)
        {
            try
            {
                await _emailApi.SendMultipleSmtpEmail(model);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return Success();
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
            try
            {
                await _emailApi.SendEmailByDefault(title, body);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return Success();
        }
        #endregion

        #region 微信
        /// <summary>
        /// 获取微信用户OpenId
        /// </summary>
        /// <param name="appId">微信AppId</param>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> GetWechatUserOpenId([FromQuery][Required(ErrorMessage = "微信公众号AppId不能为空!")] string appId,
            [FromQuery][Required(ErrorMessage = "授权code不能为空!")] string code)
        {
            return await _wechatApi.GetWechatUserOpenId(appId, code);
        }
        /// <summary>
        /// 获取微信公众号接口调用权限签名
        /// </summary>
        /// <param name="appId">微信公众号AppId</param>
        /// <param name="url">url地址</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(WeChatSignModel), 200)]
        public async Task<ResponseModel> GetWeChatAccessToken([FromQuery][Required(ErrorMessage = "微信公众号AppId不能为空！")] string appId,
            [FromQuery][Required(ErrorMessage = "请求地址不能为空!")] string url)
        {
            return await _wechatApi.GetWechatSign(appId, url);
        }
        #endregion

        #region 企业微信
        /// <summary>
        /// 获取企业微信UserId
        /// </summary>
        /// <param name="code">企业微信code</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> GetUserId([FromQuery] string code)
        {
            return await _qyWechatApi.GetUserId(code);
        }
        /// <summary>
        /// 发送企业微信文本消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> SendTextMessage([FromBody] MessageSendModel model)
        {
            return await _qyWechatApi.SendTextMessage(model);
        }
        /// <summary>
        /// 发送企业微信卡片消息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        public async Task<ResponseModel> SendCardMessage([FromBody] CardMsgSendModel model)
        {
            return await _qyWechatApi.SendCardMessage(model);
        }
        #endregion
    }
}
