using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using UniconGS.Source;
using UniconGS.Enums;
using System.Threading.Tasks;

namespace UniconGS.UI.GPRS
{
    /// <summary>
    /// Interaction logic for GPRSConfiguration.xaml
    /// </summary>
    public partial class GPRSConfiguration
    {


        #region Events

        public delegate void ShowMessageEventHandler(string message, string caption, MessageBoxImage image);

        public event ShowMessageEventHandler ShowMessage;

        public delegate void StartWorkEventHandler();

        public delegate void StopWorkEventHandler();

        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;

        private delegate void ReadComplete(ushort[] res);

        private delegate void WriteComplete(bool res);

        #endregion

        #region Globals

        private Slot _querer = null;
        private GPRSSettings _value = new GPRSSettings("", "", "", "", false);

        #endregion

        public GPRSConfiguration()
        {
            InitializeComponent();
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON_GS)
            {
                CheckPiconGSVersion();
            }

            this.UpdateBinding();
        }

        public GPRSSettings Settings
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                this.UpdateBinding();
            }
        }



        public void SetAutonomous()
        {
            this.uiExport.IsEnabled = false;
            this.uiImport.IsEnabled = false;
            this.uiAPN.IsEnabled = false;
            this.uiPassword.IsEnabled = false;
            this.uiUserName.IsEnabled = false;
            this.uiGSMSyncCheckBox.IsEnabled = false;
        }

        public void DisableAutonomous()
        {
            this.uiExport.IsEnabled = true;
            this.uiImport.IsEnabled = true;
            this.uiAPN.IsEnabled = true;
            this.uiPassword.IsEnabled = true;
            this.uiUserName.IsEnabled = true;
            this.uiGSMSyncCheckBox.IsEnabled = true;
        }

        private async void CheckPiconGSVersion()
        {
            bool _GSMVisible = await GetPiconGSSignature();
            if (_GSMVisible)
                this.uiGSMSyncCheckBox.Visibility = Visibility.Visible;
        }

        private async Task<bool> GetPiconGSSignature()
        {
            try
            {
                ushort[] res = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x0400), 20);
                return ReadSignatureComplete(res);
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        private bool ReadSignatureComplete(ushort[] value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return GetSignatureString((value).ToList());
            }
        }
        private bool GetSignatureString(List<ushort> value)
        {
            ushort[] deviceName = value.GetRange(0, 5).ToArray();
            //убираем лишний символ
            deviceName[4] = (ushort)(LOBYTE(deviceName[4]));
            ushort[] version = value.GetRange(8, 3).ToArray();

            var devName = "Имя устройства: " + Converter.GetStringFromWords(deviceName) + ";\r\n";

            return CheckVersionUpdateAvailability(
                Converter.GetStringFromWords(deviceName),
                (byte)(version[1] >> 8),
                ((byte)version[1])
                                                );
        }

        /// <summary>
        /// Изменение последовательности байт с преобразованием в слова
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public static ushort TOWORD(byte high, byte low)
        {
            UInt16 ret = (UInt16)high;
            return (ushort)((ushort)(ret << 8) + (ushort)low);
        }
        /// <summary>
        /// Возвращает младший байт слова
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Мл.байт</returns>
        public static byte LOBYTE(int v)
        {
            return (byte)(v & 0xff);
        }
        /// <summary>
        /// Возвращает старший байт слова.
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Ст.байт</returns>
        public static byte HIBYTE(int v)
        {
            return (byte)(v >> 8);
        }

        private bool CheckVersionUpdateAvailability(string deviceName, byte versionFirstByte, byte versionSecondByte)
        {
            if (!deviceName.ToLower().Contains("gs")) return false;

            if (Convert.ToInt16(versionFirstByte) > 5) return true;
            else if (Convert.ToInt16(versionFirstByte) == 5)
            {
                if (Convert.ToInt16(versionSecondByte) > 0) return true;
                else return false;
            }
            else return false;

        }

        private async void uiImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                uiImport.IsEnabled = uiExport.IsEnabled = false;
                List<ushort> ushorts = new List<ushort>();
                ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, 0x8000, 68));
                ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, 0x8000 + 68, 60));

                ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, 0x8080, 1));

                uiImport.IsEnabled = uiExport.IsEnabled = true;
                ushort[] value = ushorts.ToArray();
                ImportComplete(value);
                Value = value;
            }
            catch

            {

            }

        }

        private void ImportComplete(ushort[] value)
        {
            if (value != null)
            {

                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Чтение конфигурации GPRS модема прошло успешно",
                        "Чтение конфигурации GPRS модема", MessageBoxImage.Information);
                }
            }
            else
            {
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время чтения конфигурации GPRS модема из устройства произошла ошибка.",
                        "Чтение конфигурации GPRS модема", MessageBoxImage.Error);
                }
            }
            uiImport.IsEnabled = uiExport.IsEnabled = true;
        }
        private async void uiExport_Click(object sender, RoutedEventArgs e)
        {
            uiImport.IsEnabled = uiExport.IsEnabled = false;
            await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x8000, Value.Skip(0).Take(68).ToArray());
            await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x8000 + 68, Value.Skip(68).Take(60).ToArray());
            await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x8080, new ushort[] { Value.Last() });
            ExportComplete(true);
        }

        public void ExportComplete(bool res)
        {
            if (res)
            {
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Запись конфигурации GPRS модема в устройство прошла успешно.",
                        "Запись конфигурации GPRS модема в устройство", MessageBoxImage.Information);
                }

            }
            else
            {

                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время записи конфигурации GPRS модема в устройство произошла ошибка.",
                        "Запись конфигурации GPRS модема в устройство", MessageBoxImage.Error);
                }



            }
            uiImport.IsEnabled = uiExport.IsEnabled = true;
        }

        private void UpdateBinding()
        {
            this.uiMain.DataContext = this._value; ;
        }


        #region IQueryMember

        public ushort[] Value
        {
            get
            {
                return this._value.GetValue();
            }
            set
            {
                if (value != null)
                {
                    this._value.SetValue(value);
                    this.UpdateBinding();
                }
                else
                {
                    this._value = new GPRSSettings();
                    this.UpdateBinding();
                }
            }
        }



        #endregion


    }
}
