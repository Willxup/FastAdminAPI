using System;
using System.Security.Cryptography;

namespace FastAdminAPI.Network.QyWechat.Common
{
#pragma warning disable SYSLIB0021
    public static class QyWechatUtilHelper
    {
        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <returns>时间戳</returns>
        public static long GetTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }
        /// <summary>
        /// 生成随机串，随机串包含字母或数字
        /// </summary>
        /// <returns>随机串</returns>
        public static string GetNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        /// <summary>
        /// 使用SHA1哈希加密算法生成签名
        /// </summary>
        /// <param name="rawstring">待处理的字符串</param>
        /// <returns></returns>
        public static string GetSignature(string rawstring)
        {
            //return FormsAuthentication.HashPasswordForStoringInConfigFile(s, "SHA1").ToString().ToLower();
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = System.Text.Encoding.Default.GetBytes(rawstring);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string signature = BitConverter.ToString(bytes_sha1_out);
            signature = signature.Replace("-", "").ToLower();
            return signature;
        }
    }
}
