using System;
using FastAdminAPI.Common.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FastAdminAPI.Common.Cryptos
{
    public static class UserPasswordCryptoTool
    {
        private const string HASH_PREFIX = "{PBKDF2}";
        private static readonly PasswordHasher<object> _passwordHasher = new(
            Options.Create(new PasswordHasherOptions { IterationCount = 600000 }));

        /// <summary>
        /// 生成用户密码哈希，盐由框架生成并保存在哈希中
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new UserOperationException("密码不能为空!");

            return HASH_PREFIX + _passwordHasher.HashPassword(null, password);
        }

        /// <summary>
        /// 是否为已升级的密码格式
        /// </summary>
        public static bool IsHashedPassword(string storedPassword)
        {
            return storedPassword?.StartsWith(HASH_PREFIX, StringComparison.Ordinal) ?? false;
        }

        /// <summary>
        /// 校验密码，兼容未升级的旧值；哈希验证失败时不回退比较旧值
        /// </summary>
        public static bool VerifyPassword(string password, string storedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedPassword))
                return false;

            if (!IsHashedPassword(storedPassword))
                return string.Equals(password, storedPassword, StringComparison.Ordinal);

            try
            {
                return _passwordHasher.VerifyHashedPassword(null,
                    storedPassword.Substring(HASH_PREFIX.Length), password) != PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
