using System;

namespace FastAdminAPI.Framework.Extensions.DbOperationExtensions
{
    /// <summary>
    /// 数据库操作映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbOperationFieldAttribute : Attribute
    {
        private readonly string _fieldName;
        private readonly bool _isCondition;
        private readonly bool _isAllowEmpty;

        /// <summary>
        /// 构造1
        /// </summary>
        /// <param name="fieldName">表字段名</param>
        public DbOperationFieldAttribute(string fieldName)
        {
            _fieldName = fieldName;
        }
        /// <summary>
        /// 构造2
        /// </summary>
        /// <param name="fieldName">表字段名</param>
        /// <param name="isCondition">是否为条件</param>
        public DbOperationFieldAttribute(string fieldName, bool isCondition)
        {
            _fieldName = fieldName;
            _isCondition = isCondition;
        }
        /// <summary>
        /// 构造3
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

        public string GetFieldName()
        {
            return _fieldName;
        }
        public bool IsCondition()
        {
            return _isCondition;
        }
        public bool IsAllowEmpty()
        {
            return _isAllowEmpty;
        }
    }
}
