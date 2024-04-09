using DotNetCore.CAP;
using FastAdminAPI.Business.PrivateFunc.Applications.Business;
using FastAdminAPI.Business.PrivateFunc.Applications.Models;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.PrivateFunc.Applications
{
    /// <summary>
    /// 申请处理器
    /// </summary>
    internal class ApplicationProcessor : IApplicationProcessor
    {
        /// <summary>
        /// SugarScope
        /// </summary>
        private readonly SqlSugarScope _dbContext;
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;
        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 事件总线
        /// </summary>
        private readonly ICapPublisher _capPublisher;
        /// <summary>
        /// 企业微信API
        /// </summary>
        private readonly IQyWechatApi _qyWechatApi;
        /// <summary>
        /// 邮件Api
        /// </summary>
        private readonly IEmailApi _emailApi;


        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redis"></param>
        /// <param name="configuration"></param>
        /// <param name="capPublisher"></param>
        public ApplicationProcessor(ISqlSugarClient dbContext, IRedisHelper redis, IConfiguration configuration, 
            ICapPublisher capPublisher, IQyWechatApi qyWechatApi, IEmailApi emailApi)
        {
            _dbContext = dbContext as SqlSugarScope;
            _redis = redis;
            _configuration = configuration;
            _capPublisher = capPublisher;
            _qyWechatApi = qyWechatApi;
            _emailApi = emailApi;
        }

        /// <summary>
        /// 获取申请处理器
        /// </summary>
        /// <param name="applicationCategory"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        private AbstractApplicationProcessor GetApplicationProcessor(byte applicationCategory)
        {
            return applicationCategory switch
            {
                //测试
                (int)ApplicationEnums.ApplicationCategory.Test => new TestApplicationProcessor(_dbContext, _redis, _configuration, _capPublisher, _qyWechatApi, _emailApi),
                //其他
                _ => throw new UserOperationException("无法识别的申请类别!")
            };
        }

        /// <summary>
        /// 完成申请
        /// </summary>
        /// <param name="applicationCategory">申请类别</param>
        /// <param name="applicationType">申请类型</param>
        /// <param name="data">完成申请所需数据</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> AcceptApplication(byte applicationCategory, long applicationType, CompleteApplicationModel data)
        {

            return await GetApplicationProcessor(applicationCategory).AcceptApplication(applicationType, data);
        }
        /// <summary>
        /// 拒绝申请
        /// </summary>
        /// <param name="applicationCategory">申请类别</param>
        /// <param name="applicationType">申请类型</param>
        /// <param name="data">完成申请所需数据</param>
        /// <returns></returns>
        public async Task<ResponseModel> RejectApplication(byte applicationCategory, long applicationType, CompleteApplicationModel data)
        {
            return await GetApplicationProcessor(applicationCategory).RejectApplication(applicationType, data);
        }
    }
}
