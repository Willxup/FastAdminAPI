using System;
using System.Collections.Generic;
using System.Text;

namespace FastAdminAPI.Common.Converters
{
    public static class GuidConverter
    {
        /// <summary>
        /// 将一个标准的GUID转换成短的字符串如：3d4ebc5f5f2c4ede
        /// </summary>
        /// <returns></returns>
        public static string GenerateShortGuid()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= b + 1;
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        /// <summary>
        /// 获得一个19位长的序列,8346734568923542345
        /// </summary>
        /// <returns></returns>
        public static string GenerateIntegerGuid()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0).ToString();
        }
    }
}
