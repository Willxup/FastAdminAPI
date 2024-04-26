using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace FastAdminAPI.Common.Attributes
{
    /// <summary>
    /// 过时弃用API
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ObsoletedApiAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotSupportedException("Api not supported!");
        }
    }
}
