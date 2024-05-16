using FastAdminAPI.Common.BASE;
using FastAdminAPI.Network.Models.Wechat;
using Refit;
using System.Threading.Tasks;

namespace FastAdminAPI.Network.Interfaces
{
    public interface IWechatApi
    {
        /// <summary>
        /// 获取微信接口调用权限签名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Post("/api/WechatApi/GetWechatSign")]
        Task<ResponseModel> GetWechatSign([Body] WeChatSignRequestModel model);
        /// <summary>
        /// 获取微信公众号用户OpenId
        /// </summary>
        /// <param name="appId">微信公众号AppId</param>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        [Get("/api/WechatApi/GetWechatUserOpenId")]
        Task<ResponseModel> GetWechatUserOpenId([Query] string appId, [Query] string code);
    }
}
