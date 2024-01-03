using System;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    /// <summary>
    /// 数据库排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DbDefaultOrderByAttribute : Attribute
    {
        private readonly string _orderField;
        private readonly DbSortWay _sortWay;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="orderField">排序字段</param>
        /// <param name="sortWay">排序方式</param>
        public DbDefaultOrderByAttribute(string orderField, DbSortWay sortWay)
        {
            _orderField = orderField;
            _sortWay = sortWay;
        }


        public string GetOrderField()
        {
            return _orderField;
        }
        public DbSortWay GetSortWay()
        {
            return _sortWay;
        }
    }
}
