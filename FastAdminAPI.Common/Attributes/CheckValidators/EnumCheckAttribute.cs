using System;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Common.Attributes.CheckValidators
{
    /// <summary>
    /// 枚举校验 只支持数字类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EnumCheckAttribute : RequiredAttribute
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        private readonly Type _enumType;
        /// <summary>
        /// 是否允许为空
        /// </summary>
        private readonly bool _isAllowEmpty;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="enumType">枚举类</param>
        /// <param name="isAllowEmpty">是否允许为空 默认否</param>
        public EnumCheckAttribute(Type enumType, bool isAllowEmpty = false)
        {
            _enumType = enumType;
            _isAllowEmpty = isAllowEmpty;
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return _isAllowEmpty;
            }
            if (!_enumType.IsEnum)
            {
                ErrorMessage = "该类型不是枚举类型!";
                return false;
            }
            try
            {
                if (Enum.IsDefined(_enumType, Convert.ToInt32(value)))
                {
                    return true;
                }
                else
                {
                    ErrorMessage = $"请输入正确的值!";
                    return false;
                }
            }
            catch (InvalidOperationException)
            {
                //System.InvalidOperationException：
                //value is not type System.SByte, System.Int16, System.Int32, System.Int64, 
                //System.Byte,System.UInt16, System.UInt32, or System.UInt64, or System.String
                ErrorMessage = $"不支持的传参类型!";
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"参数类型错误, {ex.Message}";
                return false;
            }

        }
    }
}
