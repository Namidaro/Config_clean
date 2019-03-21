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
using UniconGS.Source;
using System.ComponentModel;

namespace UniconGS.UI
{
    [Serializable]
    public class GPRSSettings : INotifyPropertyChanged
    {
        private string _userName = string.Empty;
        private string _userPassword = string.Empty;
        private string _callingNumber = string.Empty;
        private string _apnSettings = string.Empty;

       

        public string UserName
        {
            get
            {
                return this._userName;
            }
            set
            {
                if (!this._userName.Equals(value))
                {
                    this._userName = value;
                    this.OnPropertyChanged("UserName");

                }

            }
        }

        public string UserPassword
        {
            get
            {
                return this._userPassword;
            }
            set
            {
                if (!this._userPassword.Equals(value))
                {
                    this._userPassword = value;
                    this.OnPropertyChanged("UserPassword");

                }
            }
        }

        public string CallingNumber
        {
            get
            {
                return this._callingNumber;
            }
            set
            {
                if (!this._callingNumber.Equals(value))
                {
                    this._callingNumber = value;
                    this.OnPropertyChanged("CallingNumber");

                }
            }
        }

        public string APNSettings
        {
            get
            {
                return this._apnSettings;
            }
            set
            {
                if (!this._apnSettings.Equals(value))
                {
                    this._apnSettings = value;
                    this.OnPropertyChanged("APNSettings");

                }
            }
        }

        public GPRSSettings()
        {
            this._apnSettings = string.Empty;
            this._callingNumber = string.Empty;
            this._userName = string.Empty;
            this._userPassword = string.Empty;
        }

        public GPRSSettings(string userName, string userPassword, string callingNumber, string apnSettings)
        {
            this.UserPassword = userPassword;
            this.UserName = userName;
            this.CallingNumber = callingNumber;
            this.APNSettings = apnSettings;
        }

        #region Get/Set data

        public void SetValue(ushort[] value)
        {
            /*Максимальная длина строки 64 символа!!!! или 32 ushort*/
            this.APNSettings = Converter.GetStringFromWords(value.ToList().GetRange(0, 32).ToArray());
            this.CallingNumber = Converter.GetStringFromWords(value.ToList().GetRange(32, 32).ToArray());
            this.UserName = Converter.GetStringFromWords(value.ToList().GetRange(64, 32).ToArray());
            this.UserPassword = Converter.GetStringFromWords(value.ToList().GetRange(96, 32).ToArray());
        }

        public ushort[] GetValue()
        {
            /*Максимальная длина строки 64 символа!!!! или 32 ushort*/
            List<ushort> tmp = new List<ushort>();
            tmp.AddRange(Converter.GetWordsFromString(this.APNSettings, 32));
            tmp.AddRange(Converter.GetWordsFromString(this.CallingNumber, 32));
            tmp.AddRange(Converter.GetWordsFromString(this.UserName, 32));
            tmp.AddRange(Converter.GetWordsFromString(this.UserPassword, 32));
            return tmp.ToArray();
        }
        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string fieldName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(fieldName));
            }
        }
        #endregion
    }

    /// <summary>
    /// Interaction logic for GPRSConfiguration.xaml
    /// </summary>
    public partial class GPRSConfiguration : UserControl, IBaseControl
    {
        #region Events
        public delegate void ShowMessageEventHandler(string message, string caption);
        public event ShowMessageEventHandler ShowMessage;

        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();

        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;
        #endregion


        #region Globals
        private Slot _querer = null;
        private GPRSSettings _value = new GPRSSettings("", "", "", "");
        #endregion

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

        public GPRSConfiguration()
        {
            InitializeComponent();
            this.uiExport.Click += new RoutedEventHandler(uiExport_Click);
            this.uiImport.Click += new RoutedEventHandler(uiImport_Click);
            this.UpdateBinding();
        }

        private void uiImport_Click(object sender, RoutedEventArgs e)
        {
            if (this.StartWork != null)
                this.StartWork();
            var result = DataTransfer.ReadWords(ref this._querer);
            if (result)
            {
                this.Value = this._querer.Value;
                if (this.StopWork != null)
                    this.StopWork();
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Чтение конфигурации GPRS модема прошло успешно",
                        "Чтение конфигурации GPRS модема");
                }
            }
            else
            {
                if (this.StopWork != null)
                    this.StopWork();
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время чтения конфигурации GPRS модема из устройства произошла ошибка.",
                        "Чтение конфигурации GPRS модема");
                }
            }
        }

        private void uiExport_Click(object sender, RoutedEventArgs e)
        {
            if (this.StartWork != null)
                this.StartWork();
            if (this.Export())
            {
                if (this.StopWork != null)
                    this.StopWork();
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Запись конфигурации GPRS модема в устройство прошло успешно.",
                            "Запись конфигурации GPRS модема в устройство");
                }
            }
            else
            {
                if (this.StopWork != null)
                    this.StopWork();
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время записи конфигурации GPRS модема в устройство произошла ошибка.",
                                "Запись конфигурации GPRS модема в устройство");
                }
            }
        }

        public bool Export()
        {
            this._querer.Value = (this._value.GetValue() as Array).OfType<ushort>().ToArray();
            return DataTransfer.WriteWords(this._querer);
        }

        private void UpdateBinding()
        {
            this.uiMain.DataContext = this._value;

            //this.uiAPN.DataContext = this._value;
            //this.uiCallingNumber.DataContext = this._value;
            //this.uiPassword.DataContext = this._value;
            //this.uiUserName.DataContext = this._value;
        }


        #region IBaseControl Members

        public event Commands.StartUpdateEventHandler StartUpdate;

        public event Commands.StopUpdateEventHandler StopUpdate;

        public Slot Querer
        {
            get
            {
                return this._querer;
            }
            set
            {
                this._querer = value;
            }
        }

        public object Value
        {
            get
            {
                return this._value.GetValue();
            }
            set
            {
                if (value != null)
                {
                    if (value is Array)
                    {
                        this._value.SetValue((value as Array).OfType<ushort>().ToArray());
                        this.UpdateBinding();
                    }
                    else
                    {
                        this._value = new GPRSSettings();
                        this.UpdateBinding();
                    }
                }
                else
                {
                    this._value = new GPRSSettings();
                    this.UpdateBinding();
                }
            }
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public bool WriteContext()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBaseControl Members


        public void CancelUpdate()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
