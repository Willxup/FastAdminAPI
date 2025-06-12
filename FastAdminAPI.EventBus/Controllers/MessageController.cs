using System.Threading.Tasks;
using DotNetCore.CAP;
using FastAdminAPI.CAP.Subscribes;
using FastAdminAPI.EventBus.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.EventBus.Controllers
{
    /// <summary>
    /// 信息中心
    /// </summary>
    public class MessageController : ControllerBase
    {
        /// <summary>
        /// 系统Service
        /// </summary>
        private readonly IMessageService _messageService;

        /// <summary>
        /// 构造
        /// </summary>
        public MessageController(IMessageService systemService) 
        {
            _messageService = systemService;
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe(SystemSubscriber.NOTIFY_MESSAGE)]
        public async Task SendMessage(string msg)
        {
            await _messageService.SendMessage(msg);
        }
    }
}
