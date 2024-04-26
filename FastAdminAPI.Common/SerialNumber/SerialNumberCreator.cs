using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Datetime;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using System;
using System.Text;

namespace FastAdminAPI.CommonSerialNumber
{
    /// <summary>
    /// 编号生成器
    /// </summary>
    public static class SerialNumberCreator
    {
        private const string BILL_REDIS_KEY = "Redis.Bills.Key";
        private const string BILL_RECORD_REDIS_KEY = "Redis.BillRecords.Key";

        #region  内部方法  
        /// <summary>
        /// 获取交易日流水编号
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="transactionKey">交易日流水RedisKey</param>
        /// <returns></returns>
        private static string GetTransactiontNo(IRedisHelper redis, string transactionKey)
        {
            string date = DateTime.Now.ToString("yyMMdd");
            var key = $"{transactionKey}{date}";
            long num;
            if (redis.KeyExists(key))
            {
                num = Convert.ToInt64(redis.StringIncrement(key));
            }
            else
            {
                num = 1;
                redis.StringSet(key, num, TimeSpan.FromSeconds(DateTool.GetRemainingTimeOfDay()));
            }
            long result = num / 10;
            if (result == 0)//个位数
            {
                return $"000{num}";
            }
            else if (result > 0 && result < 10) //2位数
            {
                return $"00{num}";
            }
            else if (result >= 10 && result < 100)//三位数
            {
                return $"0{num}";
            }
            else
            {
                return num.ToString();
            }
        }
        /// <summary>
        /// 获取两位0-9的随机数
        /// </summary>
        /// <returns></returns>
        private static string GetRandomNo()
        {
            Random random = new();
            return random.Next(0, 100) + "";
        }
        /// <summary>
        /// Id参数格式化
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string IdFormat(long id)
        {
            long result = id / 10;
            if (result == 0)//个位数
            {
                return $"000{id}";
            }
            else if (result > 0 && result < 10) //2位数
            {
                return $"00{id}";
            }
            else if (result >= 10 && result < 100) //3位数
            {
                return $"0{id}";
            }
            else
            {
                return id.ToString();
            }
        }
        #endregion

        /// <summary>
        /// 创建账单编号 RB + 产品(0001) + yyMMdd + 日流水(0001) + 随机数(00)
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="productId">产品Id</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public static string CreateBillNo(IRedisHelper redis,  long productId)
        {
            try
            {
                StringBuilder builder = new();
                builder.Append("RB");
                builder.Append(IdFormat(productId));
                builder.Append(DateTime.Now.ToString("yyMMdd"));
                builder.Append(GetTransactiontNo(redis, BILL_REDIS_KEY));
                builder.Append(GetRandomNo());
                return builder.ToString();
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"创建应收账单编号失败:{ex.Message}", ex);
                throw new UserOperationException($"创建应收账单编号失败!");
            }
        }
        /// <summary>
        /// 创建账单记录编号 PD + 产品(0001) + yyMMddHHmmssfff + 日流水(0001) + 随机数(00)
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="productId">产品Id</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public static string CreateBillRecordNo(IRedisHelper redis, long productId)
        {
            try
            {
                StringBuilder builder = new();
                builder.Append("PD");
                builder.Append(IdFormat(productId));
                builder.Append($"{DateTime.Now.ToString("yyMMddHHmmssfff")}");
                builder.Append(GetTransactiontNo(redis, BILL_RECORD_REDIS_KEY));
                builder.Append(GetRandomNo());
                return builder.ToString();
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"创建账单记录编号:{ex.Message}", ex);
                throw new UserOperationException($"创建账单记录编号!");
            }
        }
    }
}
