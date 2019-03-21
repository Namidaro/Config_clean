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
using System.Threading;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for Time.xaml
    /// </summary>
    public partial class Time : UserControl, IBaseControl
    {
        #region Nastied types
        public class InnerTime
        {
            public int? Year { get; set; }
            public int? Month { get; set; }
            public int? Day { get; set; }
            public int? Hour { get; set; }
            public int? Minute { get; set; }
            public int? Second { get; set; }

            public InnerTime(int? year, int? month, int? day, int? hour, int? minute, int? second)
            {
                this.Day = day;
                this.Hour = hour;
                this.Minute = minute;
                this.Month = month;
                this.Second = second;
                this.Year = year;
            }
        }
        #endregion

        #region Events
        public delegate InnerTime WriteTimeEventHandler(DateTime curPLCDateTime);
        public event WriteTimeEventHandler WriteTime;
        #endregion

        #region Globals
        private DateTime[] _value = new DateTime[2];
        private Slot _querer;
        private BackgroundWorker _worker = new BackgroundWorker();
        public AutoResetEvent doneEvent = new AutoResetEvent(false);
        #endregion

        public void SetAutonomous()
        {
            this.uiChangeTime.IsEnabled = false;
            this.uiLocalDate.Content = "Неизвестно";
            this.uiLocalTime.Content = "Неизвестно";
            this.uiRealTime.Content = "Неизвестно";
            this.uiRealDate.Content = "Неизвестно";
            this.uiSystemTime.IsEnabled = false;
        }

        public void DisableAutonomous()
        {
            this.uiChangeTime.IsEnabled = true;
            this.uiSystemTime.IsEnabled = true;
        }

        public Time()
        {
            InitializeComponent();
        }
        private void SetDefault()
        {
            this.uiLocalDate.Content = "Ошибка";
            this.uiLocalDate.ToolTip = "Ошибка при попытке чтения из устройства";
            this.uiLocalTime.Content = "Ошибка";
            this.uiLocalTime.ToolTip = "Ошибка при попытке чтения из устройства";
            this.uiRealTime.Content = "Ошибка";
            this.uiRealTime.ToolTip = "Ошибка при попытке чтения из устройства";
            this.uiRealDate.Content = "Ошибка";
            this.uiRealDate.ToolTip = "Ошибка при попытке чтения из устройства";
        }

        #region IBaseControl Members

        public void Update()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = false;
            _worker.WorkerSupportsCancellation = true;
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            _worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            _worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool res = DataTransfer.ReadWords(ref this._querer);
            e.Result = res;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                doneEvent.Set();
            }
            else
            {
                if (Convert.ToBoolean(e.Result))
                {
                    this.Value = this._querer.Value;
                }
                else
                {
                    this.Value = null;
                }
                doneEvent.Set();
            }
        }

        public object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (value != null && value is Array)
                {


                    if ((value as Array).Length == 16)
                    {
                        var realTime = (value as Array).OfType<ushort>().ToList().GetRange(0, 8);
                        var localTime = (value as Array).OfType<ushort>().ToList().GetRange(8, 8);
                        this._value = new DateTime[2];
                        try
                        {
                            DateTime dtR = new DateTime(Convert.ToInt32(realTime[0]), Convert.ToInt32(realTime[1]),
                                Convert.ToInt32(realTime[2]), Convert.ToInt32(realTime[4]),
                                Convert.ToInt32(realTime[5]), Convert.ToInt32(realTime[6]));
                            this.uiRealDate.Content =dtR.Day + "/" + dtR.Month + "/" + dtR.Year;
                            this.uiRealTime.Content = ((dtR.Hour < 10) == true ? ("0" + dtR.Hour).ToString() : (dtR.Hour.ToString())).ToString() + ":" + ((dtR.Minute < 10) == true ? ("0" + dtR.Minute).ToString() : (dtR.Minute.ToString())).ToString() + ":" + ((dtR.Second < 10 == true) ? ("0" + dtR.Second) : (dtR.Second.ToString()));

                            this._value[0] = dtR;
                        }
                        catch (Exception e)
                        {
                            this.uiRealDate.Content = "Ошибка";
                            this.uiRealDate.ToolTip = "Не верный формат даты";
                            this.uiRealTime.Content = "Ошибка";
                            this.uiRealTime.ToolTip = "Не верный формат времени";
                        }
                        try
                        {
                            DateTime dtL = new DateTime(Convert.ToInt32(localTime[0]), Convert.ToInt32(localTime[1]),
                            Convert.ToInt32(localTime[2]), Convert.ToInt32(localTime[4]),
                            Convert.ToInt32(localTime[5]), Convert.ToInt32(localTime[6]));

                            this.uiLocalDate.Content = dtL.Day + "/" + dtL.Month + "/" + dtL.Year;
                            this.uiLocalTime.Content = ((dtL.Hour < 10) == true ? ("0" + dtL.Hour).ToString() : (dtL.Hour.ToString())).ToString() + ":" + ((dtL.Minute < 10) == true ? ("0" + dtL.Minute).ToString() : (dtL.Minute.ToString())).ToString() + ":" + ((dtL.Second < 10 == true) ? ("0" + dtL.Second) : (dtL.Second.ToString()));
                            this._value[1] = dtL;
                        }
                        catch (Exception e)
                        {
                            this.uiLocalDate.Content = "Ошибка";
                            this.uiLocalDate.ToolTip = "Не верный формат даты";
                            this.uiLocalTime.Content = "Ошибка";
                            this.uiLocalTime.ToolTip = "Не верный формат времени";
                        }
                    }
                }
                else
                {
                    this.SetDefault();
                }

            }

        }


        public bool WriteContext()
        {
            throw new NotImplementedException();
        }
        #endregion

        private void uiChangeTime_Click(object sender, RoutedEventArgs e)
        {
            if (this.WriteTime != null)
            {
                var timeToWrite = this.WriteTime(this._value[1]);
                if (timeToWrite != null && this.StartUpdate != null && this.StopUpdate != null)
                {
                    var tmp = this._value;
                    this._value[1] = new DateTime(timeToWrite.Year == null ? tmp[1].Year : Convert.ToInt32(timeToWrite.Year),
                    timeToWrite.Month == null ? tmp[1].Month : Convert.ToInt32(timeToWrite.Month),
                    timeToWrite.Day == null ? tmp[1].Day : Convert.ToInt32(timeToWrite.Day),
                    timeToWrite.Hour == null ? tmp[1].Hour : Convert.ToInt32(timeToWrite.Hour),
                    timeToWrite.Minute == null ? tmp[1].Minute : Convert.ToInt32(timeToWrite.Minute),
                    timeToWrite.Second == null ? tmp[1].Second : Convert.ToInt32(timeToWrite.Second));
                    this.StopUpdate();
                    this._querer.Value = new ushort[8] 
                        {
                            Convert.ToUInt16(this._value[1].Year),
                            Convert.ToUInt16(this._value[1].Month),
                            Convert.ToUInt16(this._value[1].Day),
                            Convert.ToUInt16((int)this._value[1].DayOfWeek),
                            Convert.ToUInt16(this._value[1].Hour),
                            Convert.ToUInt16(this._value[1].Minute),
                            Convert.ToUInt16(this._value[1].Second),
                            Convert.ToUInt16(this._value[1].Millisecond)
                        };
                    var writtingSlot = (Slot)this._querer.Clone();
                    writtingSlot.Start = (ushort)(writtingSlot.Start + 8);
                    writtingSlot.Size = (ushort)8;
                    this.CancelUpdate();
                    DataTransfer.WriteWords(this._querer);
                    this.Update();
                    this.StartUpdate();
                }
            }
        }

        private void uiSystemTime_Click(object sender, RoutedEventArgs e)
        {
            this._value[1] = DateTime.Now;
            this._querer.Value = new ushort[8] 
            {
                Convert.ToUInt16(this._value[1].Year),
                Convert.ToUInt16(this._value[1].Month),
                Convert.ToUInt16(this._value[1].Day),
                Convert.ToUInt16((int)this._value[1].DayOfWeek),
                Convert.ToUInt16(this._value[1].Hour),
                Convert.ToUInt16(this._value[1].Minute),
                Convert.ToUInt16(this._value[1].Second),
                Convert.ToUInt16(this._value[1].Millisecond)
            };
            var writtingSlot = (Slot)this._querer.Clone();
            writtingSlot.Start = (ushort)(writtingSlot.Start + 8);
            writtingSlot.Size = (ushort)8;

            if (this.StopUpdate != null && this.StartUpdate != null)
            {
                this.StopUpdate();
                this.CancelUpdate();
                DataTransfer.WriteWords(writtingSlot);
                this.Update();
                this.StartUpdate();

            }
        }

        #region IBaseControl Members

        public event Commands.StartUpdateEventHandler StartUpdate;

        public event Commands.StopUpdateEventHandler StopUpdate;

        #endregion

        #region IBaseControl Members


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

        #endregion

        #region IBaseControl Members


        public void CancelUpdate()
        {
            if (_worker.WorkerSupportsCancellation && _worker.IsBusy)
                _worker.CancelAsync();
        }

        #endregion
    }
}
