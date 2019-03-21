using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UniconGS.UI.MRNetworking.Resources
{
   public class SplittingBitMarginConverter:IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                if ((int) value == 8)
                {
                    return new Thickness(1,1,20,1);
                }
                else
                {
                    return new Thickness(1);
                }
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
