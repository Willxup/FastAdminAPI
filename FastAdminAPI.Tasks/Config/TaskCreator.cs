using DotNetCore.CAP;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastAdminAPI.Tasks.Config
{
    public static class TaskCreator
    {
        /// <summary>
        /// 程序集名称
        /// </summary>
        private static readonly string ASSEMBLY_NAME = "FastAdminAPI.Tasks";
        /// <summary>
        /// 命名空间
        /// </summary>
        private static readonly string NAMESPACE = "FastAdminAPI.Tasks.Tasks";

        public static void Create(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            // 获取定时任务配置信息
            List<string> tasks = configuration.GetValue<string>("Task.Configures")?.Split(";")?.ToList();

            if (tasks?.Count > 0)
            {
                // 获取服务域
                using var scope = serviceProvider.CreateScope();

                // 创建所需实例
                ISqlSugarClient sqlSugar = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
                IRedisHelper redis = scope.ServiceProvider.GetRequiredService<IRedisHelper>();
                IQyWechatApi qyWechatApi = scope.ServiceProvider.GetRequiredService<IQyWechatApi>();
                ICapPublisher capPublisher = scope.ServiceProvider.GetRequiredService<ICapPublisher>();

                foreach (var item in tasks)
                {
                    try
                    {
                        // 分隔定时任务配置信息
                        List<string> taskInfo = item.Split(",").ToList();
                        
                        // 定时任务是否开启
                        if (configuration.GetValue<bool>($"Task.{taskInfo[0]}"))
                        {
                            BaseTask task = Activator.CreateInstance(Type.GetType($"{NAMESPACE}.{taskInfo[0]},{ASSEMBLY_NAME}"),
                                                                new object[] { sqlSugar, configuration, redis, qyWechatApi, capPublisher }) as BaseTask;

                            // 设置时区
                            RecurringJobOptions options = new()
                            {
                                TimeZone = TimeZoneInfo.Local,
                                MisfireHandling = MisfireHandlingMode.Relaxed
                            };

                            // 多长时间执行一次定时任务
                            switch (taskInfo[1].ToLower())
                            {
                                // 每多少分钟执行一次
                                case "min":
                                    RecurringJob.AddOrUpdate($"Task.{taskInfo[0]}", () => task.Run(), () => $"*/{taskInfo[2]} * * * *", options);
                                    break;

                                // 每多少小时执行一次
                                case "hour":
                                    RecurringJob.AddOrUpdate($"Task.{taskInfo[0]}", () => task.Run(), () => $"0 */{taskInfo[2]} * * *", options);
                                    break;

                                // 每天指定时间(时,分)执行一次
                                case "daily":
                                    RecurringJob.AddOrUpdate($"Task.{taskInfo[0]}", () => task.Run(), () => Cron.Daily(Convert.ToInt32(taskInfo[2]), 
                                        Convert.ToInt32(taskInfo[3])), options);
                                    break;

                                // 每月指定时间(天,时)执行一次
                                case "monthly":
                                    RecurringJob.AddOrUpdate($"Task.{taskInfo[0]}", () => task.Run(), () => Cron.Monthly(Convert.ToInt32(taskInfo[2]), 
                                        Convert.ToInt32(taskInfo[3])), options);
                                    break;

                                // 每年指定时间(月,天,时)执行一次
                                case "yearly":
                                    RecurringJob.AddOrUpdate($"Task.{taskInfo[0]}", () => task.Run(), () => Cron.Yearly(Convert.ToInt32(taskInfo[2]), 
                                        Convert.ToInt32(taskInfo[3]), Convert.ToInt32(taskInfo[4])), options);
                                    break;

                                // cron表达式
                                case "cron":
                                    RecurringJob.AddOrUpdate($"Task.{taskInfo[0]}", () => task.Run(), () => taskInfo[2], options);
                                    break;

                                // 其他情况
                                default:
                                    NLogHelper.Error($"定时任务Task.{taskInfo[0]}未匹配到任务计划，[{JsonConvert.SerializeObject(taskInfo)}]");
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogHelper.Error($"启动任务【{item}】异常：{ex.Message}", ex);
                    }
                }
            }
        }
    }
}
