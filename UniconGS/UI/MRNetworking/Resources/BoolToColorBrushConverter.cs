using System;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;

namespace UniconGS.UI
{
    public class BoolToColorBrushConverter:IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter,
           System.Globalization.CultureInfo culture)
        {
             
            var valueAsBool = value as bool?;
           
            if (valueAsBool.HasValue == false)
            {
                return Brushes.Gray;
            }
            else if (valueAsBool.Value == true)
            {
                return Brushes.Red;
            }
            else //if (valueAsBool.Value == false)
            {
                return Brushes.Green;
            }
        }
        ///    
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// 
        


        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
