using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Utilities;

namespace FastAdminAPI.Configuration.Filters
{
    /// <summary>
    /// 全局异常日志
    /// </summary>
    public class GlobalExceptionsFilter : IExceptionFilter
    {
        public GlobalExceptionsFilter()
        {
        }

        public void OnException(ExceptionContext context)
        {
            ResponseModel responseModel = new()
            {
                Code = ResponseCode.Error,
                Message = context.Exception.Message
            };

            //自定义的操作记录日志
            if (context.Exception.GetType() == typeof(UserOperationException))
            {
                //用于处理模型校验的异常
                var result = new ObjectResult(responseModel)
                {
                    StatusCode = StatusCodes.Status200OK
                };
                context.Result = result;
            }
            else
            {
                if (!EnvTool.IsProduction)
                {
                    //堆栈信息
                    responseModel.Data = context.Exception.StackTrace.TrimStart();
                }
                //500
                context.Result = new InternalServerErrorObjectResult(responseModel);
                //日志记录
                NLogHelper.Error(responseModel.Message, context.Exception);
            }

        }
    }

    /// <summary>
    /// 响应码
    /// </summary>
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

}
