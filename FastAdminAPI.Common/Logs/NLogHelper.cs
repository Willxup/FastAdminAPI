using System;
using FastAdminAPI.Common.Datetime;
using NLog;

namespace FastAdminAPI.Common.Logs
{
    /// <summary>
    /// 日志操作
    /// </summary>
    public class NLogHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #region 内部方法
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="throwMsg"></param>
        /// <param name="ex"></param>
        private static void ErrorLog(string throwMsg, Exception ex = null)
        {
            string errorMsg;

            if (ex == null)
                errorMsg = string.Format("\r\n【描述】：{0} \r\n", new object[] { throwMsg });
            else
                errorMsg = string.Format("\r\n【描述】：{0}\r\n【堆栈】：{1} \r\n", new object[] { throwMsg, ex.StackTrace.TrimStart() });

            logger.Error(errorMsg);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="operateMsg"></param>
        private static void DebugLog(string operateMsg)
        {
            string Msg = string.Format("\r\n【描述】：{0} \r\n", new object[] { operateMsg });

            logger.Debug(Msg);
        } 
        #endregion

        /// <summary>
        /// 调试信息日志(时间前缀)
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message)
        {
            DebugLog($"{DateTime.Now.ToFormattedString()} - {message}");
        }
        /// <summary>
        /// 错误信息日志(时间前缀)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Error(string message, Exception ex = null)
        {
            ErrorLog($"{DateTime.Now.ToFormattedString()} - {message}", ex);
        }
    }
}
