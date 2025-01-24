using DotNetCore.CAP;
using FastAdminAPI.CAP.Subscribes;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.Models.QyWechat;
using FastAdminAPI.Network.QyWechat.Common;
using FastAdminAPI.Schedules.Configuration;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Threading.Tasks;

namespace FastAdminAPI.Schedules.ScheduleJob
{
    public class TestJob : BaseScheduleJob
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="dbContext"></param>
        /// <param name="redis"></param>
        /// <param name="qyWechatApi"></param>
        /// <param name="capPublisher"></param>
        public TestJob(IConfiguration configuration, ISqlSugarClient dbContext,
            IRedisHelper redis, IQyWechatApi qyWechatApi, ICapPublisher capPublisher)
            : base(configuration, dbContext, redis, qyWechatApi, capPublisher) { }

        public override async Task Run()
        {
            // 测试事件总线
            await _capPublisher.PublishAsync(SystemSubscriber.NOTIFY_MESSAGE, $"测试事件总线!");

            //测试企业微信通知
            await _qyWechatApi.SendCardMessage(new CardMsgSendModel
            {
                touser = "test_user",
                textcard = new Textcard()
                {
                    btntxt = "详情",
                    title = "企业微信发送信息测试",
                    description = "测试",
                    url = QyWechatNotifyUrls.Get(QyWechatNotifyUrls.NOTIFY_TEST_URL)
                }
            });
        }
    }
}
