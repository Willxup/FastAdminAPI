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
        private bool IsAllowEmpty;
        public PhoneCheckAttribute(bool _IsAllowEmpty = false, string ErrorMessage = "手机号码输入错误")
        {
            base.ErrorMessage = ErrorMessage;
            IsAllowEmpty = _IsAllowEmpty;
        }
        public override bool IsValid(object value)
        {
            if (value is null)
            {
                //不允许为空抛出异常
                if (!IsAllowEmpty)
                    throw new UserOperationException(ErrorMessage);
                else
                    return true;
            }
            Type type = value.GetType();
            if (type == typeof(string))
            {
                string phone = value.ToString();
                if (phone.Length != 11)
                    throw new UserOperationException("请输入11位正确的手机号码！");
                if (!Regex.IsMatch(phone, @"^1\d{10}$"))
                    throw new UserOperationException("手机号码格式错误,请检查！");

            }
            else
                throw new UserOperationException("数据类型错误");
            return true;
        }
    }
}
