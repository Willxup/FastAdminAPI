using DotNetCore.CAP;
using FastAdminAPI.Business.PrivateFunc.Applications.Models;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.PrivateFunc.Applications
{
    /// <summary>
    /// 申请处理器抽象类
    /// </summary>
    internal abstract class AbstractApplicationProcessor
    {
        /// <summary>
        /// SugarScope
        /// </summary>
        protected readonly SqlSugarScope _dbContext;
        /// <summary>
        /// Redis
        /// </summary>
        protected readonly IRedisHelper _redis;
        /// <summary>
        /// 配置
        /// </summary>
        protected readonly IConfiguration _configuration;
        /// <summary>
        /// 企业微信API
        /// </summary>
        protected readonly IQyWechatApi _qyWechatApi;
        /// <summary>
        /// 事件总线
        /// </summary>
        protected readonly ICapPublisher _capPublisher;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redis"></param>
        /// <param name="configuration"></param>
        /// <param name="capPublisher"></param>
        internal AbstractApplicationProcessor(ISqlSugarClient dbContext, IRedisHelper redis, IConfiguration configuration, IQyWechatApi qyWechatApi, ICapPublisher capPublisher)
        {
            _dbContext = dbContext as SqlSugarScope;
            _redis = redis;
            _configuration = configuration;
            _qyWechatApi = qyWechatApi;
            _capPublisher = capPublisher;
        }
        /// <summary>
        /// 完成申请
        /// </summary>
        /// <param name="applicationType">申请类型</param>
        /// <param name="data">完成申请所需数据</param>
        /// <returns></returns>
        internal abstract Task<ResponseModel> CompleteApplication(long applicationType, CompleteApplicationModel data);
        /// <summary>
        /// 拒绝申请
        /// </summary>
        /// <param name="applicationType">申请类型</param>
        /// <param name="data">完成申请所需数据</param>
        /// <returns></returns>
        internal abstract Task<ResponseModel> RejectApplication(long applicationType, CompleteApplicationModel data);
    }
}
