using System;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    /// <summary>
    /// 数据库查询表字段名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbQueryFieldAttribute : DbQueryAttribute
    {
        private readonly string _fieldName;
        private readonly bool _isDate = false;
        private readonly DbTimeSuffixType _suffixType;
        private readonly string _timeSuffix = "";

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fieldName">表字段名</param>
        public DbQueryFieldAttribute(string fieldName)
        {
            _fieldName = DbCommonUtils.CheckDbFieldName(fieldName);
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fieldName">表字段名</param>
        /// <param name="suffixType">日期的时间后缀类型(仅用于查询)，开始/结束</param>
        /// <param name="timeSuffix">日期的时间后缀(HH:mm:ss)，以修改默认时间拼接，默认拼接(00:00:00/23:59:59)</param>
        public DbQueryFieldAttribute(string fieldName, DbTimeSuffixType suffixType, string timeSuffix = "")
        {
            _isDate = true;
            _fieldName = DbCommonUtils.CheckDbFieldName(fieldName);
            _suffixType = suffixType;
            _timeSuffix = timeSuffix;
        }
        public string GetFieldName()
        {
            return _fieldName;
        }
        public bool IsDateQuery()
        {
            return _isDate;
        }
        public DbTimeSuffixType GetTimeSuffixType()
        {
            return _suffixType;
        }
        public string GetTimeSuffix()
        {
            return _timeSuffix;
        }
    }
}
