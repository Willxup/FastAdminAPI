using FastAdminAPI.Common.BASE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace FastAdminAPI.Common.Attributes.CheckValidators
{
    /// <summary>
    /// Model规则有效性验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class ModelValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                string msg = string.Empty;
                foreach (var key in modelState.Keys)
                {
                    var state = modelState[key];
                    if (state.Errors.Any())
                    {
                        msg += state.Errors.First().ErrorMessage; // + "，";
                    }
                }
                //msg = msg.Remove(msg.Length - 1, 1) + "。";
                ResponseModel response = new()
                {
                    Code = ResponseCode.Error,
                    Message = msg
                };
                actionContext.HttpContext.Response.StatusCode = 200;
                actionContext.Result = new ObjectResult(response);
            }
        }
    }
}
