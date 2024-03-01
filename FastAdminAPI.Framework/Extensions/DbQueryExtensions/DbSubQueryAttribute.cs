using System;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    /// <summary>
    /// 数据库查询SELEC子查询
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbSubQueryAttribute : DbQueryAttribute
    {
        /// <summary>
        /// 子查询
        /// </summary>
        private readonly string _subQuery;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="subQuery">子查询</param>
        public DbSubQueryAttribute(string subQuery)
        {
            _subQuery = subQuery;
        }

        /// <summary>
        /// 获取子查询
        /// </summary>
        /// <returns></returns>
        public string GetSubQuery()
        {
            return _subQuery;
        }
    }
}
