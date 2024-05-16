using FastAdminAPI.Common.BASE;
using FastAdminAPI.Network.Models.Wechat;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 微信API
    /// </summary>
    public interface IWechatApiService
    {
        /// <summary>
        /// 获取微信接口调用权限签名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> GetWechatSign(WeChatSignRequestModel model);
        /// <summary>
        /// 获取微信公众号用户OpenId
        /// </summary>
        /// <param name="appId">微信公众号AppId</param>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        Task<ResponseModel> GetWechatUserOpenId(string appId, string code);
    }
}
