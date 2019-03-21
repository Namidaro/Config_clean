using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UniconGS.UI.Picon2.ValidationRules
{
    public class Picon2ConfigIntValidationRules : ValidationRule
    {
        public Type ValidationType { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int _valAsInt;
            bool IsValid = false;
            if (value != null)
                _valAsInt = Convert.ToInt32(value);
            else
                _valAsInt = -1;

            if (_valAsInt >= 0 && _valAsInt <= 1000)
                IsValid = true;
            else
                IsValid = false;

            return IsValid ? new ValidationResult(true, null) : new ValidationResult(false, $"Число должно быть в пределах [0..1000]");


        }
    }
}
