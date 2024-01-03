using FastAdminAPI.EventBus.IServices;
using FastAdminAPI.EventBus.Services.BASE;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace FastAdminAPI.EventBus.Services
{
    /// <summary>
    /// 系统
    /// </summary>
    public class SystemService : BaseService, ISystemService
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        public SystemService(ISqlSugarClient dbContext):base(dbContext) { }


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
