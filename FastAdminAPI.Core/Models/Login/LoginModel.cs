using System.ComponentModel.DataAnnotations;
using FastAdminAPI.Common.Attributes.CheckValidators;
using FastAdminAPI.Common.Enums;

namespace FastAdminAPI.Core.Models.Login
{
    public class LoginByPasswordModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage = "账号不能为空!")]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空!")]
        public string Password { get; set; }
        /// <summary>
        /// 设备 0PC 1企业微信
        /// </summary>
        [EnumCheck(typeof(BusinessEnums.LoginDevice))]
        public int Device { get; set; } = (byte)BusinessEnums.LoginDevice.PC;
    }
    public class LoginByQyWechatModel
    {
        /// <summary>
        /// 企业微信Code
        /// </summary>
        [Required(ErrorMessage = "企业微信Code不能为空!")]
        public string Code { get; set; }
        /// <summary>
        /// 设备 0PC 1企业微信
        /// </summary>
        [EnumCheck(typeof(BusinessEnums.LoginDevice))]
        public int Device { get; set; } = (byte)BusinessEnums.LoginDevice.PC;
    }
    public class LoginByQyWechatResponseModel
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 企业微信UserId
        /// </summary>
        public string QyUserId { get; set; }
    }
}
