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
using System.Windows.Shapes;

namespace UniconGS.UI.Settings
{
    /// <summary>
    /// Логика взаимодействия для DeviceSignatureWindow.xaml
    /// </summary>
    public partial class DeviceSignatureWindow : Window
    {
        public delegate void UpdateCommandDelegate();
        public event UpdateCommandDelegate UpdateEvent;

        public DeviceSignatureWindow()
        {
            InitializeComponent();
            this.uiDeviceSignatureText.Text = " ";
            uiUpdateDeviceButton.Visibility = Visibility.Hidden;
        }

        public DeviceSignatureWindow(string _text, bool _isUpdateable)
        {
            InitializeComponent();
            this.uiDeviceSignatureText.Text = _text;
            if (_isUpdateable)
                uiUpdateDeviceButton.Visibility = Visibility.Visible;
            else
                uiUpdateDeviceButton.Visibility = Visibility.Hidden;
        }

        private void uiUpdateDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDeviceCommand();
            this.Close();
        }

        private void uiOKButton_Click(object sender, RoutedEventArgs e)
        {
            OKCommand();
        }

        private void OKCommand()
        {
            this.Close();
        }

        private async void UpdateDeviceCommand()
        {
            try
            {
                await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x20, new ushort[] { 1 });
                RTUConnectionGlobal.CloseConnection();

                UpdateEvent?.Invoke();

            }
            catch { }
        }
    }
}
