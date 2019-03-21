using System;
using System.Windows;
using System.Windows.Data;

namespace UniconGS.Converters
{
    public class BooleanReverseConverter : IValueConverter
    {
        #region IValueConverter Members
        // какая-то херня была, взял из скады
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? valueAsBool = value as bool?;
            if (!valueAsBool.HasValue)
            {
                return !(bool)valueAsBool;
            }
            else if (valueAsBool.Value == true)
            {
                return !(bool)valueAsBool;
            }
            else
            {
                return !(bool)valueAsBool;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                var valueAsBool = value as bool?;
                if (valueAsBool.Equals(true))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
