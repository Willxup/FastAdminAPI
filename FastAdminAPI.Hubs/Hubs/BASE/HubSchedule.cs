using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Configuration.BASE;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace FastAdminAPI.Hubs.Hubs.BASE
{
    /// <summary>
    /// Hub 定时任务 用于推送消息
    /// </summary>
    public class HubSchedule : BackgroundService
    {
        private readonly IHubContext<MeHub, IMeHub> _hubContext;
        private readonly IRedisHelper _redis;
        private static Dictionary<string, int> CLIENT_MESSAGE_COUNT;

        public HubSchedule(IHubContext<MeHub, IMeHub> hubContext, IRedisHelper redis)
        {
            _hubContext = hubContext;
            _redis = redis;
            CLIENT_MESSAGE_COUNT = new Dictionary<string, int>();
        }

        /// <summary>
        /// 定时任务
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _redis.KeyDeleteAsync(Define.HUB_CONNECT_REDIS_KEY);

            while (!stoppingToken.IsCancellationRequested)
            {
                var connections = await _redis.HashGetAllAsync<HubConnection>(Define.HUB_CONNECT_REDIS_KEY);

                if (connections?.Count > 0)
                {
                    foreach (var item in connections)
                    {
                        HubConnection connection = item.Value;

                        int count = GetUnreadMessageCount(connection.ConnectionId);
                        await _hubContext.Clients.Client(item.Key).SetUnreadMessageCount(count);
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
        /// <summary>
        /// 获取未读消息数 模拟从数据库或Redis取数
        /// </summary>
        /// <param name="connectionid"></param>
        /// <returns></returns>
        private static int GetUnreadMessageCount(string connectionid)
        {
            if(CLIENT_MESSAGE_COUNT.TryGetValue(connectionid, out int count))
            {
                count++;
                CLIENT_MESSAGE_COUNT[connectionid] = count;
                return count;
            }
            else
            {
                CLIENT_MESSAGE_COUNT.Add(connectionid, 0);
                return 0;
            }
        }
    }
}
