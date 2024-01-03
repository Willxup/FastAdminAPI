using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FastAdminAPI.Common.Enums
{
    /// <summary>
    /// 枚举<see cref="Enum"/>的扩展辅助操作方法
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举项上的<see cref="DescriptionAttribute"/>特性的文字描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var member = type.GetMember(value.ToString()).FirstOrDefault();
            var descAttr = member.GetCustomAttribute<DescriptionAttribute>();
            return (member != null && descAttr != null) ? descAttr.Description : value.ToString();
        }
        /// <summary>
        /// 将值转换为枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static T ConvertToEnum<T>(int value) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            else
            {
                throw new ArgumentException($"The value {value} is not defined in the enum {typeof(T).Name}.");
            }
        }
    }
}
