using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.TokenPass
{
    public class QyWechatConfigParameterModel
    {
        /// <summary>
        /// 前端地址
        /// </summary>
        [Required(ErrorMessage = "地址不能为空!")]
        public string Url { get; set; }
    }
    public class QyWechatUserRequestModel
    {
        /// <summary>
        /// 企业微信返回的Code
        /// </summary>
        [Required(ErrorMessage = "企业微信返回的Code不能为空!")]
        public string Code { get; set; }
    }
    public class QyWechatUserResponseModel
    {
        /// <summary>
        /// 企业微信UserId
        /// </summary>
        public string UserId { get; set; }
    }
}
