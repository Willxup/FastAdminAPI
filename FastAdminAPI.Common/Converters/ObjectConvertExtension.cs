using System;

namespace FastAdminAPI.Common.Converters
{
    public static class ObjectConvertExtension
    {
        #region object重载
        /// <summary>
        /// 对象转长整型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static long ToLong(this object thisValue)
        {
            if (thisValue == null) return 0;
            if (thisValue != null && thisValue != DBNull.Value && long.TryParse(thisValue.ToString(), out long reval))
            {
                return reval;
            }
            return 0;
        }
        ///<summary>
        ///对象转长整型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<param name="defaultValue"></param>
        ///<returns></returns>
        public static long ToLong(this object thisValue, long defaultValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && long.TryParse(thisValue.ToString(), out long reval))
            {
                return reval;
            }
            return defaultValue;
        }
        ///<summary>
        ///对象转整型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<returns></returns>
        public static int ToInt(this object thisValue)
        {
            if (thisValue == null) return 0;
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out int reval))
            {
                return reval;
            }
            return 0;
        }
        ///<summary>
        ///对象转整型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<param name="defaultValue"></param>
        ///<returns></returns>
        public static int ToInt(this object thisValue, int defaultValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out int reval))
            {
                return reval;
            }
            return defaultValue;
        }
        ///<summary>
        ///对象转浮点型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<returns></returns>
        public static double ToDouble(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out double reval))
            {
                return reval;
            }
            return 0;
        }
        ///<summary>
        ///对象转浮点型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<param name="defaultValue"></param>
        ///<returns></returns>
        public static double ToDouble(this object thisValue, double defaultValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out double reval))
            {
                return reval;
            }
            return defaultValue;
        }
        ///<summary>
        ///对象转字符型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<returns></returns>
        public static string ToString(this object thisValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return "";
        }
        ///<summary>
        ///对象转字符型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<param name="defaultValue"></param>
        ///<returns></returns>
        public static string ToString(this object thisValue, string defaultValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return defaultValue;
        }
        ///<summary>
        ///对象转浮点型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<returns></returns>
        public static decimal ToDecimal(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out decimal reval))
            {
                return reval;
            }
            return 0;
        }
        ///<summary>
        ///对象转浮点型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<param name="defaultValue"></param>
        ///<returns></returns>
        public static decimal ToDecimal(this object thisValue, decimal defaultValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out decimal reval))
            {
                return reval;
            }
            return defaultValue;
        }
        ///<summary>
        ///对象转日期型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<returns>默认返回当前时间</returns>
        public static DateTime ToDate(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out DateTime reval))
            {
                return reval;
            }
            return DateTime.Now;
        }
        ///<summary>
        ///对象转日期型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<param name="defaultValue">默认当前日期</param>
        ///<returns></returns>
        public static DateTime? ToDate(this object thisValue, DateTime? defaultValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out DateTime reval))
            {
                return reval;
            }
            return defaultValue;
        }
        ///<summary>
        ///对象转布尔型
        ///</summary>
        ///<param name="thisValue"></param>
        ///<returns></returns>
        public static bool ToBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        } 
        #endregion

        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ObjectIsNullOrEmpty<T>(T data)
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
    }
}
