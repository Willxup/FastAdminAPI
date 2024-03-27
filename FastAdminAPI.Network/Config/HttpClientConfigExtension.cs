using FastAdminAPI.Network.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace FastAdminAPI.Network.Config
{
    public static class HttpClientConfigExtension
    {
        /// <summary>
        /// HttpClient配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientConfig(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<HttpClientTool>();

            return services;
        }
    }
}
