using DotNetCore.CAP;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System.Threading.Tasks;

namespace FastAdminAPI.Schedules.Configuration
{
    /// <summary>
    /// 抽象任务类
    /// </summary>
    public abstract class BaseScheduleJob
    {
        /// <summary>
        /// 配置
        /// </summary>
        protected readonly IConfiguration _configuration;
        /// <summary>
        /// Sqlsugar
        /// </summary>
        protected SqlSugarScope _dbContext;
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

        public BaseScheduleJob(IConfiguration configuration, ISqlSugarClient dbContext, IRedisHelper redis,
            IQyWechatApi qyWechatApi, ICapPublisher capPublisher)
        {
            _configuration = configuration;
            _dbContext = dbContext as SqlSugarScope;
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
