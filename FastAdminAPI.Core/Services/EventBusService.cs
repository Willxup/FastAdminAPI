using System.Threading.Tasks;
using DotNetCore.CAP;
using FastAdminAPI.CAP.Subscribes;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Services.BASE;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace FastAdminAPI.Core.Services
{
    public class EventBusService : BaseService, IEventBusService
    {
        /// <summary>
        /// 事件总线发布
        /// </summary>
        private readonly ICapPublisher _capPublisher;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        /// <param name="capPublisher"></param>
        public EventBusService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext, ICapPublisher capPublisher) : base(dbContext, httpContext)
        {
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <returns></returns>
        public async Task PublishMessage(string msg)
        {
            await _capPublisher.PublishAsync(SystemSubscriber.NOTIFY_MESSAGE, msg);
        }
    }
}
