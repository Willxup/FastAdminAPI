using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Converters;
using FastAdminAPI.Common.SystemUtilities;
using SqlSugar;
using System;
using System.Linq;

namespace FastAdminAPI.Framework.Extensions
{
    /// <summary>
    /// 数据库工具类
    /// </summary>
    public static class DbCommonUtils
    {
        /// <summary>
        /// 统一配置sqlsugar
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static SqlSugarScope ConfigSqlSugar(string connectionString)
        {
            StaticConfig.EnableAllWhereIF = true;
            return new SqlSugarScope(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true, //自动释放
                LanguageType = LanguageType.English
            }, ConfigSqlSugarSetting);
        }
        /// <summary>
        /// 配置sqlsugar设置
        /// </summary>
        /// <param name="db"></param>
        public static void ConfigSqlSugarSetting(SqlSugarClient db)
        {
            db.CurrentConnectionConfig.InitKeyType = InitKeyType.Attribute;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings() { IsAutoRemoveDataCache = true };
            db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
            {
                SqlFuncServices = DbExtension.CustomSqlFunc()
            };
            if (EnvironmentHelper.IsDevelop)
            {
                db.Aop.OnLogExecuting = (sql, parameters) =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(DateTime.Now.ToFormattedString());
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("【SQL】：");
                    Console.ResetColor();
                    Console.WriteLine(sql);
                    if (parameters != null && parameters?.Length > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(DateTime.Now.ToFormattedString());
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("【Paramters】：");
                        Console.ResetColor();
                        Console.WriteLine(string.Join(",", parameters?.Select(it => "【" + it.ParameterName + "=" + it.Value + "】")));
                    }
                };
            }
        }
        /// <summary>
        /// 检查数据库字段名是否为空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public static string CheckDbFieldName(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            else
            {
                throw new UserOperationException("数据库字段名不能为空!");
            }
        }
        /// <summary>
        /// 是否为数据库排序方式
        /// </summary>
        /// <param name="sortway"></param>
        /// <returns></returns>
        public static bool IsDbSortWay(string sortway)
        {
            if(sortway.ToUpper() == DbSortWay.ASC.ToString() || sortway.ToUpper() == DbSortWay.DESC.ToString())
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 时间后缀类型
    /// </summary>
    public enum DbTimeSuffixType
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        StartTime,
        /// <summary>
        /// 结束时间
        /// </summary>
        EndTime
    }
    /// <summary>
    /// 数据库操作符
    /// </summary>
    public enum DbOperator
    {
        /// <summary>
        /// 等于 =
        /// </summary>
        Equal,
        /// <summary>
        /// Like
        /// </summary>
        Like,
        /// <summary>
        /// 大于 >
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于等于 >=
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于 <
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于等于 <=
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// IN
        /// </summary>
        In,
        /// <summary>
        /// NOT IN
        /// </summary>
        NotIn,
        /// <summary>
        /// LIKE LEFT
        /// </summary>
        LikeLeft,
        /// <summary>
        /// LIKE RIGHT
        /// </summary>
        LikeRight,
        /// <summary>
        /// 不等于 !=
        /// </summary>
        NoEqual,
        /// <summary>
        /// 字符串是为null或空
        /// </summary>
        IsNullOrEmpty,
        /// <summary>
        /// IS NOT
        /// </summary>
        IsNot,
        /// <summary>
        /// Not LIKE
        /// </summary>
        NoLike,
        /// <summary>
        /// IS NULL
        /// </summary>
        EqualNull,
        /// <summary>
        /// IN LIKE
        /// </summary>
        InLike
    }
    /// <summary>
    /// 数据库排序方式
    /// </summary>
    public enum DbSortWay
    {
        /// <summary>
        /// 正序
        /// </summary>
        ASC,
        /// <summary>
        /// 倒序
        /// </summary>
        DESC
    }
}
