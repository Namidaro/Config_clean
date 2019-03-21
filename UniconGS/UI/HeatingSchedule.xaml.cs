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
    public class Date : INotifyPropertyChanged
    {
        private int _day = 1;
        private int _month = 1;


        public int DayNumber
        {
            get
            {
                return this._day;
            }
            set
            {
                this._day = value;
                onPropertyChanged("DayNumber");
            }
        }

        public int MonthNumber
        {
            get
            {
                return this._month;
            }
            set
            {
                this._month = value;
                onPropertyChanged("MonthNumber");
            }
        }

        public Date()
        {
            this.MonthNumber = 1;
            this.DayNumber = 1;
        }

        public Date(int month, int day)
        {
            this.DayNumber = day;
            this.MonthNumber = month;
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
            var turnOnTmp = BitConverter.GetBytes(value[1]);
            var turnOffTmp = BitConverter.GetBytes(value[0]);

            this.TurnOn = new Date(Convert.ToInt32(turnOnTmp[1]), Convert.ToInt32(turnOnTmp[0]));
            this.TurnOff = new Date(Convert.ToInt32(turnOffTmp[1]), Convert.ToInt32(turnOffTmp[0]));
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

    /// <summary>
    /// Interaction logic for HeatingSchedule.xaml
    /// </summary>
    public partial class HeatingSchedule : UserControl, IBaseControl
    {

        public void SetAutonomous()
        {
            this.uiExport.IsEnabled = false;
            this.uiImport.IsEnabled = false;
        }

        public void DisableAutonomous()
        {
            this.uiExport.IsEnabled = true;
            this.uiImport.IsEnabled = true;
        }

        #region Event
        public delegate void ShowMessageEventHandler(string message, string caption);
        public event ShowMessageEventHandler ShowMessage;

        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();

        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;
        #endregion

        #region Globals
        private Slot _querer = null;
        private Heating _value = new Heating();
        #endregion

        public Heating HeatingValue
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

        public HeatingSchedule()
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
                    this.ShowMessage("Чтение графика обогрева прошло успешно",
                        "Чтение графика обогрева");
                }
            }
            else
            {
                if (this.StopWork != null)
                    this.StopWork();
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время чтения графика обогрева из устройства произошла ошибка.",
                        "Чтение графика обогрева");
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
                    this.ShowMessage("Запись графика обогрева в устройство прошло успешно.",
                        "Запись графика обогреваа в устройство");
                }
            }
            else
            {
                if (this.StopWork != null)
                    this.StopWork();
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время записи графика обогрева в устройство произошла ошибка.",
                        "Запись графика обогрева в устройство");
                }
            }
        }

        public bool Export()
        {
            this._querer.Value = (this._value.GetValue() as Array).OfType<ushort>().ToArray();
            return DataTransfer.WriteWords(this._querer);
        }

        #region Privates
        private void UpdateBinding()
        {
            this.PART_TURNOFF.DataContext = this._value.TurnOff;
            this.PART_TURNON.DataContext = this._value.TurnOn;
        }
        #endregion

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
                    this._value.SetValue((value as Array).OfType<ushort>().ToArray());
                    this.UpdateBinding();
                }
                else
                {
                    this._value = new Heating();
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
