using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using Refit;

namespace FastAdminAPI.Network.Interfaces
{
    /// <summary>
    /// 微信API
    /// </summary>
    public interface IWechatApi
    {
        #region 通用
        /// <summary>
        /// 获取微信用户OpenId
        /// </summary>
        /// <param name="appId">微信AppId</param>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        [Get("/api/WechatApi/GetWechatUserOpenId")]
        Task<ResponseModel> GetWechatUserOpenId([Query] string appId, [Query] string code);
        #endregion

        #region 公众号
        /// <summary>
        /// 获取微信公众号接口调用权限签名
        /// </summary>
        /// <param name="appId">微信公众号AppId</param>
        /// <param name="url">url地址</param>
        /// <returns></returns>
        [Get("/api/WechatApi/GetWechatSign")]
        Task<ResponseModel> GetWechatSign([Query] string appId, [Query] string url);
        #endregion
    }
}
