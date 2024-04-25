using System;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Common.Attributes.CheckValidators
{
    /// <summary>
    /// 数字长度校验
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MaxNumberLengthAttribute : MaxLengthAttribute
    {
        /// <summary>
        /// 最大长度
        /// </summary>
        private readonly int _maxLength;
        /// <summary>
        /// 小数点后位数
        /// </summary>
        private readonly int _decimalPlaces;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="maxLength">最大长度(不包含小数点)</param>
        /// <param name="decimalPlaces">小数点后位数(默认2)</param>
        public MaxNumberLengthAttribute(int maxLength, int decimalPlaces = 2) : base(maxLength)
        {
            _maxLength = maxLength;
            _decimalPlaces = decimalPlaces;
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            // 如果value为null，不进行校验，返回true
            if (value is null)
            {
                return true;
            }

            // 最终比较字符串的长度
            string valueStr = "";

            try
            {
                // 整型
                if (value is byte || value is byte? || value is sbyte || value is sbyte? ||
                    value is int || value is int? || value is long || value is long?)
                {
                    valueStr = Convert.ToInt64(value).ToString();
                }
                // 浮点型
                else if (value is double || value is double? ||
                         value is float || value is float?)
                {
                    valueStr = Convert.ToDouble(value).ToString();

                    // 如果小数点后的位数超过限制，返回false
                    if (valueStr.Contains('.') && valueStr.Split(".")[1].Length > _decimalPlaces)
                    {
                        ErrorMessage = "小数点后位数过长!";
                        return false;
                    }

                    // 如果没有小数点，补全小数点
                    if (!valueStr.Contains('.'))
                    {
                        valueStr += ".".PadRight(_decimalPlaces + 1, '0'); // 拼接0在后面，+1表示.的长度，PadRight(总长度，拼接的字符)
                    }
                }
            }
            catch (Exception)
            {
                ErrorMessage = $"参数类型有误!";
                return false;
            }

            // 如果值的长度小于等于要求的长度，返回true
            // -1表示去除小数点的长度
            if ((valueStr.Length -1) <= _maxLength)
                return true;

            return false;
        }
    }
}
