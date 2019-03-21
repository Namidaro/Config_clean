using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UniconGS.UI.Configuration
{
    /// <summary>
    /// Логика взаимодействия для ErrorMaskMark.xaml
    /// </summary>
    public partial class ErrorMaskMark : UserControl
    {
        public ErrorMaskMark()
        {
            InitializeComponent();
        }


        public void ChannelOn()
        {
            uiErrorMaskMarkOn.Visibility = Visibility.Visible;
            uiErrorMaskMarkOff.Visibility = Visibility.Hidden;
        }
        public void ChannelOff()
        {
            uiErrorMaskMarkOn.Visibility = Visibility.Hidden;
            uiErrorMaskMarkOff.Visibility = Visibility.Visible;
        }
    }
}
