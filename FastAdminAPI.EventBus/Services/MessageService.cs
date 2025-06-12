using System;
using System.Threading.Tasks;
using FastAdminAPI.EventBus.IServices;
using FastAdminAPI.EventBus.Services.BASE;
using SqlSugar;

namespace FastAdminAPI.EventBus.Services
{
    /// <summary>
    /// 信息中心
    /// </summary>
    public class MessageService : BaseService, IMessageService
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        public MessageService(ISqlSugarClient dbContext):base(dbContext) { }


        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <returns></returns>
        public async Task SendMessage(string msg)
        {
            await Task.Delay(100);
            Console.WriteLine($"TEST_CAP:{msg}");
        }
    }
}
