using System;

namespace FastAdminAPI.Framework.Extensions.DbOperationExtensions
{
    /// <summary>
    /// 数据库操作映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbOperationFieldAttribute : Attribute
    {
        /// <summary>
        /// 表字段名
        /// </summary>
        private readonly string _fieldName;
        /// <summary>
        /// 是否为更新条件
        /// </summary>
        private readonly bool _isCondition;
        /// <summary>
        /// 是否允许为空
        /// </summary>
        private readonly bool _isAllowEmpty;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fieldName">表字段名</param>
        public DbOperationFieldAttribute(string fieldName)
        {
            _fieldName = fieldName;
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fieldName">表字段名</param>
        /// <param name="isCondition">是否为条件</param>
        public DbOperationFieldAttribute(string fieldName, bool isCondition)
        {
            _fieldName = fieldName;
            _isCondition = isCondition;
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fieldName">表字段名</param>
        /// <param name="isCondition">是否为条件</param>
        /// <param name="isAllowEmpty">是否允许更新成null(默认不允许)</param>
        public DbOperationFieldAttribute(string fieldName, bool isCondition, bool isAllowEmpty)
        {
            _fieldName = fieldName;
            _isCondition = isCondition;
            _isAllowEmpty = isAllowEmpty;
        }

        /// <summary>
        /// 获取表字段名
        /// </summary>
        /// <returns></returns>
        public string GetFieldName()
        {
            return _fieldName;
        }
        /// <summary>
        /// 是否为条件
        /// </summary>
        /// <returns></returns>
        public bool IsCondition()
        {
            return _isCondition;
        }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        /// <returns></returns>
        public bool IsAllowEmpty()
        {
            return _isAllowEmpty;
        }
    }
}
