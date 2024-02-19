using Autofac.Extensions.DependencyInjection;
using FastAdminAPI.Business.Extensions;
using FastAdminAPI.CAP.Extensions;
using FastAdminAPI.Common.Attributes.CheckValidators;
using FastAdminAPI.Common.Extensions;
using FastAdminAPI.Common.Filters;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Middlewares;
using FastAdminAPI.Common.Network;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Common.Swagger;
using FastAdminAPI.Common.SystemUtilities;
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
    //��ȡ����
    IConfiguration configuration = builder.Configuration;
    //��������
    string serviceName = "FastAdminAPI.Core";

    #region Nlog
    // Setup NLog for Dependency injection
    var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    builder.Host.UseNLog();
    #endregion

    #region ���������ļ�
    builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        //����appsetting.json
        config.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{EnvironmentHelper.GetEnv()}.json", optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables();
    });
    #endregion

    #region ���� Services

    #region ����ע��
    builder.Services.AddOptions();

    // Add services to the container.
    builder.Services.AddControllers(c =>
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
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<HttpClientHelper>();
    builder.Services.AddRefitClients(configuration);

    // �����ע��
    builder.Services.AddAllServices();

    //ҵ������
    builder.Services.AddBusinessServices();
    #endregion

    #region SqlSugar ORM���
    builder.Services.AddSingleton<ISqlSugarClient>(sugar =>
    {
        return DbCommonUtils.ConfigSqlSugar(configuration.GetValue<string>("Database.ConnectionString"));
    });
    #endregion

    #region ����
    builder.Services.AddMemoryCache();
    #endregion

    #region  ��Ӧ���ѹ��
    builder.Services.AddCompressResponse();
    #endregion

    #region ͳһ��֤
    builder.Services.AddJwtAuthentication();
    #endregion

    #region swagger�ĵ�����
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwagger(serviceName, AppContext.BaseDirectory);
    #endregion

    #region �¼�����
    builder.Services.AddEventBus(configuration);
    #endregion

    #endregion

    WebApplication app = builder.Build();

    #region ���� Configure

    #region Swagger�ĵ�
    // Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.Configure(serviceName); });
    #endregion

    #region ѹ��
    app.UseResponseCompression();
    #endregion

    #region ������Ӧ��־
    app.UseRequestResponseLog();
    #endregion

    #region У��
    app.UseAuthentication();
    app.UseJwtTokenAuth();//TokenУ��
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

    NLogHelper.Error("������Ŀʧ��!", ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}