using Autofac.Extensions.DependencyInjection;
using FastAdminAPI.Business.Extensions;
using FastAdminAPI.CAP.Extensions;
using FastAdminAPI.Common.Attributes.CheckValidators;
using FastAdminAPI.Common.Extensions;
using FastAdminAPI.Common.Filters;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Middlewares;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Common.Swagger;
using FastAdminAPI.Common.Utilities;
using FastAdminAPI.Framework.Extensions;
using FastAdminAPI.Network.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using SqlSugar;
using System;
using System.IO;

try
{
    //builder
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    //获取配置
    IConfiguration configuration = builder.Configuration;
    //服务名称
    string serviceName = "FastAdminAPI.Core";

    #region Nlog
    // Setup NLog for Dependency injection
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    builder.Host.UseNLog();
    #endregion

    #region 载入配置文件
    builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        //设置appsetting.json
        config.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{EnvTool.GetEnv()}.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
    });
    #endregion

    #region 服务 Services

    #region 服务注入
    builder.Services.AddOptions();

    // Add services to the container.
    builder.Services.AddControllers(c =>
        {
            c.Filters.Add<ModelValidationAttribute>();
            c.Filters.Add(typeof(GlobalExceptionsFilter));
        })
        .AddControllersAsServices()
        .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })// 取消默认驼峰
        .AddNewtonsoftJson(options => //Newtonsoft.Json
        { 
            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";  
        }); 

    //健康检查
    builder.Services.AddHealthChecks();

    // HttpContext
    builder.Services.AddHttpContextAccessor();

    // Redis
    builder.Services.AddSingleton<IRedisHelper, RedisHelper>();

    // HttpClient & Helper & Refit
    builder.Services.AddHttpClientConfig();
    builder.Services.AddRefitClients(configuration);

    // 服务层注入
    builder.Services.AddAllServices();

    //业务服务层
    builder.Services.AddBusinessServices();
    #endregion

    #region SqlSugar ORM框架
    builder.Services.AddSingleton<ISqlSugarClient>(sugar =>
    {
        return DbExtension.ConfigSqlSugar(configuration.GetValue<string>("Database.ConnectionString"));
    });
    #endregion

    #region 缓存
    builder.Services.AddMemoryCache();
    #endregion

    #region  响应结果压缩
    builder.Services.AddCompressResponse();
    #endregion

    #region 统一认证
    builder.Services.AddJwtAuthentication();
    #endregion

    #region swagger文档配置
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwagger(serviceName, AppContext.BaseDirectory);
    #endregion

    #region 事件总线
    builder.Services.AddEventBus(configuration);
    #endregion

    #endregion

    WebApplication app = builder.Build();

    #region 配置 Configure

    #region Swagger文档
    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.Configure(serviceName); });
    #endregion

    #region 压缩
    app.UseResponseCompression();
    #endregion

    #region 请求响应日志
    app.UseRequestResponseLog();
    #endregion

    #region 校验
    app.UseAuthentication();
    app.UseJwtTokenAuth();//Token校验
    #endregion

    #region Map
    app.MapControllers();
    app.MapHealthChecks("/api/healthcheck");
    #endregion

    #endregion

    #region 例子：获取IOC容器注入的对象
    ////1.refit
    //var email1Api = RestService.For<IEmailApi>("http://localhost:9000", RefitConfigExtension.REFIT_SETTINGS);

    //var result1 = email1Api.SendEmailByDefault("test1", "test refit");

    //result1.Wait();

    //Console.WriteLine(email1Api.GetHashCode());

    ////2.serviceProvider
    //var email2Api = app.Services.GetService<IEmailApi>();

    //var result2 = email2Api.SendEmailByDefault("test2", "test refit");

    //result2.Wait();

    //Console.WriteLine(email2Api.GetHashCode());

    ////3.autofac
    //var autofac = app.Services.GetAutofacRoot();

    //var scope = autofac.BeginLifetimeScope();

    //var email3Api = scope.Resolve<IEmailApi>();

    //var result3 = email3Api.SendEmailByDefault("test3", "test refit");

    //result3.Wait();

    //Console.WriteLine(email3Api.GetHashCode());

    //scope.Dispose();
    #endregion

    app.Run();

}
catch (Exception ex)
{

    NLogHelper.Error("启动项目失败!", ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}