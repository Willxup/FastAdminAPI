using FastAdminAPI.Business.IServices;
using FastAdminAPI.Business.PrivateFunc.Applications;
using FastAdminAPI.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FastAdminAPI.Business.Extensions
{
    public static class BusinessServiceInjectionExtension
    {
        /// <summary>
        /// 注入 全部 业务服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            //数据权限
            services.AddScoped<IDataPermissionService, DataPermissionService>();

            //申请审批通用流程
            services.AddScoped<IApplicationHandler, ApplicationHandler>();
            services.AddScoped<IApplicationProcessor, ApplicationProcessor>();

            //区域服务
            services.AddScoped<IRegionService, RegionService>();

            return services;
        }
        /// <summary>
        /// 注入 数据权限 业务服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDataPermissionService(this IServiceCollection services)
        {
            //数据权限
            services.AddScoped<IDataPermissionService, DataPermissionService>();

            return services;
        }
        /// <summary>
        /// 注入 通用申请 业务服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            //申请审批通用流程
            services.AddScoped<IApplicationHandler, ApplicationHandler>();
            services.AddScoped<IApplicationProcessor, ApplicationProcessor>();

            return services;
        }
        /// <summary>
        /// 注入 区域 业务服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRegionService(this IServiceCollection services)
        {
            //区域服务
            services.AddScoped<IRegionService, RegionService>();

            return services;
        }
    }
}
