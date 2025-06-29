﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Configuration.Authentications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace FastAdminAPI.Configuration.Middlewares
{
    /// <summary>
    /// Token认证
    /// </summary>
    public class JwtTokenAuthMiddleware
    {
        /// <summary>
        /// 委托
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 配置获取
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;
        /// <summary>
        /// 登录许可Key
        /// </summary>
        private readonly string LOGIN_PERMIT_KEY;
        /// <summary>
        /// 登录许可有效期
        /// </summary>
        private readonly int LOGIN_PERMIT_EXPIRES;
        /// <summary>
        /// 登录许可差异，用于判断是否更新登录许可有效期
        /// </summary>
        private readonly int LOGIN_PERMIT_EXPIRES_DIFF;
        /// <summary>
        /// 路径过滤器
        /// 字符全部小写
        /// </summary>
        private readonly string[] WHITE_LIST;


        /// <summary>
        /// 中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="redis"></param>
        public JwtTokenAuthMiddleware(RequestDelegate next, IConfiguration configuration, IRedisHelper redis)
        {
            _next = next;
            _configuration = configuration;
            _redis = redis;
            LOGIN_PERMIT_KEY = configuration.GetValue<string>("Redis.LoginPermit.Key");
            LOGIN_PERMIT_EXPIRES = configuration.GetValue<int>("Redis.LoginPermit.Expires");
            LOGIN_PERMIT_EXPIRES_DIFF = configuration.GetValue<int>("Redis.LoginPermit.ExpiresDiff");
            WHITE_LIST = _configuration.GetValue<string>("RequestFilter.WhiteList")?.ToLower()?.Split(",");
        }

        /// <summary>
        /// Token认证
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            #region 白名单
            // 健康检查
            if (httpContext.Request.Path.Value.ToLower().StartsWith("/api/healthcheck"))
            {
                await _next(httpContext);
                return;
            }

            // 无需token校验
            if (httpContext.Request.Path.Value.ToLower().StartsWith("/api/tokenpass"))
            {
                await _next(httpContext);
                return;
            }

            // 白名单
            if (WHITE_LIST?.Contains(httpContext.Request.Path.Value.ToLower()) ?? false)
            {
                await _next(httpContext);
                return;
            }

            // SignalR hub(websocket) 校验放在hub及UseAuthentication中
            if (httpContext.Request.Path.Value.ToLower().StartsWith("/hubs"))
            {
                await _next(httpContext);
                return;
            }
            #endregion

            #region 静态文件访问
            // 静态文件访问
            if (httpContext.Request.Path.Value.ToLower().StartsWith("/oss"))
            {
                // 提取链接上的token
                if (httpContext.Request.Query.TryGetValue("token", out StringValues value))
                {
                    // token
                    string tokenByQuery = value.ToString();

                    // token长度不能小于128
                    if (tokenByQuery.Length < 128)
                    {
                        await FailAuth(httpContext, GetInvalidTokenResponse());
                        return;
                    }

                    // 校验是否为系统颁发的令牌
                    if (!JwtHelper.ValidateJwtToken(tokenByQuery))
                    {
                        await FailAuth(httpContext, GetInvalidTokenResponse());
                        return;
                    }

                    JwtTokenModel jwtByQuery;
                    try
                    {
                        // 解析JWT Token
                        jwtByQuery = JwtHelper.SerializeJwt(tokenByQuery);
                    }
                    catch (Exception)
                    {
                        await FailAuth(httpContext, GetInvalidTokenResponse());
                        return;
                    }

                    // 滑动过期时间下，令牌中的过期时间超过8小时，需要重新登录
                    // 这里存在token里的过期时间可以看JwtHelper中的设置，两者配合处理token在滑动过期下的最终过期时间
                    if (DateTime.Now.Subtract(jwtByQuery.Expires).Hours > 8)
                    {
                        await FailAuth(httpContext, GetExpirationTokenResponse());
                        return;
                    }

                    // 登录许可的redis key
                    string permitKey1 = LOGIN_PERMIT_KEY + jwtByQuery.UserId + ":" + jwtByQuery.Device;

                    // 获取登录许可(token)
                    string tokenByRedis1 = await _redis.StringGetAsync(permitKey1);

                    // 如果许可已不存在于Redis中，说明令牌过期
                    if (string.IsNullOrEmpty(tokenByRedis1))
                    {
                        await FailAuth(httpContext, GetExpirationTokenResponse());
                        return;
                    }

                    // 如果登录许可(token)与传入的token不一致，说明在其他设备登录
                    if (tokenByRedis1 != tokenByQuery)
                    {
                        await FailAuth(httpContext, GetOtherDeviceLoginResponse());
                        return;
                    }
                }
                else
                {
                    await FailAuth(httpContext, GetUnAuthorizedResponse());
                    return;
                }

                await _next(httpContext);
                return;
            }
            #endregion

            #region 接口请求访问
            // 是否包含Authorization请求头
            if (string.IsNullOrEmpty(httpContext.Request.Headers.Authorization))
            {
                await FailAuth(httpContext, GetUnAuthorizedResponse());
                return;
            }

            // 获取header中的JWTToken
            var token = httpContext.Request.Headers.Authorization.ToString();

            // 校验是否有Bearer前缀
            if (!token.Contains("Bearer "))
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }
            else
            {
                token = token.Replace("Bearer ", ""); // 将Bearer去除
            }

            // token长度不能小于128
            if (token.Length < 128)
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }

            // 校验是否为系统颁发的令牌
            if (!JwtHelper.ValidateJwtToken(token))
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }

            // 解析JWT Token
            JwtTokenModel jwt;

            try
            {
                jwt = JwtHelper.SerializeJwt(token);
            }
            catch (Exception) // 解析token失败
            {
                await FailAuth(httpContext, GetInvalidTokenResponse());
                return;
            }

            // 滑动过期时间下，令牌中的过期时间超过8小时，需要重新登录
            // 这里存在token里的过期时间可以看JwtHelper中的设置，两者配合处理token在滑动过期下的最终过期时间
            if (DateTime.Now.Subtract(jwt.Expires).Hours > 8)
            {
                await FailAuth(httpContext, GetExpirationTokenResponse());
                return;
            }

            // 登录许可的redis key
            string permitKey = LOGIN_PERMIT_KEY + jwt.UserId + ":" + jwt.Device;

            // 如果许可已不存在于Redis中，说明令牌过期
            if (!await _redis.KeyExistsAsync(permitKey))
            {
                await FailAuth(httpContext, GetExpirationTokenResponse());
                return;
            }

            // 获取登录许可(token)
            string tokenByRedis = await _redis.StringGetAsync(permitKey);

            // 如果登录许可(token)与传入的token不一致，说明在其他设备登录
            if (tokenByRedis != token)
            {
                await FailAuth(httpContext, GetOtherDeviceLoginResponse());
                return;
            }

            // 获取登录许可的有效时间
            double expires = (await _redis.KeyTimeToLiveAsync(permitKey)) ?? 0;

            // 如果 [登录许可有效期] 与 [登录有效期] 之间的差 小于等于 [登录许可差异]， 更新登录许可的有效期，以保持滑动过期
            if ((LOGIN_PERMIT_EXPIRES - expires) >= LOGIN_PERMIT_EXPIRES_DIFF)
            {
                await _redis.KeySetExpireAsync(permitKey, TimeSpan.FromSeconds(LOGIN_PERMIT_EXPIRES));
            }

            await _next(httpContext); 
            #endregion
        }

        /// <summary>
        /// 校验失败
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private static async Task FailAuth(HttpContext httpContext, ResponseModel response)
        {
            httpContext.Response.StatusCode = 200;
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        /// <summary>
        /// 获取 未授权 响应
        /// </summary>
        /// <returns></returns>
        private static ResponseModel GetUnAuthorizedResponse()
        {
            ResponseModel response = new()
            {
                Code = ResponseCode.Unauthorized,
                Message = ResponseMessage.Unauthorized
            };
            return response;
        }
        /// <summary>
        /// 获取 过期令牌 响应
        /// </summary>
        /// <returns></returns>
        private static ResponseModel GetExpirationTokenResponse()
        {
            ResponseModel response = new()
            {
                Code = ResponseCode.ExpirationToken,
                Message = ResponseMessage.ExpirationToken
            };
            return response;
        }
        /// <summary>
        /// 获取 无效令牌 响应
        /// </summary>
        /// <returns></returns>
        private static ResponseModel GetInvalidTokenResponse()
        {
            ResponseModel response = new()
            {
                Code = ResponseCode.InvalidToken,
                Message = ResponseMessage.InvalidToken
            };
            return response;
        }
        /// <summary>
        /// 获取 其他设备登录 响应
        /// </summary>
        /// <returns></returns>
        private static ResponseModel GetOtherDeviceLoginResponse()
        {
            ResponseModel response = new()
            {
                Code = ResponseCode.OtherDeviceLogin,
                Message = ResponseMessage.OtherDeviceLogin
            };
            return response;
        }
    }
}