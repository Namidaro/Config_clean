using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UniconGS.Source;
using UniconGS.Enums;


namespace UniconGS
{
    /// <summary>
    /// Логика взаимодействия для GSMConnection.xaml
    /// </summary>
    public partial class GSMConnection : Window
    {
        public const string r = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
        Regex reg = new Regex(r, RegexOptions.Compiled);

        public GSMConnection()
        {
            InitializeComponent();
            ResulGSM = new ResultGSM();
            uiApply.Click += uiApply_Click;
            uiSettingsCancel.Click += uiSettingsCancel_Click;
            uiiPTex.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(ip_PreviewTextImput);
            uiiPTex.PreviewKeyUp += UiiPTex_PreviewKeyUp;

            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
            {
                uiPortNumberGSM.Text = "502";
            }
            else
            {
                uiPortNumberGSM.Text = "4444";
            }


            if (ConfiguratorSettings.Default.ipSettings == null)
            {
                uiiPTex.Text = "127.0.0.1";
            }
            else
            {
                uiiPTex.Text = ConfiguratorSettings.Default.ipSettings;
            }

        }

        private void UiiPTex_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            Check();
        }
        private void Check()
        {
            uiApply.IsEnabled = false;


            if (reg.IsMatch(uiiPTex.Text))
            {
                //e.Handled = true;
                uiApply.IsEnabled = true;
            }


        }

        private void ip_PreviewTextImput(object sender, TextCompositionEventArgs e)
        {
            Check();
        }

        public ResultGSM ResulGSM { get; set; }



        private void uiApply_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!Validator.ValidateTextBox(uiReadTimeout, 1, 10000))
                {
                    MessageBox.Show("Не верно задано время ожидания чтения", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    throw new ArgumentException();
                }
                if (!Validator.ValidateTextBox(uiWriteTimeout, 1, 10000))
                {
                    MessageBox.Show("Не верно задано время ожидания записи", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    throw new ArgumentException();
                }
                if (!Validator.ValidateTextBox(uiRetries, 0, 5))
                {
                    MessageBox.Show("Не верно задано значение повторов", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    throw new ArgumentException();
                }
                if (!Validator.ValidateTextBox(uiWaitUntilRetry, 0, 1000))
                {
                    MessageBox.Show("Не верно задано значение задержки повторов", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    throw new ArgumentException();
                }
                ResultGSM.IPAdress = uiiPTex.Text;
                ResultGSM.PortNumber = int.Parse(uiPortNumberGSM.Text);
                ConfiguratorSettings.Default.ipSettings = uiiPTex.Text;
                ConfiguratorSettings.Default.Save();
                
                ResultGSM.ModbusReadTimeout= int.Parse(uiReadTimeout.Text);
                ResultGSM.ModbusWriteTimeout= int.Parse(uiWriteTimeout.Text);
                ResultGSM.ModbusRetries= int.Parse(uiRetries.Text);
                ResultGSM.ModbusWaitUntilRetry= int.Parse(uiWaitUntilRetry.Text);

                DialogResult = true;
                Close();
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Введено не верное значение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           


        }
        private void uiSettingsCancel_Click(object sender, RoutedEventArgs e)
        {
            ResultGSM.PortNumber = 0;
            ResultGSM.IPAdress = string.Empty;
            this.Close();
        }

        public class ResultGSM
        {
            public ResultGSM()

            {
                PortNumber = 0;
                IPAdress = String.Empty;

                ModbusReadTimeout = 0;
                ModbusWriteTimeout = 0;
                ModbusRetries = 0;
                ModbusWaitUntilRetry = 0;
            }


            public static int PortNumber { get; set; }
            public static string IPAdress { get; set; }

            public static int ModbusReadTimeout { get; set; }
            public static int ModbusWriteTimeout { get; set; }
            public static int ModbusRetries { get; set; }
            public static int ModbusWaitUntilRetry { get; set; }
        }


    }

}
