using Microsoft.AspNetCore.Builder;

namespace FastAdminAPI.Common.Middlewares
{
    public static class UseMiddlewareHelpers
    {
        /// <summary>
        /// JWT认证
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtTokenAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtTokenAuthMiddleware>();
        }
        /// <summary>
        /// 请求响应日志
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestResponseLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLogMiddleware>();
        }
    }
}
