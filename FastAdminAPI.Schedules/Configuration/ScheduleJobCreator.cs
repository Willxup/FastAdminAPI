﻿using DotNetCore.CAP;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.Models.Schedules;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace FastAdminAPI.Schedules.Configuration
{
    public static class ScheduleJobCreator
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
        /// 初始化定时任务
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="serviceProvider"></param>
        public static void Init(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            List<ScheduleJobOptions> jobOptions = configuration.GetSection("ScheduleJob").Get<List<ScheduleJobOptions>>();

            if (jobOptions?.Count > 0)
            {
                using var scope = serviceProvider.CreateScope();
                ISqlSugarClient sqlSugar = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
                IRedisHelper redis = scope.ServiceProvider.GetRequiredService<IRedisHelper>();
                IQyWechatApi qyWechatApi = scope.ServiceProvider.GetRequiredService<IQyWechatApi>();
                ICapPublisher capPublisher = scope.ServiceProvider.GetRequiredService<ICapPublisher>();

                foreach (var item in jobOptions)
                {
                    try
                    {
                        if (item.IsEnable)
                        {
                            BaseScheduleJob job = Activator.CreateInstance(Type.GetType($"{NAMESPACE}.{item.JobName},{ASSEMBLY_NAME}"),
                                new object[] { configuration, sqlSugar, redis, qyWechatApi, capPublisher }) as BaseScheduleJob;
                            Create(job, item);
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogHelper.Error($"启动任务【{item.JobName}】异常：{ex.Message}", ex);
                    }
                }
            }
        }
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="job">任务</param>
        /// <param name="jobOptions">任务选项</param>
        public static void Create(BaseScheduleJob job, ScheduleJobOptions jobOptions)
        {
            RecurringJobOptions options = new()
            {
                TimeZone = TimeZoneInfo.Local,
                MisfireHandling = MisfireHandlingMode.Relaxed
            };

            Destroy($"Job.{jobOptions.JobName}");

            switch (jobOptions.Frequency.ToLower())
            {
                case "minutes":
                    RecurringJob.AddOrUpdate($"Job.{jobOptions.JobName}",
                        () => job.Run(),
                        () => $"*/{jobOptions.Minute} * * * *",
                        options);
                    NLogHelper.Debug($"定时任务【{jobOptions.JobName}】创建成功!");
                    break;
                case "hours":
                    RecurringJob.AddOrUpdate($"Job.{jobOptions.JobName}",
                        () => job.Run(),
                        () => $"0 */{jobOptions.Hour} * * *",
                        options);
                    NLogHelper.Debug($"定时任务【{jobOptions.JobName}】创建成功!");
                    break;
                case "daily":
                    RecurringJob.AddOrUpdate($"Job.{jobOptions.JobName}",
                        () => job.Run(),
                        () => Cron.Daily(Convert.ToInt32(jobOptions.Hour), Convert.ToInt32(jobOptions.Minute)),
                        options);
                    NLogHelper.Debug($"定时任务【{jobOptions.JobName}】创建成功!");
                    break;
                case "weekly":
                    RecurringJob.AddOrUpdate($"Job.{jobOptions.JobName}",
                        () => job.Run(),
                        () => Cron.Weekly(ConvertDayOfWeek(jobOptions.DayOfWeek), Convert.ToInt32(jobOptions.Hour), Convert.ToInt32(jobOptions.Minute)),
                        options);
                    NLogHelper.Debug($"定时任务【{jobOptions.JobName}】创建成功!");
                    break;
                case "monthly":
                    RecurringJob.AddOrUpdate($"Job.{jobOptions.JobName}",
                        () => job.Run(),
                        () => Cron.Monthly(Convert.ToInt32(jobOptions.Day), Convert.ToInt32(jobOptions.Hour)),
                        options);
                    NLogHelper.Debug($"定时任务【{jobOptions.JobName}】创建成功!");
                    break;
                case "yearly":
                    RecurringJob.AddOrUpdate($"Job.{jobOptions.JobName}",
                        () => job.Run(),
                        () => Cron.Yearly(Convert.ToInt32(jobOptions.Month), Convert.ToInt32(jobOptions.Day), Convert.ToInt32(jobOptions.Hour)),
                        options);
                    NLogHelper.Debug($"定时任务【{jobOptions.JobName}】创建成功!");
                    break;
                default:
                    NLogHelper.Error($"定时任务【{jobOptions.JobName}】未匹配到任务计划，[{JsonConvert.SerializeObject(jobOptions)}]");
                    break;
            }
        }
        /// <summary>
        /// 销毁任务
        /// </summary>
        /// <param name="jobName"></param>
        public static void Destroy(string jobName)
        {
            RecurringJob.RemoveIfExists($"Job.{jobName}");
        }
        /// <summary>
        /// 转换DayOfWeek枚举
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static DayOfWeek ConvertDayOfWeek(string dayOfWeek)
        {
            try
            {
                int value = Convert.ToInt32(dayOfWeek);

                return EnumExtension.ConvertToEnum<DayOfWeek>(value);
            }
            catch (Exception ex)
            {
                throw new Exception("DayOfWeek枚举转换失败", ex);
            }
        }
    }
}
