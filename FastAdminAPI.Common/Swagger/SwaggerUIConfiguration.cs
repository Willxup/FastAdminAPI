using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Linq;
using static FastAdminAPI.Common.Swagger.CustomApiVersion;

namespace FastAdminAPI.Common.Swagger
{
    /// <summary>
    /// SwaggerUI配置
    /// </summary>
    public static class SwaggerUIConfiguration
    {
        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="options"></param>
        /// <param name="serviceName">服务名称</param>
        public static void Configure(this SwaggerUIOptions options, string serviceName)
        {

            // 根据版本名称正序 遍历展示
            typeof(ApiVersions).GetEnumNames().OrderBy(e => e).ToList().ForEach(version =>
            {
                options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{serviceName} {version}");
            });

            // 将swagger首页,设置成我们自定义的页面:解决方案名+.index.html               
            // 路径配置，设置为空，表示直接在根域名（localhost:9000）访问该文件,注意localhost:9000/swagger是访问不到的，去launchSettings.json把launchUrl去掉
            options.RoutePrefix = "";

            // Display
            options.DefaultModelExpandDepth(2);
            options.DefaultModelRendering(ModelRendering.Model);
            options.DefaultModelsExpandDepth(-1);
            options.DisplayOperationId();
            options.DisplayRequestDuration();
            options.DocExpansion(DocExpansion.None);
            options.ShowExtensions();
        }
    }
}
