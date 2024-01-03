using System;
using System.Globalization;

namespace FastAdminAPI.Common.Converters
{
    public static class DateTimeConvertExtension
    {
        /// <summary>
        /// 时间类型转格式化后时间字符串
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormattedString(this DateTime? datetime, string format = "yyyy-MM-dd HH:mm:ss", DateTime? defaultValue = null)
        {
            if(datetime != null)
            {
                return ((DateTime)datetime).ToFormattedString(format);
            }
            return defaultValue != null ? ((DateTime)defaultValue).ToFormattedString(format) : DateTime.MinValue.ToFormattedString(format);
        }
        /// <summary>
        /// 时间类型转格式化后时间字符串
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
