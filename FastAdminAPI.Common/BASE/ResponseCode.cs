using System.ComponentModel;

namespace FastAdminAPI.Common.BASE
{
    /// <summary>
    /// 系统响应
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 200,
        /// <summary>
        /// 请求已成功处理，并且在服务器上创建了新的资源
        /// </summary>
        [Description("请求已成功处理")]
        Create = 201,
        /// <summary>
        /// 服务器成功处理了请求，但没有返回任何内容。
        /// </summary>
        [Description("服务器成功处理了请求")]
        NoContent = 204,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("警告")]
        Warn = 210,
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Error = 400,
        /// <summary>
        /// 未授权
        /// </summary>
        [Description("未授权")]
        Unauthorized = 401,
        /// <summary>
        /// 禁止访问
        /// </summary>
        [Description("禁止访问")]
        Forbidden = 403,
        /// <summary>
        /// 未找到
        /// </summary>
        [Description("未找到")]
        NotFound =404,
        /// <summary>
        /// 服务器内部错误。服务器遇到了意外情况，无法完成请求。
        /// </summary>
        [Description("服务器内部错误")]
        InternalServerError = 500,
        /// <summary>
        /// 无效令牌
        /// </summary>
        [Description("无效令牌")]
        InvalidToken = 600,
        /// <summary>
        /// token过期
        /// </summary>
        [Description("令牌过期")]
        ExpirationToken = 601,
        /// <summary>
        /// 其他设备登录
        /// </summary>
        [Description("其他设备登录")]
        OtherDeviceLogin = 602,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 900
    }
}
