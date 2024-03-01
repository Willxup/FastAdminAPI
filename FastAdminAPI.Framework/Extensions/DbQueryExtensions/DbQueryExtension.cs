using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Converters;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    public static class DbQueryExtension
    {

        #region 私有方法
        /// <summary>
        /// 获取Array的所有元素
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static string[] GetArrayElements(Array array)
        {
            string[] elements = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                elements[i] = array.GetValue(i).ToString();
            }
            return elements;
        }
        /// <summary>
        /// 获取List的所有元素
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetListElements(IEnumerable list)
        {
            foreach (var item in list)
            {
                yield return item.ToString();
            }
        }
        /// <summary>
        /// 获取查询条件参数
        /// </summary>
        /// <typeparam name="TSearch"></typeparam>
        /// <param name="search"></param>
        /// <returns></returns>
        private static List<IConditionalModel> GetWhereParameters<TSearch>(this TSearch search)
        {
            List<IConditionalModel> conditions = new();

            var props = search.GetType().GetProperties();
            if (props?.Length > 0)
            {
                foreach (var prop in props)
                {
                    //校验是否为忽略字段
                    if (prop.GetCustomAttributes(typeof(DbIgnoreFieldAttribute), true).Length > 0)
                    {
                        continue;
                    }
                    //校验是否标记查询
                    if (prop.GetCustomAttributes(typeof(DbQueryFieldAttribute), true).Length == 0)
                    {
                        continue;
                    }
                    //校验是否多次标记
                    if (prop.GetCustomAttributes(typeof(DbQueryOperatorAttribute), true).Length == 0)
                    {
                        continue;
                    }

                    //获取属性值
                    var value = prop.GetValue(search);

                    //null校验
                    if (value is null)
                    {
                        continue;
                    }
                    //空字符串校验
                    if (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
                    {
                        continue;
                    }

                    ConditionalModel condition = new();

                    //是否为日期查询，用于string类型
                    bool isDateQuery = false; 
                    string timeSuffix = "";

                    //表别名
                    if (prop.IsDefined(typeof(DbTableAliasAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbTableAliasAttribute), true)[0] as DbTableAliasAttribute;
                        condition.FieldName += attr.GetTableAlias() + ".";
                    }

                    //表字段名
                    if (prop.IsDefined(typeof(DbQueryFieldAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbQueryFieldAttribute), true)[0] as DbQueryFieldAttribute;
                        condition.FieldName += attr.GetFieldName();

                        //日期查询
                        if (attr.IsDateQuery())
                        {
                            isDateQuery = true;
                            if (attr.GetTimeSuffixType() == DbTimeSuffixType.StartTime)
                            {
                                timeSuffix = !string.IsNullOrWhiteSpace(attr.GetTimeSuffix()) ? attr.GetTimeSuffix() : "00:00:00";
                            }
                            else if (attr.GetTimeSuffixType() == DbTimeSuffixType.EndTime)
                            {
                                timeSuffix = !string.IsNullOrWhiteSpace(attr.GetTimeSuffix()) ? attr.GetTimeSuffix() : "23:59:59";
                            }
                        }
                    }

                    //操作符
                    if (prop.IsDefined(typeof(DbQueryOperatorAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbQueryOperatorAttribute), true)[0] as DbQueryOperatorAttribute;
                        condition.ConditionalType = attr.GetDbOperator();
                    }

                    //字段类型
                    Type propType = prop.PropertyType;

                    //字符串
                    if (propType == typeof(string))
                    {
                        condition.CSharpTypeName = typeof(string)?.Name;
                        condition.FieldValue = (string)value;

                        //日期查询
                        if (isDateQuery)
                        {
                            condition.FieldValue += $" {timeSuffix}";
                        }
                    }
                    //时间
                    else if (propType == typeof(DateTime) || propType == typeof(DateTime?))
                    {
                        condition.CSharpTypeName = typeof(DateTime)?.Name;
                        condition.FieldValue = ((DateTime)value).ToFormattedString();
                    }
                    //基础类型
                    else if (propType.IsPrimitive)
                    {
                        condition.CSharpTypeName = propType.Name;
                        condition.FieldValue = value + "";
                    }
                    //数组
                    else if (value is Array array)
                    {
                        condition.CSharpTypeName = propType.GetElementType()?.Name; //获取泛型类型
                        condition.FieldValue = string.Join(",", GetArrayElements(array));
                    }
                    //集合
                    else if (value is IEnumerable enumerable)
                    {
                        condition.CSharpTypeName = propType.GetGenericArguments()?[0]?.Name; //获取泛型类型
                        condition.FieldValue = string.Join(",", GetListElements(enumerable));
                    }
                    //其他类型
                    else
                    {
                        condition.CSharpTypeName = propType.Name;
                        condition.FieldValue = value.ToString();
                    }

                    conditions.Add(condition);
                }
            }

            return conditions;
        }
        /// <summary>
        /// 获取查询结果参数SQL
        /// </summary>
        /// <typeparam name="TResultModel"></typeparam>
        /// <param name="search"></param>
        /// <returns></returns>
        private static string GetSelectSQL<TResultModel>(this TResultModel search)
        {
            StringBuilder select = new();

            var props = search.GetType().GetProperties();

            if (props?.Length > 0)
            {
                foreach (var prop in props)
                {
                    //校验是否为忽略字段
                    if (prop.GetCustomAttributes(typeof(DbIgnoreFieldAttribute), true).Length > 0)
                    {
                        continue;
                    }
                    //校验是否标记查询
                    if (prop.GetCustomAttributes(typeof(DbQueryAttribute), true).Length == 0)
                    {
                        continue;
                    }
                    //校验是否多次标记
                    if (prop.GetCustomAttributes(typeof(DbQueryAttribute), true).Length > 1)
                    {
                        throw new UserOperationException("[DbQueryFieldAttribute]和[DbSubQueryAttribute]不能同时使用!");
                    }

                    string sql = string.Empty;
                    
                    //表别名
                    if (prop.IsDefined(typeof(DbTableAliasAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbTableAliasAttribute), true)[0] as DbTableAliasAttribute;
                        if (prop.IsDefined(typeof(DbSubQueryAttribute), true))
                        {
                            throw new UserOperationException("使用[DbSubQueryAttribute]子查询时，请去除[DbTableAliasAttribute]!");
                        }

                        sql += "`" + attr.GetTableAlias() + "`" + ".";
                    }

                    //表字段名
                    if (prop.IsDefined(typeof(DbQueryFieldAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbQueryFieldAttribute), true)[0] as DbQueryFieldAttribute;

                        string fieldName = attr.GetFieldName();

                        if (fieldName.ToUpper().Contains("SELECT") || fieldName.ToUpper().Contains("FROM") || fieldName.ToUpper().Contains("WHERE"))
                        {
                            throw new UserOperationException("请使用[DbSubQueryAttribute]子查询!");
                        }

                        //如果为布尔值，转换为布尔值
                        if (attr.IsBoolValue() && attr.GetTrueValue() != null)
                        {
                            sql = "IF(" + sql + "`" + fieldName + "`" + $" = {attr.GetTrueValue()}, " + "TRUE, FALSE)";
                        }
                        else
                        {
                            sql += "`" + fieldName + "`";
                        }
                    }

                    //子查询
                    if (prop.IsDefined(typeof(DbSubQueryAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbSubQueryAttribute), true)[0] as DbSubQueryAttribute;

                        sql += "(" + attr.GetSubQuery() + ")";
                    }

                    //表字段或子查询 别名
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sql += " AS " + prop.Name + ", ";

                        select.Append(sql);
                    }
                }
            }
            if (select.Length > 0)
            {
                return select.Remove(select.Length - 2, 2).ToString();
            }
            return string.Empty;
        }
        #endregion

        #region Where
        /// <summary>
        /// 自动拼接查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static ISugarQueryable<T> Where<T, TSearch>(this ISugarQueryable<T> queryable, TSearch search)
        {
                return queryable.Where(search.GetWhereParameters());
        }
        #endregion

        #region Select
        /// <summary>
        /// 自动拼接查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ISugarQueryable<TResult> Select<T, TResult>(this ISugarQueryable<T> queryable, TResult result)
        {
            return queryable.Select<TResult>(result.GetSelectSQL());
        }
        #endregion

        #region OrderBy
        /// <summary>
        /// 自动拼接排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSearch"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static ISugarQueryable<T> OrderBy<T, TSearch>(this ISugarQueryable<T> queryable, TSearch search)
            where TSearch : DbQueryPagingModel
        {
            //指定排序
            if (search.SortFields?.Count > 0)
            {
                foreach (var item in search.SortFields)
                {
                    if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    {
                        if (DbCommonUtils.IsDbSortWay(item.Value))
                            throw new UserOperationException("排序方式错误!");

                        queryable = queryable.OrderBy($"{item.Key} {item.Value}");
                    }
                }
            }
            //默认排序(特性指定)
            else
            {
                object[] sort = search.GetType().GetCustomAttributes(typeof(DbDefaultOrderByAttribute), true);
                
                if (sort?.Length > 0)
                {
                    foreach (var item in sort)
                    {
                        var attr = item as DbDefaultOrderByAttribute;

                        queryable = queryable.OrderBy($"{attr.GetOrderField()} {attr.GetSortWay()}");
                    }
                }
            }

            return queryable;
        }
        #endregion
    }
}
