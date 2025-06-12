using System;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Logs;

namespace FastAdminAPI.Common.Cryptos
{
    public static class UserPasswordCryptoTool
    {
        /// <summary>
        /// 密码密钥
        /// </summary>

        public const string PASSWORD_CRYPTO_KEY = "fastadminapiwxdaelzekf884knywill";

        /// <summary>
        /// 加密用户密码
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string EncryptPassword(string plaintext)
        {
            try
            {
                return CryptoTool.RijndaelEncrypt(plaintext, PASSWORD_CRYPTO_KEY);
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"用户密码加密失败，原因：{ex.Message}", ex);
                throw new UserOperationException("数据异常，请稍后再试!");
            }

        }
        /// <summary>
        /// 解密用户密码
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static string DecryptPassword(string ciphertext)
        {
            try
            {
                return CryptoTool.RijndaelDecrypt(ciphertext, PASSWORD_CRYPTO_KEY);
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"用户密码解密失败，原因：{ex.Message}", ex);
                throw new UserOperationException("数据异常，请稍后再试!");
            }
        }
    }
}
