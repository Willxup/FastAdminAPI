using System;

namespace FastAdminAPI.Common.Attributes
{
    /// <summary>
    /// 用户操作异常
    /// </summary>
    public class UserOperationException : Exception
    {
        /// <summary>
        /// 构造
        /// </summary>
        public UserOperationException() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message">错误信息</param>
        public UserOperationException(string message) : base(message) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">内部错误</param>
        public UserOperationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
