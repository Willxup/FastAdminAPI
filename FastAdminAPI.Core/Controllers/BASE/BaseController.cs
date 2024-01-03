using FastAdminAPI.Common.BASE;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.Core.Controllers.BASE
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class BaseController : ControllerBase
    {

        /// <summary>
        /// 通用响应-成功
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ResponseModel Success(object data = null, string msg = null)
        {
            ResponseModel result = ResponseModel.Success(data);

            if (!string.IsNullOrEmpty(msg)) result.Message = msg;

            return result;
        }
        /// <summary>
        /// 通用响应-错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected ResponseModel Error(string msg, ResponseCode code = ResponseCode.Error)
        {
            return ResponseModel.Error(msg, code);
        }
        /// <summary>
        /// 通用响应-警告
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        protected ResponseModel Warn(string msg, object data = null, ResponseCode code = ResponseCode.Warn)
        {
            return ResponseModel.Warn(msg, code, data);
        }
    }
}
