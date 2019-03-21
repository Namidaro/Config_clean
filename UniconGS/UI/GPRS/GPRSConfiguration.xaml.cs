using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using UniconGS.Source;

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
        private GPRSSettings _value = new GPRSSettings("", "", "", "");

        #endregion

        public GPRSConfiguration()
        {
            InitializeComponent();
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
        }

        public void DisableAutonomous()
        {
            this.uiExport.IsEnabled = true;
            this.uiImport.IsEnabled = true;
            this.uiAPN.IsEnabled = true;
            this.uiPassword.IsEnabled = true;
            this.uiUserName.IsEnabled = true;
        }

        private async void uiImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                uiImport.IsEnabled = uiExport.IsEnabled = false;
                List<ushort> ushorts = new List<ushort>();
                ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, 0x8000, 68));
                ushorts.AddRange (await RTUConnectionGlobal.GetDataByAddress(1, 0x8000+68, 60));
                uiImport.IsEnabled = uiExport.IsEnabled = true;
                ushort[] value = ushorts.ToArray();  
                ImportComplete(value);
                Value = value;
            }
            catch

            {
               
            }

        }

        private void ImportComplete(ushort[] value )
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
            await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x8000+68, Value.Skip(68).Take(60).ToArray());
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
