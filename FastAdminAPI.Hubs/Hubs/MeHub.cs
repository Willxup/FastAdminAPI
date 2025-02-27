using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Configuration.BASE;
using FastAdminAPI.Hubs.Hubs.BASE;
using FastAdminAPI.Hubs.Hubs.Interfaces;
using System.Threading.Tasks;

namespace FastAdminAPI.Hubs.Hubs
{
    /// <summary>
    /// MeHub 服务端方法
    /// </summary>
    public class MeHub : BaseHub<IMeHub>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="redis"></param>
        public MeHub(IRedisHelper redis) : base(redis) { }

        /// <summary>
        /// 获取未读消息数
        /// </summary>
        /// <returns></returns>
        public async Task GetUnreadMessageCount()
        {
            var connection = await _redis.HashGetAsync<HubConnection>(Define.HUB_CONNECT_REDIS_KEY, Context.ConnectionId);

            if (connection != null)
            {
                var count = 1; // 可从数据库或Redis中查询

                await Clients.Caller.SetUnreadMessageCount(count); // 调用客户端方法
            }
            else
            {
                await base.OnDisconnectedAsync(new UserOperationException("获取用户信息失败!"));
            }
        }
    }
}
