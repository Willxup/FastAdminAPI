using System;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    /// <summary>
    /// 数据库排序
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DbDefaultOrderByAttribute : Attribute
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        private readonly string _orderField;
        /// <summary>
        /// 排序规则
        /// </summary>
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

        /// <summary>
        /// 获取排序字段
        /// </summary>
        /// <returns></returns>
        public string GetOrderField()
        {
            return _orderField;
        }
        /// <summary>
        /// 获取排序规则
        /// </summary>
        /// <returns></returns>
        public DbSortWay GetSortWay()
        {
            return _sortWay;
        }
    }
}
