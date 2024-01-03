using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.QyWechat.Common;
using FastAdminAPI.Network.QyWechat.Config;
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
        /// Refit配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddRefitClients(this IServiceCollection services, IConfiguration configuration)
        {
            //Refit设置
            RefitSettings refitSettings = new()
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                }),
                //ExceptionFactory = (response) => throw new UserOperationException($"code:{response.StatusCode}, message:{response.RequestMessage}")
            };

            // QyWechat
            services.AddRefitClient<IQyWechatProvider>(refitSettings)
                    .ConfigureHttpClient(c =>
                    {
                        c.BaseAddress = new Uri(BaseQyWechatConfiguration.BaseUrl);
                        c.Timeout = TimeSpan.FromSeconds(60);
                    });

            // Core
            services.AddRefitClient<IQyWechatApi>(refitSettings)
                    .ConfigureHttpClient(c =>
                    {
                        c.BaseAddress = new Uri(configuration.GetValue<string>("FastAdminAPI.Core.Url")); //本地启动
                        c.Timeout = TimeSpan.FromSeconds(60);
                    });

            return services;
        }
    }
}
