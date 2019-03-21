using System;
using System.ComponentModel;

namespace UniconGS.UI.HeatingSchedule
{
    [Serializable]
    public class Heating : INotifyPropertyChanged
    {
        private Date _turnOn = new Date();
        private Date _turnOff = new Date();

        public Date TurnOn
        {
            get
            {
                return this._turnOn;
            }
            set
            {
                this._turnOn = value;
                onPropertyChanged("TurnOn");
            }
        }
        public Date TurnOff
        {
            get
            {
                return this._turnOff;
            }
            set
            {
                this._turnOff = value;
                onPropertyChanged("TurnOff");
            }
        }

        public Heating(Date turnOn, Date turnOff)
        {
            this.TurnOn = turnOn;
            this.TurnOff = turnOff;
        }

        public Heating()
        {
            this.TurnOff = new Date();
            this.TurnOn = new Date();
        }
        public ushort[] GetValue()
        {
            return new ushort[2]
            {
                BitConverter.ToUInt16(
                    new byte[2]
                    {
                        Convert.ToByte(this.TurnOff.DayNumber),
                        Convert.ToByte(this.TurnOff.MonthNumber)
                    }, 0),
                BitConverter.ToUInt16(
                    new byte[2]
                    {
                        Convert.ToByte(this.TurnOn.DayNumber),
                        Convert.ToByte(this.TurnOn.MonthNumber)
                    }, 0)
            };
        }
        public void SetValue(ushort[] value)
        {
            var turnOnTmp = BitConverter.GetBytes(value[1]); //1
            var turnOffTmp = BitConverter.GetBytes(value[0]);

            this.TurnOn = new Date(Convert.ToInt32(turnOnTmp[1]), Convert.ToInt32(turnOnTmp[0])); //1
            this.TurnOff = new Date(Convert.ToInt32(turnOffTmp[1]), Convert.ToInt32(turnOffTmp[0]));//1 
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void onPropertyChanged(string fieldName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(fieldName));
            }
        }
        #endregion
    }
}
