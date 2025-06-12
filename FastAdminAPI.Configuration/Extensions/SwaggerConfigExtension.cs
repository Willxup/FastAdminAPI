using System;
using System.IO;
using System.Linq;
using FastAdminAPI.Configuration.BASE;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using static FastAdminAPI.Configuration.Swagger.CustomApiVersion;

namespace FastAdminAPI.Configuration.Extensions
{
    /// <summary>
    /// Swagger配置
    /// </summary>
    public static class SwaggerConfigExtension
    {
        /// <summary>
        /// 配置Swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceName">服务名称</param>
        /// <param name="basePath">文档位置</param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, string serviceName, string basePath)
        {
            services.AddSwaggerGen(options =>
            {
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    options.SwaggerDoc(version, new OpenApiInfo
                    {
                        // {ApiName} 定义成全局变量
                        Title = $"{serviceName} 接口文档",
                        Description = $"{serviceName} " + version,
                        // TermsOfService = "None",
                        Contact = new OpenApiContact { Name = serviceName, Email = "fastadminapi@willxup.top" }
                    });
                    // 按相对路径排序
                    //c.OrderActionsBy(o => o.RelativePath);
                });
                // 解决相同类名会报错的问题
                options.CustomSchemaIds(type => type.FullName);

                // 配置的xml文件名
                var xmlPath = Path.Combine(basePath, $"{serviceName}.xml");
                // 将代码中的XML注释添加到swagger中，补充swagger文档细节
                options.IncludeXmlComments(xmlPath, true);

                #region Token绑定
                // 添加header验证信息
                // options.OperationFilter<SwaggerHeader>();

                // 设置一个安全定义
                options.AddSecurityDefinition(Define.ISSUER, new OpenApiSecurityScheme
                {
                    Description = "请在下框中输入Bearer {token}",
                    Name = "Authorization", // jwt默认的参数名称
                    In = ParameterLocation.Header, // jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                // 添加安全需求
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            //Name = issuerName
                            Reference = new OpenApiReference
                            {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = Define.ISSUER // 这个Id需要对应安全定义
                            },

                        }, Array.Empty<string>()
                    }
                });
                #endregion
            });

            // 使swagger支持newtonsoft.json
            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }
    }
}
