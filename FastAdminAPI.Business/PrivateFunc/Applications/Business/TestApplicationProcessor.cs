using DotNetCore.CAP;
using FastAdminAPI.Business.PrivateFunc.Applications.Models;
using FastAdminAPI.CAP.Subscribes;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.Models.QyWechat;
using FastAdminAPI.Network.QyWechat.Common;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.PrivateFunc.Applications.Business
{
    /// <summary>
    /// 测试申请处理器(审批实现)
    /// </summary>
    internal class TestApplicationProcessor : AbstractApplicationProcessor
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redis"></param>
        /// <param name="configuration"></param>
        /// <param name="qyWechatApi"></param>
        internal TestApplicationProcessor(ISqlSugarClient dbContext, IRedisHelper redis, 
            IConfiguration configuration, ICapPublisher capPublisher, IQyWechatApi qyWechatApi, IEmailApi emailApi) 
            : base(dbContext, redis, configuration, capPublisher, qyWechatApi, emailApi) { }

        /// <summary>
        /// 通过申请
        /// </summary>
        /// <param name="applicationType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal override async Task<ResponseModel> Accept(long applicationType, CompleteApplicationModel data)
        {
            return applicationType switch
            {
                //调课申请
                (long)ApplicationEnums.ApplicationType.Test => await AcceptTestApplication(data),
                _ => throw new UserOperationException("申请类型错误!")
            };
        }
        /// <summary>
        /// 拒绝申请
        /// </summary>
        /// <param name="applicationType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal override async Task<ResponseModel> Reject(long applicationType, CompleteApplicationModel data)
        {
            return applicationType switch
            {
                //调课申请
                (long)ApplicationEnums.ApplicationType.Test => await RejectTestApplication(data),
                _ => throw new UserOperationException("申请类型错误!")
            };
        }

        #region 实现
        /// <summary>
        /// 通过Test申请
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<ResponseModel> AcceptTestApplication(CompleteApplicationModel data)
        {
            // 测试事件总线
            await _capPublisher.PublishAsync(SystemSubscriber.NOTIFY_MESSAGE, $"Test申请已通过!");

            //测试企业微信通知
            await _qyWechatApi.SendCardMessage(new CardMsgSendModel
            {
                touser = "test_user",
                textcard = new Textcard()
                {
                    btntxt = "详情",
                    title = "企业微信发送信息-Test申请已通过",
                    description = data.DataContent,
                    url = QyWechatNotifyUrls.Get(QyWechatNotifyUrls.NOTIFY_TEST_URL)
                }
            });

            return ResponseModel.Success();
        }
        /// <summary>
        /// 拒绝Test申请
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<ResponseModel> RejectTestApplication(CompleteApplicationModel data)
        {
            // 测试事件总线
            await _capPublisher.PublishAsync(SystemSubscriber.NOTIFY_MESSAGE, $"Test申请未通过!");

            //测试企业微信通知
            await _qyWechatApi.SendCardMessage(new CardMsgSendModel
            {
                touser = "test_user",
                textcard = new Textcard()
                {
                    btntxt = "详情",
                    title = "企业微信发送信息-Test申请未通过",
                    description = data.DataContent,
                    url = QyWechatNotifyUrls.Get(QyWechatNotifyUrls.NOTIFY_TEST_URL)
                }
            });

            return ResponseModel.Success();
        }
        #endregion
    }
}
