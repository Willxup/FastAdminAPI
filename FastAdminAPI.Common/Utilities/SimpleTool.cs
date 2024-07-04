using System.Text.RegularExpressions;

namespace FastAdminAPI.Common.Utilities
{
    /// <summary>
    /// 简易工具
    /// </summary>
    public static class SimpleTool
    {
        /// <summary>
        /// 字符串过滤
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string StringFormatFilter(string str)
        {
            return str?.Replace("\n", "")?
                        .Replace(" ", "")?
                        .Replace("\t", "")?
                        .Replace("\r", "");
        }
        /// <summary>
        /// 校验是否为纯数字字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool IsStringNumber(string str)
        {
            if (Regex.IsMatch(str, @"^\d+$"))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 校验是否为手机号
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="isStrict">是否严格校验(默认否)</param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string phone, bool isStrict = false)
        {
            //手机号位数校验
            if (phone.Length != 11)
            {
                return false;
            }

            //手机号校验    
            if (!Regex.IsMatch(phone, @"^1\d{10}$"))
            {
                return false;
            }

            //严格校验
            if (isStrict)
            {
                if (!Regex.IsMatch(phone, @"^1(3\d|4[5-9]|5[0-35-9]|6[2567]|7[0-8]|8\d|9[0-35-9])\d{8}$"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
