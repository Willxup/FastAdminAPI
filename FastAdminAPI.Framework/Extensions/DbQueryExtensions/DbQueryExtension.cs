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
                    if (prop.GetCustomAttributes(typeof(DbIgnoreFieldAttribute), true).Length > 0)
                    {
                        continue;
                    }
                    if (prop.GetCustomAttributes(typeof(DbQueryFieldAttribute), true).Length == 0)
                    {
                        continue;
                    }
                    if (prop.GetCustomAttributes(typeof(DbQueryOperatorAttribute), true).Length == 0)
                    {
                        continue;
                    }

                    var value = prop.GetValue(search);

                    if (value is null)
                    {
                        continue;
                    }
                    if (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
                    {
                        continue;
                    }

                    ConditionalModel condition = new();

                    bool isDateQuery = false; //是否为日期查询，用于string类型
                    string timeSuffix = "";

                    if (prop.IsDefined(typeof(DbTableAliasAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbTableAliasAttribute), true)[0] as DbTableAliasAttribute;
                        condition.FieldName += attr.GetTableAlias() + ".";
                    }

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

                    if (prop.IsDefined(typeof(DbQueryOperatorAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbQueryOperatorAttribute), true)[0] as DbQueryOperatorAttribute;
                        condition.ConditionalType = attr.GetDbOperator();
                    }

                    Type propType = prop.PropertyType;

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
                    else if (propType == typeof(DateTime) || propType == typeof(DateTime?))
                    {
                        condition.CSharpTypeName = typeof(DateTime)?.Name;
                        condition.FieldValue = ((DateTime)value).ToFormattedString();
                    }
                    else if (propType.IsPrimitive)
                    {
                        condition.CSharpTypeName = propType.Name;
                        condition.FieldValue = value + "";
                    }
                    else if (value is Array array)
                    {
                        condition.CSharpTypeName = propType.GetElementType()?.Name; //获取泛型类型
                        condition.FieldValue = string.Join(",", GetArrayElements(array));
                    }
                    else if (value is IEnumerable enumerable)
                    {
                        condition.CSharpTypeName = propType.GetGenericArguments()?[0]?.Name; //获取泛型类型
                        condition.FieldValue = string.Join(",", GetListElements(enumerable));
                    }
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
                    if (prop.GetCustomAttributes(typeof(DbIgnoreFieldAttribute), true).Length > 0)
                    {
                        continue;
                    }
                    if (prop.GetCustomAttributes(typeof(DbQueryFieldAttribute), true).Length == 0)
                    {
                        continue;
                    }

                    string sql = string.Empty;
                    if (prop.IsDefined(typeof(DbTableAliasAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbTableAliasAttribute), true)[0] as DbTableAliasAttribute;
                        sql += attr.GetTableAlias() + ".";
                    }

                    if (prop.IsDefined(typeof(DbQueryFieldAttribute), true))
                    {
                        var attr = prop.GetCustomAttributes(typeof(DbQueryFieldAttribute), true)[0] as DbQueryFieldAttribute;
                        sql += attr.GetFieldName();
                    }

                    if (!string.IsNullOrEmpty(sql))
                    {
                        sql += " AS " + prop.Name + ", ";
                        select.Append(sql);
                    }
                }
            }
            if(select.Length > 0)
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
            if (search.SortFields?.Count > 0)
            {
                var query = queryable;
                foreach (var item in search.SortFields)
                {
                    if(!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    {
                        if (DbCommonUtils.IsDbSortWay(item.Value))
                            throw new UserOperationException("排序方式错误!");

                        query = query.OrderBy($"{item.Key} {item.Value}");
                    }
                }
                return query;
            }
            else
            {
                object[] sort = search.GetType().GetCustomAttributes(typeof(DbDefaultOrderByAttribute), true);
                if(sort?.Length > 0)
                {
                    var query = queryable;
                    foreach(var item in sort)
                    {
                        var attr = item as DbDefaultOrderByAttribute;
                        query = query.OrderBy($"{attr.GetOrderField()} {attr.GetSortWay()}");
                    }
                }
            }
            return queryable;
        }
        #endregion
    }
}
