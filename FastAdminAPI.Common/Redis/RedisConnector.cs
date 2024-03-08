using FastAdminAPI.Common.Logs;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace FastAdminAPI.Common.Redis
{
    /// <summary>
    /// Redis连接
    /// </summary>
    public static class RedisConnector
    {
        /// <summary>
        /// 对象锁
        /// </summary>
        private static readonly object _lock = new();
        /// <summary>
        /// 连接实例
        /// </summary>
        private static ConnectionMultiplexer _instance = null;
        /// <summary>
        /// 连接缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> _connects = new();

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="Configuration">配置</param>
        /// <param name="connectString">连接字符串</param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetInstance(IConfiguration Configuration, string connectString = "")
        {
            try
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null || !_instance.IsConnected)
                        {
                            _instance = GetConnect(Configuration, connectString);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                NLogHelper.Error($"Redis获取连接失败!{ex.Message}", ex);
            }
                
            return _instance;
            
        }

        /// <summary>
        /// 获取Redis连接缓存
        /// </summary>
        /// <param name="Configuration">配置</param>
        /// <param name="connectString">连接字符串</param>
        /// <returns></returns>
        private static ConnectionMultiplexer GetConnect(IConfiguration Configuration,string connectString = "")
        {
            string key = connectString;

            //如果连接字符串为空，写入默认key
            if (string.IsNullOrWhiteSpace(key))
            {
                key = "default_connect";
            }

            if (!_connects.ContainsKey(key))
            {
                _connects[key] = Connect(Configuration,connectString);
            }
            return _connects[key];
        }

        /// <summary>
        /// 获取Redis连接
        /// </summary>
        /// <param name="Configuration">配置</param>
        /// <param name="connectString">连接字符串</param>
        /// <returns></returns>
        private static ConnectionMultiplexer Connect(IConfiguration Configuration,string connectString = "")
        {
            //连接字符串
            connectString = string.IsNullOrEmpty(connectString) ? Configuration.GetValue<string>("Redis.ConnectionString") : connectString;
            
            //连接Redis
            var connect = ConnectionMultiplexer.Connect(connectString);

            //注册事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;

            return connect;
        }

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine("ErrorMessage: " + e.Message);
            NLogHelper.Error($"Redis Error：{e.Message}");
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败, 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine("InternalError:Message" + e.Exception.Message);
            NLogHelper.Error($"Redis InternalError：{e.Exception.Message}");
        }

        #endregion 事件
    }
}
