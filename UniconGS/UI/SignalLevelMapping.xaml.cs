using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniconGS.Interfaces;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for SignalLevelMapping.xaml
    /// </summary>
    public partial class SignalLevelMapping : UserControl
    {
        public SignalLevelMapping()
        {
            InitializeComponent();
        }

        public void UpdateState(int SignalLevel)
        {
            if (SignalLevel >= 0 && SignalLevel <= 5)
            {
                uiSignalLevel0.Fill = Brushes.Black;
                uiSignalLevel1.Fill = Brushes.Gainsboro;
                uiSignalLevel2.Fill = Brushes.Gainsboro;
                uiSignalLevel3.Fill = Brushes.Gainsboro;
                uiSignalLevel4.Fill = Brushes.Gainsboro;
                uiSignalLevel5.Fill = Brushes.Gainsboro;
            }
            if (SignalLevel >= 6 && SignalLevel <= 10)
            {
                uiSignalLevel0.Fill = Brushes.Black;
                uiSignalLevel1.Fill = Brushes.Black;
                uiSignalLevel2.Fill = Brushes.Gainsboro;
                uiSignalLevel3.Fill = Brushes.Gainsboro;
                uiSignalLevel4.Fill = Brushes.Gainsboro;
                uiSignalLevel5.Fill = Brushes.Gainsboro;
            }
            if (SignalLevel >= 11 && SignalLevel <= 15)
            {
                uiSignalLevel0.Fill = Brushes.Black;
                uiSignalLevel1.Fill = Brushes.Black;
                uiSignalLevel2.Fill = Brushes.Black;
                uiSignalLevel3.Fill = Brushes.Gainsboro;
                uiSignalLevel4.Fill = Brushes.Gainsboro;
                uiSignalLevel5.Fill = Brushes.Gainsboro;
            }
            if (SignalLevel >= 16 && SignalLevel <= 20)
            {
                uiSignalLevel0.Fill = Brushes.Black;
                uiSignalLevel1.Fill = Brushes.Black;
                uiSignalLevel2.Fill = Brushes.Black;
                uiSignalLevel3.Fill = Brushes.Black;
                uiSignalLevel4.Fill = Brushes.Gainsboro;
                uiSignalLevel5.Fill = Brushes.Gainsboro;
            }
            if (SignalLevel >= 20 && SignalLevel <= 25)
            {
                uiSignalLevel0.Fill = Brushes.Black;
                uiSignalLevel1.Fill = Brushes.Black;
                uiSignalLevel2.Fill = Brushes.Black;
                uiSignalLevel3.Fill = Brushes.Black;
                uiSignalLevel4.Fill = Brushes.Black;
                uiSignalLevel5.Fill = Brushes.Gainsboro;
            }
            if (SignalLevel >= 26 && SignalLevel <= 32)
            {
                uiSignalLevel0.Fill = Brushes.Black;
                uiSignalLevel1.Fill = Brushes.Black;
                uiSignalLevel2.Fill = Brushes.Black;
                uiSignalLevel3.Fill = Brushes.Black;
                uiSignalLevel4.Fill = Brushes.Black;
                uiSignalLevel5.Fill = Brushes.Black;
            }
            if (SignalLevel == 99)
            {
                uiSignalLevel0.Fill = Brushes.Gainsboro;
                uiSignalLevel1.Fill = Brushes.Gainsboro;
                uiSignalLevel2.Fill = Brushes.Gainsboro;
                uiSignalLevel3.Fill = Brushes.Gainsboro;
                uiSignalLevel4.Fill = Brushes.Gainsboro;
                uiSignalLevel5.Fill = Brushes.Gainsboro;
            }

        }
    }
}
