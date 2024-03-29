using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Converters;
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
        /// <param name="tokenModel"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string IssueJwt(JwtTokenModel tokenModel, IConfiguration configuration)
        {
            try
            {
                var claims = new List<Claim>
                {
                     /*
                       1、这里将用户的部分信息，比如 uid 存到了Claim 中，如果你想知道如何在其他地方将这个 uid从 Token 中取出来，请看下边的SerializeJwt()方法的调用。
                       2、你也可以研究下 HttpContext.User.Claims
                     */
                    new("UserId", tokenModel.UserId.ToString()),
                    new("Account", tokenModel.Account),
                    new("EmployeeId", tokenModel.EmployeeId.ToString()),
                    new("EmployeeName", tokenModel.EmployeeName),
                    new("PostIds", tokenModel.PostIds ?? string.Empty),
                    new("Avatar", tokenModel.Avatar?? string.Empty),
                    new("Device", tokenModel.Device.ToString()),

                    // 过期时间可自定义，注意JWT有自己的缓冲过期时间
                    new("Expires", DateTime.Now.AddSeconds(configuration.GetValue<int>("Redis.LoginPermit.Expires"))
                        .ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo)),
               };

                //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Define.TOKEN_SECRET));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwt = new JwtSecurityToken(
                    issuer: Define.ISSUER,
                    audience: Define.AUDIENCE,
                    claims: claims,
                    signingCredentials: creds,
                    expires: DateTime.Now.AddSeconds(DateTimeHelper.GetRemainingTimeOfDay()) //获取当天剩余的时间，token有效期到今晚24时
                    );

                var jwtHandler = new JwtSecurityTokenHandler();
                var encodedJwt = jwtHandler.WriteToken(jwt);

                return encodedJwt;
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
                var jwtHandler = new JwtSecurityTokenHandler();

                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

                jwtToken.Payload.TryGetValue("UserId", out object userId);
                jwtToken.Payload.TryGetValue("Account", out object account);
                jwtToken.Payload.TryGetValue("EmployeeId", out object employeeId);
                jwtToken.Payload.TryGetValue("EmployeeName", out object employeeName);
                jwtToken.Payload.TryGetValue("PostIds", out object postIds);
                jwtToken.Payload.TryGetValue("Avatar", out object avatar);

                jwtToken.Payload.TryGetValue("Device", out object device);
                jwtToken.Payload.TryGetValue("Expires", out object expires);
                var jwt = new JwtTokenModel
                {
                    UserId = userId != null ? Convert.ToInt64(userId) : -1,
                    Account = account != null ? account.ToString() : string.Empty,
                    EmployeeId = employeeId != null ? Convert.ToInt64(employeeId) : -1,
                    EmployeeName = employeeName != null ? employeeName.ToString() : string.Empty,
                    PostIds = postIds != null ? postIds.ToString() : string.Empty,
                    Avatar = avatar != null ? avatar.ToString() : string.Empty,

                    Device = device != null ? Convert.ToInt32(device) : -1,
                    Expires = expires.ToDate(),
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
                JwtSecurityTokenHandler jwtHandler = new();
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