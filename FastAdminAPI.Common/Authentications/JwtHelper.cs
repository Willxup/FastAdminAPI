using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Datetime;
using FastAdminAPI.Common.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastAdminAPI.Common.Authentications
{
    public class JwtHelper
    {
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="token"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string IssueJwt(JwtTokenModel token, IConfiguration configuration)
        {
            try
            {
                // 1. 这里将用户的部分信息，比如 uid 存到了Claim 中，如果你想知道如何在其他地方将这个 UserId 从 Token 中取出来，请看下边的SerializeJwt()方法的调用。
                // 2. 也可以研究下 HttpContext.User.Claims
                List<Claim> claims = new()
                {
                    new("UserId", token.UserId.ToString()),
                    new("Account", token.Account),
                    new("EmployeeId", token.EmployeeId.ToString()),
                    new("EmployeeName", token.EmployeeName),
                    new("Avatar", token.Avatar?? string.Empty),
                    new("Device", token.Device.ToString()),

                    // 过期时间可自定义，注意JWT有自己的缓冲过期时间
                    new("Expires", DateTime.Now.AddSeconds(configuration.GetValue<int>("Redis.LoginPermit.Expires"))
                        .ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo)),
               };

                // 秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
                SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(Define.TOKEN_SECRET));

                // 证书
                SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

                // 生成令牌
                JwtSecurityToken jwt = new(
                    issuer: Define.ISSUER,
                    audience: Define.AUDIENCE,
                    claims: claims,
                    signingCredentials: creds,
                    expires: DateTime.Now.AddSeconds(DateTool.GetRemainingTimeOfDay())); // 获取当天剩余的时间，token最终有效期到今晚24时

                // 令牌处理器
                JwtSecurityTokenHandler jwtHandler = new();

                // 生成token
                return jwtHandler.WriteToken(jwt);

            }
            catch (Exception ex)
            {
                NLogHelper.Error($"生成Token失败，{ex.Message}", ex);
                throw new UserOperationException("生成Token失败!");
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static JwtTokenModel SerializeJwt(string jwtStr)
        {
            try
            {
                // 令牌处理器
                JwtSecurityTokenHandler jwtHandler = new();

                // 读取token信息
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

                jwtToken.Payload.TryGetValue("UserId", out object userId);
                jwtToken.Payload.TryGetValue("Account", out object account);
                jwtToken.Payload.TryGetValue("EmployeeId", out object employeeId);
                jwtToken.Payload.TryGetValue("EmployeeName", out object employeeName);
                jwtToken.Payload.TryGetValue("Avatar", out object avatar);

                jwtToken.Payload.TryGetValue("Device", out object device);
                jwtToken.Payload.TryGetValue("Expires", out object expires);

                JwtTokenModel jwt = new()
                {
                    UserId = userId != null ? Convert.ToInt64(userId) : -1,
                    Account = account != null ? account.ToString() : string.Empty,
                    EmployeeId = employeeId != null ? Convert.ToInt64(employeeId) : -1,
                    EmployeeName = employeeName != null ? employeeName.ToString() : string.Empty,
                    Avatar = avatar != null ? avatar.ToString() : string.Empty,

                    Device = device != null ? Convert.ToInt32(device) : -1,
                    Expires = Convert.ToDateTime(expires),
                };

                return jwt;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"解析Token失败，{ex.Message}", ex);
                throw new UserOperationException("解析Token失败!");
            }

        }

        /// <summary>
        /// 校验JWTToken
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static bool ValidateJwtToken(string jwtStr)
        {
            try
            {
                // 令牌处理器
                JwtSecurityTokenHandler jwtHandler = new();

                // 校验token
                jwtHandler.ValidateToken(jwtStr, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Define.TOKEN_SECRET)),
                    ValidIssuer = Define.ISSUER,
                    ValidateIssuer = true,
                    ValidAudience = Define.AUDIENCE,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.FromSeconds(30), //默认300
                }, out _);

                return true;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"校验JWTToken失败，{ex.Message}", ex);
                return false;
            }
        }
    }
}