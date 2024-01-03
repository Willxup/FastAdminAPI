using System;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    /// <summary>
    /// 数据库表别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbTableAliasAttribute : Attribute
    {
        private readonly string _tableAlias;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        public DbTableAliasAttribute(string tableAlias)
        {
            _tableAlias = DbCommonUtils.CheckDbFieldName(tableAlias);
        }

        public string GetTableAlias()
        {
            return _tableAlias;
        }
    }
}
