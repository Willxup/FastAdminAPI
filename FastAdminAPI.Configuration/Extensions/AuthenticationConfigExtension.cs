using System;
using System.Threading.Tasks;
using FastAdminAPI.Configuration.Authentications;
using FastAdminAPI.Configuration.BASE;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FastAdminAPI.Configuration.Extensions
{
    /// <summary>
    /// 身份认证配置
    /// </summary>
    public static class AuthenticationConfigExtension
    {
        /// <summary>
        /// JWT校验配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册时校验，缺少有效密钥时停止启动
            SymmetricSecurityKey signingKey = JwtHelper.GetSigningKey(configuration);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidIssuer = Define.ISSUER,
                    ValidateIssuer = true,
                    ValidAudience = Define.AUDIENCE,
                    ValidateAudience = true,

                    //ValidateLifetime = true, //默认true
                    ClockSkew = TimeSpan.FromSeconds(30), //默认300
                    //RequireExpirationTime = true,  //默认true
                };
                option.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // 如果过期，则把<是否过期>添加到，返回头信息中
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }
    }
}
