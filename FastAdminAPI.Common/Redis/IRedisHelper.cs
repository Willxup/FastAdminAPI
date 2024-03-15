using FastAdminAPI.Common.Redis.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Common.Redis
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface IRedisHelper
    {
        #region key
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        bool KeyExists(string key);
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        Task<bool> KeyExistsAsync(string key);

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        bool KeyRename(string key, string newKey);
        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        Task<bool> KeyRenameAsync(string key, string newKey);

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool KeySetExpire(string key, TimeSpan? expiry = default);
        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<bool> KeySetExpireAsync(string key, TimeSpan? expiry = default);

        /// <summary>
        /// 获取rediskey的存活时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        double? KeyTimeToLive(string key);
        /// <summary>
        /// 获取rediskey的存活时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<double?> KeyTimeToLiveAsync(string key);

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        bool KeyDelete(string key);
        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        Task<bool> KeyDeleteAsync(string key);

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        long KeyDelete(List<string> keys);
        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        Task<long> KeyDeleteAsync(List<string> keys);
        #endregion key

        #region String
        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        string StringGet(string key);
        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        Task<string> StringGetAsync(string key);

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T StringGet<T>(string key);
        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> StringGetAsync<T>(string key);

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        T[] StringGet<T>(List<string> listKey);
        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        Task<T[]> StringGetAsync<T>(List<string> listKey);

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="when">条件(默认随时插入)</param>
        /// <returns></returns>
        bool StringSet(string key, string value, TimeSpan? expiry = default, When when = When.Always);
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="when">条件(默认随时插入)</param>
        /// <returns></returns>
        Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default, When when = When.Always);

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool StringSet<T>(string key, T obj, TimeSpan? expiry = default);
        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default);

        /// <summary>
        /// 保存多个相同结构key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        bool StringSet<T>(Dictionary<string, T> keyValues);
        /// <summary>
        /// 保存多个相同结构key value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        Task<bool> StringSetAsync<T>(Dictionary<string, T> keyValues);

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues);
        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        Task<bool> StringSetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        double StringIncrement(string key, double val = 1);
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        Task<double> StringIncrementAsync(string key, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        double StringDecrement(string key, double val = 1);
        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        Task<double> StringDecrementAsync(string key, double val = 1);
        #endregion String

        #region Hash
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        bool HashExist(string key, string dataKey);
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<bool> HashExistAsync(string key, string dataKey);

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        T HashGet<T>(string key, string dataKey);
        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<T> HashGetAsync<T>(string key, string dataKey);

        /// <summary>
        /// 从hash表获取多个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKeyList"></param>
        /// <returns></returns>
        List<T> HashGet<T>(string key, List<string> dataKeyList);
        /// <summary>
        /// 从hash表获取多个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKeyList"></param>
        /// <returns></returns>
        Task<List<T>> HashGetAsync<T>(string key, List<string> dataKeyList);

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> HashGetKeys<T>(string key);
        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<T>> HashGetKeysAsync<T>(string key);

        /// <summary>
        /// 获取HashKey中所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Dictionary<string, T> HashGetAll<T>(string key);
        /// <summary>
        /// 获取HashKey中所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<Dictionary<string, T>> HashGetAllAsync<T>(string key);

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        bool HashSet<T>(string key, string dataKey, T t);
        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> HashSetAsync<T>(string key, string dataKey, T t);

        /// <summary>
        /// 存储多个数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dic"></param>
        void HashSet<T>(string key, Dictionary<string, T> dic);
        /// <summary>
        /// 存储多个数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dic"></param>
        Task HashSetAsync<T>(string key, Dictionary<string, T> dic);

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        bool HashDelete(string key, string dataKey);
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        Task<bool> HashDeleteAsync(string key, string dataKey);

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        long HashDelete(string key, List<string> dataKeys);
        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        Task<long> HashDeleteAsync(string key, List<string> dataKeys);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        double HashIncrement(string key, string dataKey, double val = 1);
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        Task<double> HashIncrementAsync(string key, string dataKey, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        double HashDecrement(string key, string dataKey, double val = 1);
        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        Task<double> HashDecrementAsync(string key, string dataKey, double val = 1);
        #endregion Hash

        #region List
        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> ListGet<T>(string key);
        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<T>> ListGetAsync<T>(string key);

        /// <summary>
        /// 获取集合的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long ListLength(string key);
        /// <summary>
        /// 获取集合的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> ListLengthAsync(string key);

        /// <summary>
        /// 列表 头部 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ListPushLeft<T>(string key, T value);
        /// <summary>
        /// 列表 头部 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<long> ListPushLeftAsync<T>(string key, T value);

        /// <summary>
        /// 列表 尾部 插入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ListPushRight<T>(string key, T value);
        /// <summary>
        /// 列表 尾部 插入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<long> ListPushRightAsync<T>(string key, T value);

        /// <summary>
        /// 列表 头部 移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T ListPopLeft<T>(string key);
        /// <summary>
        /// 列表 头部 移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> ListPopLeftAsync<T>(string key);

        /// <summary>
        /// 列表 尾部 移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T ListPopRight<T>(string key);
        /// <summary>
        /// 列表 尾部 移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> ListPopRightAsync<T>(string key);

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ListRemove<T>(string key, T value);
        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<long> ListRemoveAsync<T>(string key, T value);
        #endregion List

        #region SortedSet
        /// <summary>
        /// 集合中是否存在该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SortedSetExist<T>(string key, T value);
        /// <summary>
        /// 集合中是否存在该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> SortedSetExistAsync<T>(string key, T value);

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long SortedSetLength(string key);
        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> SortedSetLengthAsync(string key);

        /// <summary>
        /// 获取成员分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        double? SortedSetGetScore<T>(string key, T value);
        /// <summary>
        /// 获取成员分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<double?> SortedSetGetScoreAsync<T>(string key, T value);

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<string> SortedSetGetRange(string key, Order order = Order.Ascending);
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<string>> SortedSetGetRangeAsync(string key, Order order = Order.Ascending);

        /// <summary>
        /// 通过指定范围(索引)获取全部（包含权重）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Dictionary<string, double> SortedSetRangeByRankWithScores(string key, long start = 0, long stop = -1, Order order = Order.Ascending);
        /// <summary>
        /// 通过指定范围(索引)获取全部（包含权重）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<Dictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending);

        /// <summary>
        /// 通过指定范围(分数)获取全部（包含权重）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        Dictionary<string, double> SortedSetRangeByScoreWithScores(string key, long start = 0, long stop = -1, Order order = Order.Ascending);
        /// <summary>
        /// 通过指定范围(分数)获取全部（包含权重）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<Dictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Order order = Order.Ascending, long skip = 0, long take = -1);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        bool SortedSetAdd<T>(string key, T value, double score);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        Task<bool> SortedSetAddAsync<T>(string key, T value, double score);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        bool SortedSetRemove<T>(string key, T value);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task<bool> SortedSetRemoveAsync<T>(string key, T value);

        /// <summary>
        /// 给指定成员增加相应的分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        double SortedSetIncrement<T>(string key, T value, double score);
        /// <summary>
        /// 给指定成员增加相应的分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        Task<double> SortedSetIncrementAsync<T>(string key, T value, double score);
        #endregion SortedSet

        #region 发布订阅
        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null);
        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="redis"></param>
        /// <param name="handler"></param>
        void Subscribe(string subChannel, IRedisHelper redis, Func<IRedisHelper, string, Task> handler = null);
        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="redis"></param>
        /// <param name="handler"></param>
        void Subscribe(string subChannel, IRedisHelper redis, Action<IRedisHelper> handler = null);
        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="redis"></param>
        /// <param name="handler"></param>
        void Subscribe(string subChannel, IRedisHelper redis, Action<IRedisHelper,string> handler = null);

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        long Publish<T>(string channel, T msg);

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        void Unsubscribe(string channel);

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        void UnsubscribeAll();
        #endregion 发布订阅

        #region lock
        /// <summary>
        /// 获取Redis锁
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="token">令牌</param>
        /// <param name="expiry">过期时间(秒)，默认10s</param>
        /// <returns></returns>
        bool GetLock(string lockName, string token, int expiry = 10);
        /// <summary>
        /// 获取Redis锁
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="token">令牌</param>
        /// <param name="expiry">过期时间(秒)，默认10s</param>
        /// <returns></returns>
        Task<bool> GetLockAsync(string lockName, string token, int expiry = 10);

        /// <summary>
        /// 释放Redis锁
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        bool ReleaseLock(string lockName, string token);
        /// <summary>
        /// 释放Redis锁
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        Task<bool> ReleaseLockAsync(string lockName, string token);

        /// <summary>
        /// 使用Redis分布式锁执行某些操作
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="func">操作</param>
        /// <param name="expiry">锁过期时间，若超出时间自动解锁 单位：sec</param>
        /// <param name="retry">获取锁的重复次数</param>
        /// <param name="tryDelay">获取锁的重试间隔  单位：ms</param>
        RedisLockResultModel<TResult> Lock<TResult>(string lockName, Func<TResult> func, int expiry = 10, int retry = 3, int tryDelay = 200);
        /// <summary>
        /// 使用Redis分布式锁执行某些异步操作
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="func">操作</param>
        /// <param name="expiry">锁过期时间，若超出时间自动解锁 单位：sec</param>
        /// <param name="retry">获取锁的重复次数</param>
        /// <param name="tryDelay">获取锁的重试间隔  单位：ms</param>
        Task<RedisLockResultModel<TResult>> LockAsync<TResult>(string lockName, Func<Task<TResult>> func, int expiry = 10, int retry = 3, int tryDelay = 200);
        #endregion

        #region 其他
        /// <summary>
        /// 创建事务
        /// </summary>
        /// <returns></returns>
        ITransaction CreateTransaction();
        /// <summary>
        /// 获取Redis数据库
        /// </summary>
        /// <returns></returns>
        IDatabase GetDatabase();
        /// <summary>
        /// 获取Redis服务
        /// </summary>
        /// <param name="hostAndPort"></param>
        /// <returns></returns>
        IServer GetServer(string hostAndPort);

        /// <summary>
        /// 获取前缀
        /// </summary>
        /// <returns></returns>
        string GetPrefixKey();
        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        void SetPrefixKey(string customKey);
        /// <summary>
        /// 重置前缀
        /// </summary>
        void ResetPrefixKey();
        /// <summary>
        /// 模糊查询keys
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        Task<string[]> QueryKeysAsync(string pattern);
        #endregion 其他
    }
}
