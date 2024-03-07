using FastAdminAPI.Common.Authentications;
using FastAdminAPI.Common.Converters;
using FastAdminAPI.Common.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Common.Middlewares
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
            REQUEST_FILTER_WHITE_URL = configuration.GetValue<string>("RequestFilter.WhiteList").ToLower().Split(",");
        }

        /// <summary>
        /// 通道中间件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            //健康检查
            if (context.Request.Path.Value.ToLower().Contains("/api/healthcheck"))
            {
                await _next(context);
                return;
            }

            //白名单
            if (REQUEST_FILTER_WHITE_URL.Contains(context.Request.Path.Value.ToLower()))
            {
                await _next(context);
                return;
            }

            // 拦截API接口
            if (context.Request.Path.Value.ToLower().Contains("/api"))
            {
                context.Request.EnableBuffering();

                string connectId = context.Connection.Id;

                using Stream originalBody = context.Response.Body;
                try
                {
                    // 请求
                    await RecordRequestLog(context.Request, connectId);

                    using var ms = new MemoryStream();
                    context.Response.Body = ms;

                    // 访问下一中间件
                    await _next(context);

                    try
                    {
                        // 响应
                        await RecordResponseLog(context.Response, ms, connectId);

                        ms.Position = 0;
                        await ms.CopyToAsync(originalBody);
                    }
                    catch (Exception ex)
                    {
                        // 记录
                        NLogHelper.Error(context.Response.ToString(), ex);
                    }
                }
                catch (Exception ex)
                {
                    // 记录
                    NLogHelper.Error(context.Response.ToString(), ex);
                    await _next(context);
                }
                finally
                {
                    context.Response.Body = originalBody;
                }
            }
            else
            {
                await _next(context);
                return;
            }
        }

        /// <summary>
        /// 请求日志
        /// </summary>
        /// <param name="request"></param>
        /// <param name="connectId"></param>
        /// <returns></returns>
        private static async Task RecordRequestLog(HttpRequest request, string connectId)
        {
            try
            {
                //log内容
                string content = string.Empty;

                if (request.Path.Value.ToLower().Contains("excel"))
                {
                    content = $"ConnectId:{connectId}\r\n" +
                              $"QueryData:{request.Path + request.QueryString}\r\n" +
                              $"Operator:{GetRequestOperatorByToken(request)}\r\n" +
                              $"RequestPath:{request.GetDisplayUrl()}\r\n";
                }
                else
                {

                    //读取请求体
                    var sr = new StreamReader(request.Body);

                    content = $"ConnectId:{connectId}\r\n" +
                              $"QueryData:{request.Path + request.QueryString}\r\n" +
                              $"BodyData:{await sr.ReadToEndAsync()}\r\n" +
                              $"Operator:{GetRequestOperatorByToken(request)}\r\n" +
                              $"RequestPath:{request.GetDisplayUrl()}\r\n";
                }



                if (!string.IsNullOrEmpty(content))
                {
                    //debug模式时，不需要写入日志，直接展示在控制台
#if !DEBUG
                    //写入日志
                    LogLockHelper.WriteLog("RequestResponseLog", "Request Data:", content);
#else
                    //写入控制台
                    ConsoleLog("Request Data", content);
#endif

                    //将请求体的流重置
                    request.Body.Position = 0;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 响应日志
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ms"></param>
        /// <param name="connectId"></param>
        /// <returns></returns>
        private static async Task RecordResponseLog(HttpResponse response, MemoryStream ms, string connectId)
        {
            try
            {
                //响应体
                string body = string.Empty;

                if (response.HttpContext.Request.Path.Value.ToLower().Contains("excel"))
                {
                    body = $"ConnectId:{connectId}\r\n";
                }
                else
                {
                    //设置流的位置
                    ms.Position = 0;

                    //读取响应
                    body = await new StreamReader(ms).ReadToEndAsync();
                }

                if (!string.IsNullOrEmpty(body))
                {
                    string content = $"ConnectId:{connectId}\r\n" +
                                     $"BodyData:{body}\r\n";

                    //debug模式时，不需要写入日志，直接展示在控制台
#if !DEBUG
                    //写入日志
                    LogLockHelper.WriteLog("RequestResponseLog", "Response Data:", content);
#else
                    //写入控制台
                    ConsoleLog("Response Data", content);
#endif
                }

            }
            catch (Exception) { }
        }

        /// <summary>
        /// 获取token中的请求操作人信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetRequestOperatorByToken(HttpRequest request)
        {
            string operatorName = null;
            if (!string.IsNullOrEmpty(request.Headers.Authorization))
            {
                string token = request.Headers.Authorization.ToString()?.Replace("Bearer ", "");
                try
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        JwtTokenModel jwtToken = JwtHelper.SerializeJwt(token);
                        operatorName = $"[{jwtToken.UserId}]-[{jwtToken.EmployeeId}]-[{jwtToken.EmployeeName}]";
                    }
                }
                catch (Exception ex)
                {
                    NLogHelper.Error($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - 解析token失败，token:【{token}】", ex);
                }
            }
            return operatorName;
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

