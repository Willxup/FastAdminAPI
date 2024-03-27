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
                  .AddJsonFile($"appsettings.{EnvTool.GetEnv()}.json", optional: true, reloadOnChange: true)
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
    builder.Services.AddHttpClientConfig();
    builder.Services.AddRefitClients(configuration);

    // �����ע��
    builder.Services.AddAllServices();

    //ҵ������
    builder.Services.AddBusinessServices();
    #endregion

    #region SqlSugar ORM���
    builder.Services.AddSingleton<ISqlSugarClient>(sugar =>
    {
        return DbExtension.ConfigSqlSugar(configuration.GetValue<string>("Database.ConnectionString"));
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

    #region ���ӣ���ȡIOC����ע��Ķ���
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

    NLogHelper.Error("������Ŀʧ��!", ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}