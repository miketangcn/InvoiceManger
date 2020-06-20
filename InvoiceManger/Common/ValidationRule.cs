using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InvoiceManger.Common
{
    public class RequiredRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "该字段不能为空值！");
            if (string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, "该字段不能为空字符串！");
            return new ValidationResult(true, null);
        }

    }
    public class InvoiceSnRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex invoiceSNReg = new Regex(@"^[0-9]{8}$");
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false,"发票号不能为空");
            }
            else if (!invoiceSNReg.IsMatch(value.ToString()))
            {
                return new ValidationResult(false,"发票号格式错误");
            }
            return new ValidationResult(true, null);
        }
    }
    public class InvoiceCodeRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex invoiceCodeReg = new Regex(@"^[0-9]{11,13}$");
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "发票编码不能为空");
            }
            else if (!invoiceCodeReg.IsMatch(value.ToString()))
            {
                return new ValidationResult(false, "发票编码格式错误");
            }
            return new ValidationResult(true, null);
        }
    }
    public class PasswordRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex passwordReg = new Regex(@"^[A-Za-z0-9]{6,12}$");
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "密码不能为空");
            }
            else if (!passwordReg.IsMatch(value.ToString()))
            {
                return new ValidationResult(false, "密码格式为6-12位字母和数字的组合");
            }
            return new ValidationResult(true, null);
        }
    }
    public class AmountRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex amountReg = new Regex(@"^([1-9][0-9]*)+(.[0-9]{1,2})?$");
            if (!amountReg.IsMatch(value.ToString()))
            {
                return new ValidationResult(false, "请输入有效的发票金额！");
            }
            return new ValidationResult(true, null);
        }
    }

    public class EmailRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex emailReg = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");

            if (!String.IsNullOrEmpty(value.ToString()))
            {
                if (!emailReg.IsMatch(value.ToString()))
                {
                    return new ValidationResult(false, "邮箱地址不准确！");
                }
            }
            return new ValidationResult(true, null);
        }
    }
}
