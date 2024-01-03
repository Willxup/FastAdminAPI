using System;
using System.Security.Cryptography;
using System.Text;

namespace FastAdminAPI.Common.Cryptions
{
    // Disable the warning.
    #pragma warning disable SYSLIB0021
    #pragma warning disable SYSLIB0022
    public static class AESEncryptHelper
    {
        /// <summary>
        /// AES加密(256)
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <param name="cryptoKey">密钥</param>
        /// <returns></returns>
        public static string RijndaelEncrypt(string encryptStr, string cryptoKey)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(encryptStr);
            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// AES解密(256)
        /// </summary>
        /// <param name="decryptStr">密文</param>
        /// <param name="cryptoKey">密钥</param>
        /// <returns></returns>
        public static string RijndaelDecrypt(string decryptStr, string cryptoKey)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(cryptoKey);
            byte[] toEncryptArray = Convert.FromBase64String(decryptStr);
            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <param name="cryptoKey">密钥</param>
        /// <returns></returns>
        public static string AesEncrypt(string encryptStr, string cryptoKey)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = Encoding.UTF8.GetBytes(cryptoKey);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    byte[] inputBuffers = Encoding.UTF8.GetBytes(encryptStr);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    aesProvider.Dispose();
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">密文</param>
        /// <param name="cryptoKey">密钥</param>
        /// <returns></returns>
        public static string AesDecrypt(string decryptStr, string cryptoKey)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = Encoding.UTF8.GetBytes(cryptoKey);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                {
                    byte[] inputBuffers = Convert.FromBase64String(decryptStr);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    return Encoding.UTF8.GetString(results);
                }
            }
        }

        /// <summary>
        /// 补充小于32位的AES密钥
        /// </summary>
        /// <param name="cryptoKey"></param>
        /// <returns></returns>
        public static string PaddingCryptoKey(string cryptoKey)
        {
            string newKey = cryptoKey;
            if (!string.IsNullOrEmpty(cryptoKey) && cryptoKey.Length <= 32)
            {
                if (cryptoKey.Length < 32)
                    newKey = cryptoKey.PadRight(32, '0');
            }
            return newKey;
        }

    }
}
