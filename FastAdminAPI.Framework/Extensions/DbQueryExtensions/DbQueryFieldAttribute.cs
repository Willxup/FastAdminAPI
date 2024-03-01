using System;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    /// <summary>
    /// 数据库查询表字段名
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DbQueryFieldAttribute : DbQueryAttribute
    {
        /// <summary>
        /// 表字段名
        /// </summary>
        private readonly string _fieldName;
        /// <summary>
        /// 是否为时间查询
        /// </summary>
        private readonly bool _isDateQuery = false;
        /// <summary>
        /// 时间前缀类型(开始/结束)
        /// </summary>
        private readonly DbTimeSuffixType _suffixType;
        /// <summary>
        /// 时间前缀
        /// </summary>
        private readonly string _timeSuffix = "";
        /// <summary>
        /// 结果是否为布尔值(查询结果)
        /// </summary>
        private readonly bool _isBoolValue = false;
        /// <summary>
        /// 当结果为布尔值时的true值(查询结果)
        /// </summary>
        private readonly int? _trueValue = null;

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
        /// <param name="fieldName">表字段名称</param>
        /// <param name="isBoolValue">是否为布尔值</param>
        /// <param name="trueValue">true值(当数据库与传入值相同时为TRUE)[注：数据库如果[0否1是]可直接转换，使用上面的构造即可]</param>
        public DbQueryFieldAttribute(string fieldName, bool isBoolValue, int trueValue)
        {
            _fieldName = fieldName;
            _isBoolValue = isBoolValue;
            _trueValue = trueValue;
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fieldName">表字段名</param>
        /// <param name="suffixType">日期的时间后缀类型(仅用于查询)，开始/结束</param>
        /// <param name="timeSuffix">日期的时间后缀(HH:mm:ss)，以修改默认时间拼接，默认拼接(00:00:00/23:59:59)</param>
        public DbQueryFieldAttribute(string fieldName, DbTimeSuffixType suffixType, string timeSuffix = "")
        {
            _isDateQuery = true;
            _fieldName = DbCommonUtils.CheckDbFieldName(fieldName);
            _suffixType = suffixType;
            _timeSuffix = timeSuffix;
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
        /// 是否为布尔值
        /// </summary>
        /// <returns></returns>
        public bool IsBoolValue()
        {
            return _isBoolValue;
        }
        /// <summary>
        /// 获取True值
        /// </summary>
        /// <returns></returns>
        public int? GetTrueValue()
        {
            return _trueValue;
        }
        /// <summary>
        /// 是否为时间查询
        /// </summary>
        /// <returns></returns>
        public bool IsDateQuery()
        {
            return _isDateQuery;
        }
        /// <summary>
        /// 获取时间前缀类型
        /// </summary>
        /// <returns></returns>
        public DbTimeSuffixType GetTimeSuffixType()
        {
            return _suffixType;
        }
        /// <summary>
        /// 获取时间前缀
        /// </summary>
        /// <returns></returns>
        public string GetTimeSuffix()
        {
            return _timeSuffix;
        }
    }
}
