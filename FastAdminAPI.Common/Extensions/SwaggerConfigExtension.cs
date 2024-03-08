using FastAdminAPI.Common.BASE;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using static FastAdminAPI.Common.Swagger.CustomApiVersion;

namespace FastAdminAPI.Common.Extensions
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
            services.AddSwaggerGen(c =>
            {
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerDoc(version, new OpenApiInfo
                    {
                        // {ApiName} 定义成全局变量
                        Title = $"{serviceName} 接口文档",
                        Description = $"{serviceName} " + version,
                        //TermsOfService = "None",
                        Contact = new OpenApiContact { Name = serviceName, Email = "fastadminapi@willxup.top" }
                    });
                    // 按相对路径排序
                    //c.OrderActionsBy(o => o.RelativePath);
                });
                //解决相同类名会报错的问题
                c.CustomSchemaIds(type => type.FullName);

                //配置的xml文件名
                var xmlPath = Path.Combine(basePath, $"{serviceName}.xml");
                c.IncludeXmlComments(xmlPath, true);

                #region Token绑定
                //添加header验证信息
                //c.OperationFilter<SwaggerHeader>();

                //发行人
                var issuerName = Define.ISSUER;
                var security = new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = issuerName},
                            //Name = issuerName
                        }, Array.Empty<string>()
                    }
                };
                c.AddSecurityRequirement(security);

                c.AddSecurityDefinition(issuerName, new OpenApiSecurityScheme
                {
                    Description = "请在下框中输入Bearer {token}",
                    Name = "Authorization", //jwt默认的参数名称
                    In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                #endregion
            });

            services.AddSwaggerGenNewtonsoftSupport(); //使swagger支持newtonsoft.json

            return services;
        }
    }
}
