﻿using System;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Network.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FastAdminAPI.CAP.Extensions
{
    public static class CapConfigExtension
    {
        /// <summary>
        /// CAP事件总线配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCap(opt => 
            {
                opt.ConfigureCAP(configuration);
            });

            return services;
        }
        /// <summary>
        /// CAP事件总线通用配置
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static CapOptions ConfigureCAP(this CapOptions options, IConfiguration configuration)
        {
            //使用MySQL作为存储
            options.UseMySql(config => 
            {
                //MySQL连接字符串
                config.ConnectionString = configuration.GetValue<string>("Database.ConnectionString");
                
                //CAP生成的表名前缀
                config.TableNamePrefix = "SYS_CAP";
            });

            //使用Redis Streams 作为消息传输器
            options.UseRedis(config =>
            {
                //Redis连接字符串
                ConfigurationOptions opt = ConfigurationOptions.Parse(configuration.GetValue<string>("Redis.ConnectionString"));
                //默认数据库
                opt.DefaultDatabase = configuration.GetValue<int>("Redis.DbNum");

                config.Configuration = opt;
            });


            //设置失败重试次数
            options.FailedRetryCount = configuration.GetValue<int>("CAP.FailedRetryCount");

            //执行失败以后,下一次执行的时间间隔（秒）
            options.FailedRetryInterval = configuration.GetValue<int>("CAP.FailedRetryInterval");

            //设置处理成功的数据在数据库中保存的时间（秒），为保证系统性能，数据会定期清理。
            options.SucceedMessageExpiredAfter = configuration.GetValue<int>("CAP.SucceedMessageExpiredAfter");

            //设置处理失败的数据再数据库中保存的时间（秒）
            options.FailedMessageExpiredAfter = configuration.GetValue<int>("CAP.FailedMessageExpiredAfter");

            //分组前缀
            options.GroupNamePrefix = "FastAdminAPI";
            //默认分组名称
            options.DefaultGroupName = "DefultGroup";

            ////Topic前缀
            //x.TopicNamePrefix = "Topic.";

            //失败超过重试次数后，发送邮件提醒
            options.FailedThresholdCallback = async (message) =>
            {
                try
                {
                    string title = "【CAP事件总线】异常提醒  ";
                    string msgId = message.Message?.Headers["cap-msg-id"]; //获取消息Id
                    string msgName = message.Message?.Headers["cap-msg-name"]; //获取消息名称
                    string msgSetTime = message.Message?.Headers["cap-senttime"]; //获取发送时间
                    string body = $"CAP事件总线异常 - MsgId:[{msgId}], MsgName:[{msgName}], MsgSetTime:[{msgSetTime}], Msg:[{message.Message.Value}]";
                    switch (message?.MessageType) //消息类型
                    {
                        case MessageType.Publish:
                            title += "[Publish]";
                            break;
                        case MessageType.Subscribe:
                            title += "[Subscribe]";
                            break;
                        default:
                            break;
                    };

                    //发送邮件
                    var emailApi = message.ServiceProvider.GetRequiredService<IEmailApi>();
                    await emailApi.SendEmailByDefault(title, body);

                    Console.WriteLine(title, body);
                }
                catch (Exception ex)
                {
                    NLogHelper.Error($"【CAP事件总线】异常提醒 发送失败!", ex);
                }
            };

            return options;
        }
    }
}
