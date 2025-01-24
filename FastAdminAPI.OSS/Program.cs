using Autofac.Extensions.DependencyInjection;
using FastAdminAPI.Common.Attributes.CheckValidators;
using FastAdminAPI.Common.Filters;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Common.Utilities;
using FastAdminAPI.Configuration.Extensions;
using FastAdminAPI.Configuration.Middlewares;
using FastAdminAPI.Configuration.Swagger;
using FastAdminAPI.Framework.Extensions;
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
    string serviceName = "FastAdminAPI.OSS";

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

    #region 跨域
    app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(o => true).AllowCredentials());
    #endregion

    #region 校验
    app.UseAuthentication();
    app.UseJwtTokenAuth();//Token校验
    #endregion

    #region 静态文件
    app.UseStaticFiles();
    #endregion

    #region Map
    app.MapControllers();
    app.MapHealthChecks("/api/healthcheck");
    #endregion

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
