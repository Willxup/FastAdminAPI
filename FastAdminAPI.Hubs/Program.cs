
using Autofac.Extensions.DependencyInjection;
using FastAdminAPI.Common.Attributes.CheckValidators;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Common.Utilities;
using FastAdminAPI.Configuration.Extensions;
using FastAdminAPI.Configuration.Filters;
using FastAdminAPI.Configuration.Middlewares;
using FastAdminAPI.Hubs.Hubs;
using FastAdminAPI.Hubs.Hubs.BASE;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using System;
using System.IO;

try
{
    //builder
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    // 获取配置
    IConfiguration configuration = builder.Configuration;
    // 服务名称
    //string serviceName = "FastAdminAPI.Hubs";

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

    // 业务服务注入
    builder.Services.AddBusinessServices();

    // Hub定时任务
    builder.Services.AddHostedService<HubSchedule>();
    #endregion

    #region SignalR
    builder.Services.AddSignalR()
        .AddJsonProtocol(options => { options.PayloadSerializerOptions.PropertyNamingPolicy = null; })
        .AddNewtonsoftJsonProtocol(options => { options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver(); });
    #endregion

    #region  响应结果压缩
    builder.Services.AddCompressResponse();
    #endregion

    #region 统一认证
    builder.Services.AddJwtAuthentication();
    #endregion

    #endregion

    WebApplication app = builder.Build();

    #region 压缩
    app.UseResponseCompression();
    #endregion

    #region 请求响应日志
    app.UseRequestResponseLog();
    #endregion

    #region 跨域
    app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(o => true).AllowCredentials());
    #endregion

    #region 校验
    //app.UseAuthentication();
    //app.UseJwtTokenAuth(); // Token校验
    #endregion

    #region Map
    app.MapControllers();
    app.MapHealthChecks("/api/healthcheck");
    app.MapHub<MeHub>("/hubs/me"); // signalR hub
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
