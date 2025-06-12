using System.Linq;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;
using static FastAdminAPI.Configuration.Swagger.CustomApiVersion;

namespace FastAdminAPI.Configuration.Swagger
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

            // 接口请求响应展示 模型 还是 Json例子
            options.DefaultModelRendering(ModelRendering.Model);

            // 接口请求响应展示 模型展开深度
            options.DefaultModelExpandDepth(2);

            // schema展示 展开深度 -1表示隐藏schema
            options.DefaultModelsExpandDepth(-1);

            // 是否展示操作Id(接口的唯一Id，防止出现相同方法和路径而无法确认接口实现)
            options.DisplayOperationId();

            // 展示请求响应时间
            options.DisplayRequestDuration();
            
            // Swagger文档是否展开
            options.DocExpansion(DocExpansion.None);

            // 是否展示扩展
            options.ShowExtensions();
        }
    }
}
