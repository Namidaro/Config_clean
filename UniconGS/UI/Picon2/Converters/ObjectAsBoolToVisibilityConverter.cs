using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UniconGS.UI.Picon2.Converters
{
  
    public class ObjectAsBoolToVisibilityConverter : IValueConverter
    {
    
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? valueAsBool = value as bool?;
            if (!valueAsBool.HasValue)
            {
                return Visibility.Collapsed;
            }
            else if (valueAsBool.Value == true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     NotImplementedException
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is Visibility)
            {
                var valueAsVisibility = value as Visibility?;
                if (valueAsVisibility.Equals(Visibility.Visible))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
