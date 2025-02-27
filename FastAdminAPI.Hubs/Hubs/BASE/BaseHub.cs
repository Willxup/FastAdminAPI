using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Configuration.BASE;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Hubs.Hubs.BASE
{
    public class BaseHub<T> : Hub<T>
        where T : class
    {
        protected readonly IRedisHelper _redis;

        public BaseHub(IRedisHelper redis)
        {
            _redis = redis;
        }
        public override async Task OnConnectedAsync()
        {

            if (Context.User != null && Context.User.Claims?.Count() > 0)
            {
                HubConnection connection = new()
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = Convert.ToInt64(Context.User.Claims.First(c => c.Type == "UserId").Value),
                    Account = Context.User.Claims.First(c => c.Type == "Account").Value,
                    EmployeeId = Convert.ToInt64(Context.User.Claims.First(c => c.Type == "EmployeeId").Value),
                    EmployeeName = Context.User.Claims.First(c => c.Type == "EmployeeName").Value
                };
                await Groups.AddToGroupAsync(connection.ConnectionId, "DEFAULT_GROUP");
                await _redis.HashSetAsync(Define.HUB_CONNECT_REDIS_KEY, connection.ConnectionId, connection);
            }
            else
                throw new UserOperationException("获取用户信息失败!");
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _redis.HashDeleteAsync(Define.HUB_CONNECT_REDIS_KEY, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
