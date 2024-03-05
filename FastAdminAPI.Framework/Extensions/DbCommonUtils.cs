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
    }
}
