using System;
using System.Globalization;

namespace FastAdminAPI.Common.Datetime
{
    public static class DateTimeHelper
    {
        #region DateTime和时间戳互转
        /// <summary>
        /// 时间戳类型
        /// </summary>
        public enum TimeStampType 
        {
            /// <summary>
            /// 秒
            /// </summary>
            Seconds = 1,
            /// <summary>
            /// 毫秒
            /// </summary>
            Milliseconds = 2 
        }
        /// <summary>  
        /// DateTime时间格式转换为时间戳 
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式(默认毫秒)</returns>  
        public static string ConvertDateToTimeStamp(DateTime time, TimeStampType timeStampType = TimeStampType.Milliseconds)
        {
            //DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            DateTime startTime = new DateTime(1970, 1, 1).ToLocalTime();
            var timeSpan = time - startTime;
            double timeStamp = 0;
            switch (timeStampType)
            {
                case TimeStampType.Seconds:
                    timeStamp = timeSpan.TotalSeconds;
                    break;
                case TimeStampType.Milliseconds:
                    timeStamp = timeSpan.TotalMilliseconds;
                    break;
            }
            return timeStamp.ToString();
        }
        /// <summary>
        /// DateTime时间格式转换为时间戳 
        /// </summary>
        /// <param name="date">DateTime时间格式</param>
        /// <returns></returns>
        public static double ConvertDateToTimeStamp(DateTime date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="timeStampType">时间戳类型(默认毫秒)</param>
        /// <returns></returns>
        public static DateTime? ConvertTimeStampToDate(string timestamp, TimeStampType timeStampType = TimeStampType.Milliseconds)
        {
            DateTime startTime = new(1970, 1, 1, 0, 0, 0, 0);
            DateTime? time = null;
            try
            {
                double stamp = Convert.ToDouble(timestamp);
                if (stamp >= 0)
                {
                    switch (timeStampType)
                    {
                        case TimeStampType.Seconds:
                            time = startTime.AddSeconds(stamp).ToLocalTime();
                            break;
                        case TimeStampType.Milliseconds:
                            time = startTime.AddMilliseconds(stamp).ToLocalTime();
                            break;
                    }
                }
            }
            catch (Exception)
            {
                //返回null
            }
            return time;
        }
        #endregion

        #region 剩余时间
        /// <summary>
        /// 获取当天剩余时间(秒)
        /// </summary>
        /// <returns>剩余时间总秒数</returns>
        public static double GetRemainingTimeOfDay()
        {
            return (24 * 60 * 60) - DateTime.Now.TimeOfDay.TotalSeconds;
        } 
        #endregion
    }
    public static class DateTimeConvertExtension
    {
        /// <summary>
        /// 可空时间类型格式化为字符串
        /// </summary>
        /// <param name="datetime">时间</param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormattedString(this DateTime? datetime, string format = "yyyy-MM-dd HH:mm:ss", DateTime? defaultValue = null)
        {
            if (datetime != null)
            {
                return ((DateTime)datetime).ToFormattedString(format);
            }
            return defaultValue != null ? ((DateTime)defaultValue).ToFormattedString(format) : DateTime.MinValue.ToFormattedString(format);
        }
        /// <summary>
        /// 时间类型格式化为字符串
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormattedString(this DateTime datetime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return datetime.ToString(format, DateTimeFormatInfo.InvariantInfo);
        }
    }
}
