using DotNetCore.CAP;
using FastAdminAPI.CAP.Subscribes;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.Models.QyWechat;
using FastAdminAPI.Network.QyWechat.Common;
using FastAdminAPI.Tasks.Config;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Threading.Tasks;

namespace FastAdminAPI.Tasks.Tasks
{
    /// <summary>
    /// 测试
    /// </summary>
    public class TestTask : BaseTask
    {

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="configuration"></param>
        /// <param name="redis"></param>
        /// <param name="qyWechatApi"></param>
        public TestTask(ISqlSugarClient dbContext, IConfiguration configuration, IRedisHelper redis, IQyWechatApi qyWechatApi, ICapPublisher capPublisher)
            : base(dbContext, configuration, redis, qyWechatApi, capPublisher) { }


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
