using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FastAdminAPI.Common.Datetime;
using FastAdminAPI.Common.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FastAdminAPI.Configuration.Middlewares
{
    /// <summary>
    /// 请求和响应
    /// </summary>
    public class RequestLogMiddleware
    {
        /// <summary>
        /// 委托
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 请求白名单TokenPass
        /// </summary>
        private readonly string[] REQUEST_FILTER_WHITE_URL;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        public RequestLogMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            REQUEST_FILTER_WHITE_URL = configuration.GetValue<string>("RequestFilter.WhiteList")?.ToLowerInvariant()?.Split(",");
        }

        /// <summary>
        /// 通道中间件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            // 跳过健康检查、白名单和非API请求
            if (path.Contains("/api/healthcheck") ||
                (REQUEST_FILTER_WHITE_URL?.Contains(path) ?? false) ||
                !path.StartsWith("/api"))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            try
            {
                // 请求和响应正文直接通过，不读取或替换流
                await _next(context);
            }
            catch (Exception)
            {
                RecordRequestLog(context, stopwatch.ElapsedMilliseconds, true);
                throw;
            }

            RecordRequestLog(context, stopwatch.ElapsedMilliseconds, false);
        }

        /// <summary>
        /// 请求日志
        /// </summary>
        /// <param name="context"></param>
        /// <param name="elapsedMilliseconds"></param>
        /// <param name="failed"></param>
        private static void RecordRequestLog(HttpContext context, long elapsedMilliseconds, bool failed)
        {
            try
            {
                // 仅记录有限的请求摘要，不包含查询参数、正文或Token
                string content = $"TraceIdentifier:{LimitLogValue(context.TraceIdentifier, 128)} " +
                                 $"Method:{LimitLogValue(context.Request.Method, 16)} " +
                                 $"Path:{LimitLogValue(context.Request.Path.Value, 512)} " +
                                 $"StatusCode:{context.Response.StatusCode} " +
                                 $"ElapsedMilliseconds:{elapsedMilliseconds}";

                if (failed)
                {
                    NLogHelper.Error($"请求处理异常，{content}");
                    return;
                }

                //debug模式时，不需要写入日志，直接展示在控制台
#if !DEBUG
                //写入日志
                LogLockHelper.WriteLog("RequestResponseLog", "Request Data:", content);
#else
                //写入控制台
                ConsoleLog("Request Data", content);
#endif
            }
            catch (Exception) { } // 日志失败不影响请求，也不覆盖下游异常
        }

        /// <summary>
        /// 日志字段单行化并限制长度
        /// </summary>
        private static string LimitLogValue(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            char[] chars = value.Substring(0, Math.Min(value.Length, maxLength)).ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsControl(chars[i]) || char.IsWhiteSpace(chars[i]))
                    chars[i] = ' ';
            }
            return new string(chars);
        }
        /// <summary>
        /// 控制台日志
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        private static void ConsoleLog(string type, string content)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now.ToFormattedString());
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"【{type}】：\n");
            Console.ResetColor();
            Console.WriteLine(content);
        }
    }
}

