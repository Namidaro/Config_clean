using System;
using System.Windows.Data;

namespace UniconGS.Converters
{
    public class IndexToValueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return System.Convert.ToInt32(value) - 1;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return System.Convert.ToInt32(value) + 1;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
