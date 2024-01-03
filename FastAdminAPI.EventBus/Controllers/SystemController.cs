using DotNetCore.CAP;
using FastAdminAPI.CAP.Subscribes;
using FastAdminAPI.EventBus.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FastAdminAPI.EventBus.Controllers
{
    /// <summary>
    /// 系统
    /// </summary>
    public class SystemController : ControllerBase
    {
        /// <summary>
        /// 系统Service
        /// </summary>
        private readonly ISystemService _systemService;

        /// <summary>
        /// 构造
        /// </summary>
        public SystemController(ISystemService systemService) 
        {
            _systemService = systemService;
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>        
        [CapSubscribe(SystemSubscriber.NOTIFY_MESSAGE)]
        public async Task SendMessage(string msg)
        {
            await _systemService.SendMessage(msg);
        }
    }
}
