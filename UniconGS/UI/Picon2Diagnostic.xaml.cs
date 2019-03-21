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

namespace UniconGS.UI
{
    /// <summary>
    /// Логика взаимодействия для Picon2Diagnostic.xaml
    /// </summary>
    public partial class Picon2Diagnostic : UserControl
    {
        public Picon2Diagnostic()
        {
            InitializeComponent();
        }
        public async Task Update()
        {


            ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0200, 5);
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetValue(value);
            });
        }

        private void SetValue(ushort[] value)
        {

        }
    }
}
