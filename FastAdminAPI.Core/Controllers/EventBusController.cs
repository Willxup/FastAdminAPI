using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public class EventBusController : BaseController
    {
        /// <summary>
        /// 事件总线
        /// </summary>
        private readonly IEventBusService _evenBus;

        public EventBusController(IEventBusService evenBus) 
        {
            _evenBus = evenBus;
        }

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task PublishMessage([FromQuery][Required(ErrorMessage = "信息不能为空!")] string Msg)
        {
            await _evenBus.PublishMessage(Msg);
        }
    }
}
