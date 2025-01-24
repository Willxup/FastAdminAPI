using FastAdminAPI.Common.BASE;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.Schedules.Controllers.BASE
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class BaseController : ControllerBase
    {
        protected ResponseModel Success(object data = null, string msg = null)
        {
            ResponseModel result = ResponseModel.Success(data);

            if (!string.IsNullOrEmpty(msg)) result.Message = msg;

            return result;
        }
        protected ResponseModel Error(string msg, ResponseCode code = ResponseCode.Error)
        {
            return ResponseModel.Error(msg, code);
        }
    }
}
