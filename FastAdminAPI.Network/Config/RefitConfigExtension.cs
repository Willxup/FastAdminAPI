using FastAdminAPI.Network.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using System;

namespace FastAdminAPI.Network.Config
{
    public static class RefitConfigExtension
    {
        /// <summary>
        /// Refit配置类
        /// </summary>
        public static readonly RefitSettings REFIT_SETTINGS = new()
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver() //使用newtonsoft.json
            }),
            //ExceptionFactory = (response) => throw new UserOperationException($"code:{response.StatusCode}, message:{response.RequestMessage}")
        };

        /// <summary>
        /// Refit配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddRefitClients(this IServiceCollection services, IConfiguration configuration)
        {
            // qywechat
            services.AddRefitClient<IQyWechatApi>(REFIT_SETTINGS)
                    .ConfigureHttpClient(c =>
                    {
                        c.BaseAddress = new Uri(configuration.GetValue<string>("FastAdminAPI.Core.Url")); //appsettings配置
                        c.Timeout = TimeSpan.FromSeconds(60);
                    });

            // email
            services.AddRefitClient<IEmailApi>(REFIT_SETTINGS)
                    .ConfigureHttpClient(c =>
                    {
                        c.BaseAddress = new Uri(configuration.GetValue<string>("FastAdminAPI.Core.Url")); //appsettings配置
                        c.Timeout = TimeSpan.FromSeconds(60);
                    });

            // wechat
            services.AddRefitClient<IWechatApi>(REFIT_SETTINGS)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration.GetValue<string>("FastAdminAPI.Core.Url")); //appsettings配置
                    c.Timeout = TimeSpan.FromSeconds(60);
                });

            return services;
        }
    }
}
