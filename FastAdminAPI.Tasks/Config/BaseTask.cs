using DotNetCore.CAP;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace FastAdminAPI.Tasks.Config
{
    /// <summary>
    /// 抽象任务类
    /// </summary>
    public abstract class BaseTask
    {
        /// <summary>
        /// Sqlsugar
        /// </summary>
        protected SqlSugarScope _dbContext;
        /// <summary>
        /// 配置
        /// </summary>
        protected readonly IConfiguration _configuration;
        /// <summary>
        /// Redis帮助类
        /// </summary>
        protected readonly IRedisHelper _redis;
        /// <summary>
        /// 企业微信Api接口
        /// </summary>
        protected readonly IQyWechatApi _qyWechatApi;
        /// <summary>
        /// 事件总线发布
        /// </summary>
        protected readonly ICapPublisher _capPublisher;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="configuration"></param>
        /// <param name="redis"></param>
        /// <param name="serviceProvider"></param>
        public BaseTask(ISqlSugarClient dbContext, IConfiguration configuration, IRedisHelper redis, IQyWechatApi qyWechatApi, ICapPublisher capPublisher)
        {
            _dbContext = dbContext as SqlSugarScope;
            _configuration = configuration;
            _redis = redis;
            _qyWechatApi = qyWechatApi;
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 子类实现的要做的事情
        /// </summary>
        public abstract Task Run();
    }
}
