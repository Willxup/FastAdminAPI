using Microsoft.AspNetCore.Builder;

namespace FastAdminAPI.Configuration.Middlewares
{
    public static class UseMiddlewareHelpers
    {
        /// <summary>
        /// JWT��֤
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtTokenAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtTokenAuthMiddleware>();
        }
        /// <summary>
        /// ������Ӧ��־
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestResponseLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLogMiddleware>();
        }
    }
}
