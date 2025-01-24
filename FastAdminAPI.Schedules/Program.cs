using FastAdminAPI.CAP.Extensions;
using FastAdminAPI.Common.Attributes.CheckValidators;
using FastAdminAPI.Common.Filters;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Common.Utilities;
using FastAdminAPI.Configuration.Extensions;
using FastAdminAPI.Configuration.Middlewares;
using FastAdminAPI.Configuration.Swagger;
using FastAdminAPI.Framework.Extensions;
using FastAdminAPI.Network.Config;
using FastAdminAPI.Schedules.Configuration;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.IO;

try
{
    //builder
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    //获取配置
    IConfiguration configuration = builder.Configuration;
    //服务名称
    string serviceName = "FastAdminAPI.Schedules";

    #region Nlog
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    builder.Host.UseNLog();
    #endregion

    #region 载入配置文件
    builder.Host
    //.UseServiceProviderFactory(new AutofacServiceProviderFactory())
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
    builder.Services
        .AddControllers(c =>
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
    #endregion

    #region SqlSugar ORM框架
    builder.Services.AddSingleton<ISqlSugarClient>(sugar =>
    {
        return DbExtension.ConfigSqlSugar(configuration.GetValue<string>("Database.ConnectionString"));
    });
    #endregion

    #region hangfire定时任务
    // hangfire
    builder.Services.AddHangfire(config =>
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseRedisStorage(ConnectionMultiplexer.Connect(configuration.GetValue<string>("Redis.ConnectionString")),
            new RedisStorageOptions
            {
                Db = configuration.GetValue<int>("Redis.DbNum"),
                Prefix = "FastAdminAPI_Hangfire:"
            }).WithJobExpirationTimeout(TimeSpan.FromDays(1))
    );
    //用于启动hangfire
    builder.Services.AddHangfireServer();
    #endregion

    #region 缓存
    builder.Services.AddMemoryCache();
    #endregion

    #region  响应结果压缩
    //builder.Services.AddCompressResponse();
    #endregion

    #region 事件总线
    builder.Services.AddEventBus(configuration);
    #endregion

    #region 统一认证
    builder.Services.AddJwtAuthentication();
    #endregion

    #region swagger文档配置
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwagger(serviceName, AppContext.BaseDirectory);
    #endregion

    #endregion

    WebApplication app = builder.Build();

    #region 配置 Configure

    #region Swagger文档
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.Configure(serviceName); });
    #endregion

    #region hangfire定时任务
    //启用Hangfire控制台
    app.UseHangfireDashboard("/dashboard", new DashboardOptions
    {
        DashboardTitle = "FastAdminAPI Schedules Dashboard", //页面标题
        AppPath = "/dashboard",//返回时跳转的地址
        DisplayStorageConnectionString = false,//是否显示数据库连接信息
        DefaultRecordsPerPage = 50, //默认每页显示数据条数
        Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = false, //是否启用ssl验证，即https
            SslRedirect = false,
            LoginCaseSensitive = true,
            Users = new []
            {
                new BasicAuthAuthorizationUser
                {
                    Login = "fastadminapi",
                    PasswordClear =  "123456"
                }
            }
        })}
    });
    #endregion

    #region 创建定时任务
    ScheduleJobCreator.Init(configuration, app.Services);
    #endregion

    #region 请求响应日志
    app.UseRequestResponseLog();
    #endregion

    #region 校验
    app.UseAuthentication();
    app.UseJwtTokenAuth(); //Token校验
    #endregion

    #region Map
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHangfireDashboard();
        endpoints.MapHealthChecks("/api/healthcheck");
    });
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
