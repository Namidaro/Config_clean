using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UniconGS.Source;

namespace UniconGS.UI.GPRS
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
}
