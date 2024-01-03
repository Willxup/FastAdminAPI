using System;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Common.Attributes.CheckValidators
{
    /// <summary>
    /// 枚举校验 只支持int类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EnumCheckAttribute : RequiredAttribute
    {
        private readonly Type _enumType;
        private readonly bool _IsAllowEmpty;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="enumType">枚举类</param>
        /// <param name="IsAllowEmpty">是否允许为空 默认否</param>
        public EnumCheckAttribute(Type enumType, bool IsAllowEmpty = false)
        {
            _enumType = enumType;
            _IsAllowEmpty = IsAllowEmpty;
        }
        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return _IsAllowEmpty;
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
