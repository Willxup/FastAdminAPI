using System;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    /// <summary>
    /// 数据库查询SELEC子查询
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbSubQueryAttribute : DbQueryAttribute
    {
        private readonly string _subQuery;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="subQuery">子查询</param>
        public DbSubQueryAttribute(string subQuery)
        {
            _subQuery = subQuery;
        }

        public string GetSubQuery()
        {
            return _subQuery;
        }
    }
}
