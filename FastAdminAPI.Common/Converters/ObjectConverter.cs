using System;

namespace FastAdminAPI.Common.Converters
{
    public static class ObjectConverter
    {
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
