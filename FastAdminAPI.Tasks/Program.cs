using FastAdminAPI.CAP.Extensions;
using FastAdminAPI.Common.Attributes.CheckValidators;
using FastAdminAPI.Common.Filters;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Common.Utilities;
using FastAdminAPI.Framework.Extensions;
using FastAdminAPI.Network.Config;
using FastAdminAPI.Tasks.Config;
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
    //��ȡ����
    IConfiguration configuration = builder.Configuration;
    //��������
    //string serviceName = "FastAdminAPI.Tasks";

    #region Nlog
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    builder.Host.UseNLog();
    #endregion

    #region ���������ļ�
    builder.Host
    //.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        //����appsetting.json
        config.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{EnvTools.GetEnv()}.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
    });
    #endregion

    #region ���� Services

    #region ����ע��
    builder.Services.AddOptions();

    // Add services to the container.
    builder.Services
        .AddControllers(c =>
        {
            c.Filters.Add<ModelValidationAttribute>();
            c.Filters.Add(typeof(GlobalExceptionsFilter));
        })
        .AddControllersAsServices()
        .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; })// ȡ��Ĭ���շ�
        .AddNewtonsoftJson(options => //Newtonsoft.Json
        {
            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
        });

    //�������
    builder.Services.AddHealthChecks();

    // HttpContext
    builder.Services.AddHttpContextAccessor();

    // Redis
    builder.Services.AddSingleton<IRedisHelper, RedisHelper>();

    // HttpClient & Helper & Refit
    builder.Services.AddHttpClientConfig();
    builder.Services.AddRefitClients(configuration);
    #endregion

    #region SqlSugar ORM���
    builder.Services.AddSingleton<ISqlSugarClient>(sugar =>
    {
        return DbCommonUtils.ConfigSqlSugar(configuration.GetValue<string>("Database.ConnectionString"));
    });
    #endregion

    #region hangfire��ʱ����
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
            }).WithJobExpirationTimeout(TimeSpan.FromDays(7))
    );
    //��������hangfire
    builder.Services.AddHangfireServer();
    #endregion

    #region ����
    builder.Services.AddMemoryCache();
    #endregion

    #region  ��Ӧ���ѹ��
    //builder.Services.AddCompressResponse();
    #endregion

    #region �¼�����
    builder.Services.AddEventBus(configuration);
    #endregion

    #endregion

    WebApplication app = builder.Build();

    #region ���� Configure

    #region hangfire��ʱ����
    //����Hangfire����̨
    app.UseHangfireDashboard("/dashboard", new DashboardOptions
    {
        DashboardTitle = "FastAdminAPI Tasks Dashboard", //ҳ�����
        AppPath = "/dashboard",//����ʱ��ת�ĵ�ַ
        DisplayStorageConnectionString = false,//�Ƿ���ʾ���ݿ�������Ϣ
        DefaultRecordsPerPage = 50, //Ĭ��ÿҳ��ʾ��������
        Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = false, //�Ƿ�����ssl��֤����https
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

    #region ������ʱ����
    TaskCreator.Create(configuration, app.Services);
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

    NLogHelper.Error("������Ŀʧ��!", ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}



