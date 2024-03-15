using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FastAdminAPI.Common.Redis
{
    /// <summary>
    /// Redis缓存
    /// </summary>
    public class RedisHelper : IRedisHelper
    {
        /// <summary>
        /// redis数据库编号
        /// </summary>
        private readonly int DATABASE_NUM;
        /// <summary>
        /// 自定义前缀
        /// </summary>
        private string CUSTOMER_PREFIX_KEY;

        /// <summary>
        /// 连接
        /// </summary>
        private readonly ConnectionMultiplexer _conn;
        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="dbNum">数据库编号</param>
        public RedisHelper(IConfiguration configuration, int? dbNum = null) : this(configuration, null, dbNum) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <param name="connectString">连接字符串</param>
        /// <param name="dbNum">数据库编号</param>
        public RedisHelper(IConfiguration configuration, string connectString, int? dbNum)
        {
            _configuration = configuration;

            _conn = RedisConnector.GetInstance(configuration, connectString);

            DATABASE_NUM = dbNum ?? configuration.GetValue<int>("Redis.DbNum");
            CUSTOMER_PREFIX_KEY = configuration.GetValue<string>("Redis.PrefixKey");
        }

        #region 内部方法
        /// <summary>
        /// 添加前缀
        /// </summary>
        /// <param name="oldKey"></param>
        /// <returns></returns>
        private string AddPrefixKey(string oldKey)
        {
            //var prefixKey = CustomKey ?? RedisConnectionHelper.SysCustomKey;
            var prefixKey = string.IsNullOrEmpty(CUSTOMER_PREFIX_KEY) ? _configuration.GetValue<string>("Redis.PrefixKey") : CUSTOMER_PREFIX_KEY;
            return prefixKey + oldKey;
        }
        /// <summary>
        /// Do 有返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private T Do<T>(Func<IDatabase, T> func)
        {
            var database = _conn.GetDatabase(DATABASE_NUM);
            return func(database);
        }
        /// <summary>
        /// Do 无返回
        /// </summary>
        /// <param name="func"></param>
        private void Do(Action<IDatabase> func)
        {
            var database = _conn.GetDatabase(DATABASE_NUM);
            func(database);
        }

        #region 类型转换
        /// <summary>
        /// List<string>转Redis键数组
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>

        private static RedisKey[] ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }
        /// <summary>
        /// List<string>转Redis值数组
        /// </summary>
        /// <param name="redisValues"></param>
        /// <returns></returns>
        private static RedisValue[] ConvertRedisValues(List<string> redisValues)
        {
            return redisValues.Select(redisValue => (RedisValue)redisValue).ToArray();
        }
        /// <summary>
        /// 对象转Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }
        /// <summary>
        /// RedisValue转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T ConvertObj<T>(RedisValue value)
        {
            if (value.IsNull)
            {
                return default;
            }
            //返回结果类型为字符串
            else if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
        /// <summary>
        /// SortedSet转字典
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static Dictionary<string, double> ConvertSortedSetToDic(SortedSetEntry[] values)
        {
            Dictionary<string, double> result = new();
            foreach (var item in values)
            {
                result.Add(item.Element.ToString(), item.Score);
            }
            return result;
        }
        /// <summary>
        /// redis值数组转List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        private static List<T> ConvetListToValue<T>(RedisValue[] values)
        {
            List<T> result = new();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// redis值数组转List<string>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static List<string> ConvetListToValue(RedisValue[] values)
        {
            List<string> result = new();
            foreach (var item in values)
            {
                var model = item.ToString();
                result.Add(model);
            }
            return result;
        }
        #endregion

        #endregion

        #region key
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            key = AddPrefixKey(key);
            return Do(db => db.KeyExists(key));
        }
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public async Task<bool> KeyExistsAsync(string key)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.KeyExistsAsync(key));
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            key = AddPrefixKey(key);
            return Do(db => db.KeyRename(key, AddPrefixKey(newKey)));
        }
        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public async Task<bool> KeyRenameAsync(string key, string newKey)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.KeyRenameAsync(key, AddPrefixKey(newKey)));
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeySetExpire(string key, TimeSpan? expiry = default)
        {
            key = AddPrefixKey(key);
            return Do(db => db.KeyExpire(key, expiry));
        }
        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> KeySetExpireAsync(string key, TimeSpan? expiry = default)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.KeyExpireAsync(key, expiry));
        }

        /// <summary>
        /// 获取rediskey的存活时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double? KeyTimeToLive(string key)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.KeyTimeToLive(key))?.TotalSeconds;
        }
        /// <summary>
        /// 获取rediskey的存活时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<double?> KeyTimeToLiveAsync(string key)
        {
            key = AddPrefixKey(key);
            var ttl = await Do(redis => redis.KeyTimeToLiveAsync(key));

            return ttl?.TotalSeconds;
        }

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            key = AddPrefixKey(key);
            return Do(db => db.KeyDelete(key));
        }
        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public async Task<bool> KeyDeleteAsync(string key)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.KeyDeleteAsync(key));
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(List<string> keys)
        {
            List<string> newKeys = keys.Select(c => AddPrefixKey(c)).ToList();
            return Do(db => db.KeyDelete(ConvertRedisKeys(newKeys)));
        }
        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public async Task<long> KeyDeleteAsync(List<string> keys)
        {
            List<string> newKeys = keys.Select(c => AddPrefixKey(c)).ToList();
            return await Do(db => db.KeyDeleteAsync(ConvertRedisKeys(newKeys)));
        }
        #endregion key

        #region String
        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            key = AddPrefixKey(key);
            return Do(db => db.StringGet(key));
        }
        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public async Task<string> StringGetAsync(string key)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.StringGetAsync(key));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db => ConvertObj<T>(db.StringGet(key)));
        }
        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> StringGetAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            string result = await Do(db => db.StringGetAsync(key));
            return ConvertObj<T>(result);
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public T[] StringGet<T>(List<string> listKey)
        {
            List<string> newKeys = listKey.Select(AddPrefixKey).ToList();
            return Do(db => db.StringGet(ConvertRedisKeys(newKeys))).Select(c => ConvertObj<T>(c)).ToArray();
        }
        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public async Task<T[]> StringGetAsync<T>(List<string> listKey)
        {
            List<string> newKeys = listKey.Select(AddPrefixKey).ToList();
            var result = await Do(db => db.StringGetAsync(ConvertRedisKeys(newKeys)));
            return result.Select(c => ConvertObj<T>(c)).ToArray();
        }

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan? expiry = default, When when = When.Always)
        {
            key = AddPrefixKey(key);
            return Do(db => db.StringSet(key, value, expiry, when));
        }
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = default, When when = When.Always)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.StringSetAsync(key, value, expiry, when));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T obj, TimeSpan? expiry = default)
        {
            key = AddPrefixKey(key);
            string json = ConvertJson(obj);
            return Do(db => db.StringSet(key, json, expiry));
        }
        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = default)
        {
            key = AddPrefixKey(key);
            string json = ConvertJson(obj);
            return await Do(db => db.StringSetAsync(key, json, expiry));
        }

        /// <summary>
        /// 保存多个相同结构key value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public bool StringSet<T>(Dictionary<string, T> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newKeyValues =
                keyValues.Select(c => new KeyValuePair<RedisKey, RedisValue>(AddPrefixKey(c.Key), ConvertJson(c.Value))).ToList();
            return Do(db => db.StringSet(newKeyValues.ToArray()));
        }
        /// <summary>
        /// 保存多个相同结构key value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync<T>(Dictionary<string, T> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newKeyValues =
                keyValues.Select(c => new KeyValuePair<RedisKey, RedisValue>(AddPrefixKey(c.Key), ConvertJson(c.Value))).ToList();
            return await Do(db => db.StringSetAsync(newKeyValues.ToArray()));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newKeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddPrefixKey(p.Key), p.Value)).ToList();
            return Do(db => db.StringSet(newKeyValues.ToArray()));
        }
        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(AddPrefixKey(p.Key), p.Value)).ToList();
            return await Do(db => db.StringSetAsync(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return Do(db => db.StringIncrement(key, val));
        }
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> StringIncrementAsync(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.StringIncrementAsync(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return Do(db => db.StringDecrement(key, val));
        }
        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> StringDecrementAsync(string key, double val = 1)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.StringDecrementAsync(key, val));
        }
        #endregion String

        #region Hash
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExist(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return Do(db => db.HashExists(key, dataKey));
        }
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistAsync(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            var result = Do(db => db.HashGet(key, dataKey));
            return ConvertObj<T>(result);
        }
        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T> HashGetAsync<T>(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            string value = await Do(db => db.HashGetAsync(key, dataKey));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 从hash表获取多个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKeyList"></param>
        /// <returns></returns>
        public List<T> HashGet<T>(string key, List<string> dataKeyList)
        {
            key = AddPrefixKey(key);
            var result = Do(db => db.HashGet(key, ConvertRedisValues(dataKeyList)));
            return ConvetListToValue<T>(result);

        }
        /// <summary>
        /// 从hash表获取多个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKeyList"></param>
        /// <returns></returns>
        public async Task<List<T>> HashGetAsync<T>(string key, List<string> dataKeyList)
        {
            key = AddPrefixKey(key);
            var result = await Do(db => db.HashGetAsync(key, ConvertRedisValues(dataKeyList)));
            return ConvetListToValue<T>(result);
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashGetKeys<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                RedisValue[] values = db.HashKeys(key);
                return ConvetListToValue<T>(values);
            });
        }
        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashGetKeysAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            RedisValue[] values = await Do(db => db.HashKeysAsync(key));
            return ConvetListToValue<T>(values);
        }

        /// <summary>
        /// 获取HashKey中所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T> HashGetAll<T>(string key)
        {
            Dictionary<string, T> keyValuePairs = new();

            key = AddPrefixKey(key);

            HashEntry[] dic = Do(db => db.HashGetAll(key));
            foreach (var item in dic)
            {
                keyValuePairs.Add(item.Name, ConvertObj<T>(item.Value));
            }

            return keyValuePairs;
        }
        /// <summary>
        /// 获取HashKey中所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key)
        {
            Dictionary<string, T> keyValuePairs = new();

            key = AddPrefixKey(key);

            HashEntry[] dic = await Do(db => db.HashGetAllAsync(key));
            foreach (var item in dic)
            {
                keyValuePairs.Add(item.Name, ConvertObj<T>(item.Value));
            }

            return keyValuePairs;
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                string json = ConvertJson(t);
                return db.HashSet(key, dataKey, json);
            });
        }
        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T t)
        {
            key = AddPrefixKey(key);
            return await Do(db =>
            {
                string json = ConvertJson(t);
                return db.HashSetAsync(key, dataKey, json);
            });
        }

        /// <summary>
        /// 存储多个数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dic"></param>
        public void HashSet<T>(string key, Dictionary<string, T> dic)
        {
            key = AddPrefixKey(key);
            Do(db =>
            {
                List<HashEntry> list = new();
                foreach (var item in dic)
                    list.Add(new HashEntry(item.Key, ConvertJson(item.Value)));
                db.HashSet(key, list.ToArray());
            });
        }
        /// <summary>
        /// 存储多个数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dic"></param>
        public async Task HashSetAsync<T>(string key, Dictionary<string, T> dic)
        {
            key = AddPrefixKey(key);
            await Do(db =>
            {
                List<HashEntry> list = new();
                foreach (var item in dic)
                    list.Add(new HashEntry(item.Key, ConvertJson(item.Value)));
                return db.HashSetAsync(key, list.ToArray());
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return Do(db => db.HashDelete(key, dataKey));
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete(string key, List<string> dataKeys)
        {
            key = AddPrefixKey(key);
            //List<RedisValue> dataKeys1 = new List<RedisValue>() {"1","2"};
            return Do(db => db.HashDelete(key, ConvertRedisValues(dataKeys)));
        }
        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public async Task<long> HashDeleteAsync(string key, List<string> dataKeys)
        {
            key = AddPrefixKey(key);
            //List<RedisValue> dataKeys1 = new List<RedisValue>() {"1","2"};
            return await Do(db => db.HashDeleteAsync(key, ConvertRedisValues(dataKeys)));
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return Do(db => db.HashIncrement(key, dataKey, val));
        }
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return Do(db => db.HashDecrement(key, dataKey, val));
        }
        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.HashDecrementAsync(key, dataKey, val));
        }
        #endregion Hash

        #region List
        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListGet<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(redis =>
            {
                var values = redis.ListRange(key);
                return ConvetListToValue<T>(values);
            });
        }
        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListGetAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var values = await Do(redis => redis.ListRangeAsync(key));
            return ConvetListToValue<T>(values);
        }

        /// <summary>
        /// 获取集合的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.ListLength(key));
        }
        /// <summary>
        /// 获取集合的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string key)
        {
            key = AddPrefixKey(key);
            return await Do(redis => redis.ListLengthAsync(key));
        }

        /// <summary>
        /// 列表 头部 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListPushLeft<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            Do(db => db.ListLeftPush(key, ConvertJson(value)));
        }
        /// <summary>
        /// 列表 头部 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListPushLeftAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.ListLeftPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 列表 尾部 插入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListPushRight<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            Do(db => db.ListRightPush(key, ConvertJson(value)));
        }
        /// <summary>
        /// 列表 尾部 插入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListPushRightAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.ListRightPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 列表 头部 移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListPopLeft<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                var value = db.ListLeftPop(key);
                return ConvertObj<T>(value);
            });
        }
        /// <summary>
        /// 列表 头部 移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListPopLeftAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var value = await Do(db => db.ListLeftPopAsync(key));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 列表 尾部 移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListPopRight<T>(string key)
        {
            key = AddPrefixKey(key);
            return Do(db =>
            {
                var value = db.ListRightPop(key);
                return ConvertObj<T>(value);
            });
        }
        /// <summary>
        /// 列表 尾部 移除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListPopRightAsync<T>(string key)
        {
            key = AddPrefixKey(key);
            var value = await Do(db => db.ListRightPopAsync(key));
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRemove<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            Do(db => db.ListRemove(key, ConvertJson(value)));
        }
        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRemoveAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await Do(db => db.ListRemoveAsync(key, ConvertJson(value)));
        }
        #endregion List

        #region SortedSet
        /// <summary>
        /// 集合中是否存在该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SortedSetExist<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SetContains(key, ConvertJson(value)));
        }
        /// <summary>
        /// 集合中是否存在该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetExistAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await Do(redis => redis.SetContainsAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetLength(key));
        }
        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string key)
        {
            key = AddPrefixKey(key);
            return await Do(redis => redis.SortedSetLengthAsync(key));
        }

        /// <summary>
        /// 获取成员分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public double? SortedSetGetScore<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetScore(key, ConvertJson(value)));
        }
        /// <summary>
        /// 获取成员分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<double?> SortedSetGetScoreAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await Do(redis => redis.SortedSetScoreAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> SortedSetGetRange(string key, Order order = Order.Ascending)
        {
            key = AddPrefixKey(key);
            return Do(redis =>
            {
                var values = redis.SortedSetRangeByRank(key, order: order);
                return ConvetListToValue(values);
            });
        }
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<string>> SortedSetGetRangeAsync(string key, Order order = Order.Ascending)
        {
            key = AddPrefixKey(key);
            var values = await Do(redis => redis.SortedSetRangeByRankAsync(key, order: order));
            return ConvetListToValue(values);
        }

        /// <summary>
        /// 通过指定范围(索引)获取全部（包含权重）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public Dictionary<string, double> SortedSetRangeByRankWithScores(string key, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            key = AddPrefixKey(key);
            var values = Do(redis => redis.SortedSetRangeByRankWithScores(key, start, stop, order: order));
            return ConvertSortedSetToDic(values);
        }
        /// <summary>
        /// 通过指定范围(索引)获取全部（包含权重）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            key = AddPrefixKey(key);
            var values = await Do(redis => redis.SortedSetRangeByRankWithScoresAsync(key, start, stop, order: order));
            return ConvertSortedSetToDic(values);
        }

        /// <summary>
        /// 通过指定范围(分数)获取全部（包含权重）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public Dictionary<string, double> SortedSetRangeByScoreWithScores(string key, long start = 0, long stop = -1, Order order = Order.Ascending)
        {
            key = AddPrefixKey(key);
            var values = Do(redis => redis.SortedSetRangeByScoreWithScores(key, start, stop, order: order));
            return ConvertSortedSetToDic(values);
        }
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
        public async Task<Dictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Order order = Order.Ascending, long skip = 0, long take = -1)
        {
            key = AddPrefixKey(key);
            var values = await Do(redis => redis.SortedSetRangeByScoreWithScoresAsync(key, start, stop, order: order, skip: skip, take: take));
            return ConvertSortedSetToDic(values);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetAdd(key, ConvertJson<T>(value), score));
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
        {
            key = AddPrefixKey(key);
            return await Do(redis => redis.SortedSetAddAsync(key, ConvertJson<T>(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetRemove(key, ConvertJson(value)));
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
        {
            key = AddPrefixKey(key);
            return await Do(redis => redis.SortedSetRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 给指定成员增加相应的分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public double SortedSetIncrement<T>(string key, T value, double score)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetIncrement(key, ConvertJson<T>(value), score));
        }
        /// <summary>
        /// 给指定成员增加相应的分数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public Task<double> SortedSetIncrementAsync<T>(string key, T value, double score)
        {
            key = AddPrefixKey(key);
            return Do(redis => redis.SortedSetIncrementAsync(key, ConvertJson<T>(value), score));
        }
        #endregion SortedSet

        #region 发布订阅
        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            subChannel = AddPrefixKey(subChannel);
            RedisChannel ch = new(subChannel, RedisChannel.PatternMode.Auto);
            ISubscriber sub = _conn.GetSubscriber();
            sub.Subscribe(ch, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }
        /// <summary>
        /// Redis发布订阅 订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="redis"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, IRedisHelper redis, Func<IRedisHelper, string, Task> handler = null)
        {
            subChannel = AddPrefixKey(subChannel);
            RedisChannel ch = new(subChannel, RedisChannel.PatternMode.Auto);
            ISubscriber sub = _conn.GetSubscriber();
            sub.Subscribe(ch, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(redis, message);
                }
            });
        }
        /// <summary>
        /// Redis发布订阅 订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="redis"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, IRedisHelper redis, Action<IRedisHelper> handler = null)
        {
            subChannel = AddPrefixKey(subChannel);
            RedisChannel ch = new(subChannel, RedisChannel.PatternMode.Auto);
            ISubscriber sub = _conn.GetSubscriber();
            sub.Subscribe(ch, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(redis);
                }
            });
        }
        /// <summary>
        /// Redis发布订阅 订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="redis"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, IRedisHelper redis, Action<IRedisHelper, string> handler = null)
        {
            subChannel = AddPrefixKey(subChannel);
            RedisChannel ch = new(subChannel, RedisChannel.PatternMode.Auto);
            ISubscriber sub = _conn.GetSubscriber();
            sub.Subscribe(ch, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(redis, message);
                }
            });
        }

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T msg)
        {
            channel = AddPrefixKey(channel);
            RedisChannel ch = new(channel, RedisChannel.PatternMode.Auto);
            ISubscriber sub = _conn.GetSubscriber();
            return sub.Publish(ch, ConvertJson(msg));
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            channel = AddPrefixKey(channel);
            RedisChannel ch = new(channel, RedisChannel.PatternMode.Auto);
            ISubscriber sub = _conn.GetSubscriber();
            sub.Unsubscribe(ch);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = _conn.GetSubscriber();
            sub.UnsubscribeAll();
        }
        #endregion 发布订阅

        #region lock
        /// <summary>
        /// 获取Redis锁
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="token">令牌</param>
        /// <param name="expiry">过期时间(秒)，默认10s</param>
        /// <returns></returns>
        public bool GetLock(string lockName, string token, int expiry = 10)
        {
            lockName = AddPrefixKey(lockName);
            return GetDatabase().LockTake(lockName, token, TimeSpan.FromSeconds(expiry));
        }
        /// <summary>
        /// 获取Redis锁
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="token">令牌</param>
        /// <param name="expiry">过期时间(秒)，默认10s</param>
        /// <returns></returns>
        public async Task<bool> GetLockAsync(string lockName, string token, int expiry = 10)
        {
            lockName = AddPrefixKey(lockName);
            return await GetDatabase().LockTakeAsync(lockName, token, TimeSpan.FromSeconds(expiry));
        }

        /// <summary>
        /// 释放Redis锁
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        public bool ReleaseLock(string lockName, string token)
        {
            lockName = AddPrefixKey(lockName);
            return GetDatabase().LockRelease(lockName, token);
        }
        /// <summary>
        /// 释放Redis锁
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        public async Task<bool> ReleaseLockAsync(string lockName, string token)
        {
            lockName = AddPrefixKey(lockName);
            return await GetDatabase().LockReleaseAsync(lockName, token);
        }

        /// <summary>
        /// 使用Redis分布式锁执行某些操作
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="func">操作</param>
        /// <param name="expiry">锁过期时间，若超出时间自动解锁 单位：sec</param>
        /// <param name="retry">获取锁的重复次数</param>
        /// <param name="tryDelay">获取锁的重试间隔  单位：ms</param>
        public RedisLockResultModel<TResult> Lock<TResult>(string lockName, Func<TResult> func, int expiry = 10, int retry = 3, int tryDelay = 200)
        {
            //校验委托是否为异步
            if (func.Method.IsDefined(typeof(AsyncStateMachineAttribute), false))
            {
                throw new ArgumentException("使用异步Action请调用LockActionAsync");
            }

            //返回结果
            RedisLockResultModel<TResult> result = new();

            //锁名+前缀
            lockName = AddPrefixKey(lockName);
            //过期时间
            TimeSpan exp = TimeSpan.FromSeconds(expiry);
            //Redis锁令牌
            string token = Guid.NewGuid().ToString("N");

            try
            {
                bool ok = false;
                // 延迟重试
                for (int i = 0; i < retry; i++)
                {
                    if (GetDatabase().LockTake(lockName, token, exp))
                    {
                        ok = true;
                        break;
                    }
                    else
                    {
                        Task.Delay(tryDelay).Wait();
                    }
                }
                if (!ok)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "请求次数过多，请稍后再试!";
                    return result;
                }

                //执行委托
                try
                {
                    result.Data = func();
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = $"Redis分布式锁执行失败，原因:{ex.Message}";
                    NLogHelper.Error($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Redis分布式锁执行失败，原因:{ex.Message}", ex);
                }
            }
            finally
            {
                GetDatabase().LockRelease(lockName, token);
            }
            return result;
        }
        /// <summary>
        /// 使用Redis分布式锁执行某些异步操作
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="func">操作</param>
        /// <param name="expiry">锁过期时间，若超出时间自动解锁 单位：sec</param>
        /// <param name="retry">获取锁的重复次数</param>
        /// <param name="tryDelay">获取锁的重试间隔  单位：ms</param>
        public async Task<RedisLockResultModel<TResult>> LockAsync<TResult>(string lockName, Func<Task<TResult>> func, int expiry = 10, int retry = 3, int tryDelay = 200)
        {
            //返回结果
            RedisLockResultModel<TResult> result = new();

            //锁名+前缀
            lockName = AddPrefixKey(lockName);
            //过期时间
            TimeSpan exp = TimeSpan.FromSeconds(expiry);
            //Redis锁令牌
            string token = Guid.NewGuid().ToString("N");

            try
            {
                bool ok = false;
                // 延迟重试
                for (int i = 0; i < retry; i++)
                {
                    if (await GetDatabase().LockTakeAsync(lockName, token, exp))
                    {
                        ok = true;
                        break;
                    }
                    else
                    {
                        await Task.Delay(tryDelay);
                    }
                }
                if (!ok)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "请求次数过多，请稍后再试!";
                    return result;
                }

                //执行委托
                try
                {
                    result.Data = await func();
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = $"Redis分布式锁执行失败，原因:{ex.Message}";
                    NLogHelper.Error($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Redis分布式锁执行失败，原因:{ex.Message}", ex);
                }
            }
            finally
            {
                await GetDatabase().LockReleaseAsync(lockName, token);
            }
            return result;
        }
        #endregion

        #region 其他
        /// <summary>
        /// 创建事务
        /// </summary>
        /// <returns></returns>
        public ITransaction CreateTransaction()
        {
            return GetDatabase().CreateTransaction();
        }
        /// <summary>
        /// 获取Redis数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return _conn.GetDatabase(DATABASE_NUM);
        }
        /// <summary>
        /// 获取Redis服务
        /// </summary>
        /// <param name="hostAndPort"></param>
        /// <returns></returns>
        public IServer GetServer(string hostAndPort)
        {
            return _conn.GetServer(hostAndPort);
        }
        /// <summary>
        /// 获取前缀
        /// </summary>
        /// <returns></returns>
        public string GetPrefixKey()
        {
            return CUSTOMER_PREFIX_KEY;
        }
        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        public void SetPrefixKey(string customKey)
        {
            CUSTOMER_PREFIX_KEY = customKey;
        }
        /// <summary>
        /// 重置前缀
        /// </summary>
        public void ResetPrefixKey()
        {
            CUSTOMER_PREFIX_KEY = string.IsNullOrEmpty(CUSTOMER_PREFIX_KEY) ? _configuration.GetValue<string>("Redis.PrefixKey") : CUSTOMER_PREFIX_KEY;
        }
        /// <summary>
        /// 模糊查询keys
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public async Task<string[]> QueryKeysAsync(string pattern)
        {
            var redisResult = await _conn.GetDatabase(DATABASE_NUM).ScriptEvaluateAsync(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call(‘KEYS‘, @keypattern) " +
                " return res "), new { @keypattern = pattern });
            return (string[])redisResult;
        }
        #endregion 其他
    }
}
