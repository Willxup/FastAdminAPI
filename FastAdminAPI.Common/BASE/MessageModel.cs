namespace FastAdminAPI.Common.BASE
{
    /// <summary>
    /// 业务响应描述
    /// </summary>
    public class MessageModel
    {
        #region 通用
        /// <summary>
        /// 操作成功
        /// </summary>
        public static readonly string Success = "操作成功!";
        /// <summary>
        /// 操作失败
        /// </summary>
        public static readonly string Error = "操作失败!";
        /// <summary>
        /// 系统异常
        /// </summary>
        public static readonly string Fatal = "系统异常!";
        /// <summary>
        /// 警告
        /// </summary>
        public static readonly string Warn = "警告!";
        #endregion

        #region 登录
        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        public static readonly string IncorrectLogin = "用户名或密码错误!";
        /// <summary>
        /// 未授权
        /// </summary>
        public static readonly string Unauthorized = "未授权!";
        /// <summary>
        /// 无效令牌
        /// </summary>
        public static readonly string InvalidToken = "无效令牌!";
        /// <summary>
        /// 令牌过期
        /// </summary>
        public static readonly string ExpirationToken = "过期令牌!";
        /// <summary>
        /// 没有权限操作
        /// </summary>
        public static readonly string LimitedAuthority = "没有权限操作!";
        /// <summary>
        /// 无效身份
        /// </summary>
        public static readonly string InvalidIdentity = "无效身份!";
        /// <summary>
        /// 其他设备登录
        /// </summary>
        public static readonly string OtherDeviceLogin = "已在其他设备登录!";
        #endregion
    }
}
