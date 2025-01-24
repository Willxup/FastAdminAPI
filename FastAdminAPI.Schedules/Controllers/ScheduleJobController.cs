using DotNetCore.CAP;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.Models.Schedules;
using FastAdminAPI.Schedules.Configuration;
using FastAdminAPI.Schedules.Controllers.BASE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace FastAdminAPI.Schedules.Controllers
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class ScheduleJobController : BaseController
    {
        /// <summary>
        /// 程序集名称
        /// </summary>
        private static readonly string ASSEMBLY_NAME = "FastAdminAPI.Schedules";
        /// <summary>
        /// 命名空间
        /// </summary>
        private static readonly string NAMESPACE = "FastAdminAPI.Schedules.ScheduleJob";
        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// ORM
        /// </summary>
        private readonly ISqlSugarClient _sqlSugar;
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;
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
        /// <param name="configuration"></param>
        /// <param name="sqlSugar"></param>
        /// <param name="redis"></param>
        /// <param name="qyWechatApi"></param>
        /// <param name="capPublisher"></param>
        public ScheduleJobController(IConfiguration configuration, ISqlSugarClient sqlSugar, IRedisHelper redis, 
            IQyWechatApi qyWechatApi, ICapPublisher capPublisher)
        {
            _configuration = configuration;
            _sqlSugar = sqlSugar;
            _redis = redis;
            _qyWechatApi = qyWechatApi;
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 启用定时任务
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> Enable([FromBody] ScheduleJobOptions options)
        {

            try
            {
                if (options.IsEnable)
                {
                    BaseScheduleJob job = Activator.CreateInstance(Type.GetType($"{NAMESPACE}.{options.JobName},{ASSEMBLY_NAME}"),
                                    new object[] { _configuration, _sqlSugar, _redis, _qyWechatApi, _capPublisher }) as BaseScheduleJob;
                    ScheduleJobCreator.Create(job, options);
                }

                return await Task.FromResult(Success());
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"启动任务【{options.JobName}】异常：{ex.Message}", ex);
                return await Task.FromResult(Error("启用定时任务失败!"));
            }
        }
        /// <summary>
        /// 停用定时任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> Disable([FromQuery] string jobName)
        {
            try
            {
                ScheduleJobCreator.Destroy(jobName);
                return await Task.FromResult(Success());
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"停用任务【{jobName}】异常：{ex.Message}", ex);
                return await Task.FromResult(Error("停用定时任务失败!"));
            }
        }
    }
}
