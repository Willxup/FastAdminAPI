using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Network.Models.Wechat;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 微信API
    /// 可由EventBus跨服务调用
    /// </summary>
    public class WechatApiController : BaseController
    {
        /// <summary>
        /// 微信API Service
        /// </summary>
        private readonly IWechatApiService _wechatApiService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="wechatApiService"></param>
        public WechatApiController(IWechatApiService wechatApiService)
        {
            _wechatApiService = wechatApiService;
        }

        /// <summary>
        /// 获取微信接口调用权限签名
        /// </summary>
        /// <param name="appId">微信公众号AppId</param>
        /// <param name="url">请求完整地址</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(WeChatSignModel), 200)]
        public async Task<ResponseModel> GetWechatSign([FromQuery][Required(ErrorMessage = "微信公众号AppId不能为空！")] string appId, 
            [FromQuery][Required(ErrorMessage = "请求地址不能为空!")] string url)
        {
            return await _wechatApiService.GetWechatSign(appId, url);
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
            return await _wechatApiService.GetWechatUserOpenId(appId, code);
        }

    }
}
