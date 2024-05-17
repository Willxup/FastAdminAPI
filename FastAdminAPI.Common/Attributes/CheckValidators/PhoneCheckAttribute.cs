using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FastAdminAPI.Common.Attributes.CheckValidators
{
    /// <summary>
    /// 手机号码校验
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PhoneCheckAttribute : ValidationAttribute
    {
        /// <summary>
        /// 是否允许为空
        /// </summary>
        private readonly bool _isAllowEmpty;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="isAllowEmpty">是否允许为空(默认false)</param>
        /// <param name="errorMessage">错误信息</param>
        public PhoneCheckAttribute(bool isAllowEmpty = false, string errorMessage = "手机号码输入错误")
        {
            ErrorMessage = errorMessage;
            _isAllowEmpty = isAllowEmpty;
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public override bool IsValid(object value)
        {
            if (value is null)
            {
                //不允许为空抛出异常
                if (!_isAllowEmpty)
                    throw new UserOperationException(ErrorMessage);
                else
                    return true;
            }

            Type type = value.GetType();

            if (type == typeof(string))
            {
                string phone = value.ToString();

                if (phone.Length != 11)
                {
                    ErrorMessage = "请输入11位正确的手机号码!";
                    return false;
                }

                if (!Regex.IsMatch(phone, @"^1\d{10}$"))
                {
                    ErrorMessage = "手机号码格式错误,请检查!";
                    return false;
                }

            }
            else
                return false;

            return true;
        }
    }
}
