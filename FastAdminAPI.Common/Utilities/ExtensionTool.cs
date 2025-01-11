using System;
using System.Collections.Generic;

namespace FastAdminAPI.Common.Utilities
{
    /// <summary>
    /// 扩展工具
    /// </summary>
    public static class ExtensionTool
    {
        #region T
        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ObjectIsNullOrEmpty<T>(this T data)
            where T : class, new()
        {
            //是否为空 默认为空
            bool isNullOrEmpty = true;

            //获取类型的所有属性
            var properties = data.GetType().GetProperties();
            if (properties?.Length > 0)
            {
                foreach (var property in properties)
                {
                    //获取属性的值
                    var value = property.GetValue(data);

                    //判断值是否为null
                    if (value is null)
                    {
                        continue;
                    }
                    //判断值是否为字符串且字符串为空
                    else if (value is string str && string.IsNullOrWhiteSpace(str))
                    {
                        continue;
                    }
                    //其他情况
                    else
                    {
                        isNullOrEmpty = false;
                        break;
                    }
                }
            }

            return isNullOrEmpty;
        }
        #endregion

        #region List<T>
        /// <summary>
        /// 列表是否为空或null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            bool isNullOrEmpty = false;

            // linq方式
            //if (!list?.Any() ?? true)
            //    isNullOrEmpty = true;

            if (list == null || list?.Count <= 0)
                isNullOrEmpty = true;

            return isNullOrEmpty;
        }
        /// <summary>
        /// 数组是否为空或null
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this Array array)
        {
            bool isNullOrEmpty = false;

            if (array == null || array?.Length <= 0)
                isNullOrEmpty = true;

            return isNullOrEmpty;
        }
        #endregion
    }
}
