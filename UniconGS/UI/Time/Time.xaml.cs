using System;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
using NModbus4.Device;
using UniconGS.Annotations;
using UniconGS.Interfaces;
using UniconGS.Source;
using UniconGS.UI.Schedule;
using UniconGS.Enums;
using System.Threading;

namespace UniconGS.UI.Time
{
    /// <summary>
    /// Interaction logic for Time.xaml
    /// </summary>
    public partial class Time : UserControl, IUpdatableControl
    {
        #region Events&Delegates

        public delegate InnerTime WriteTimeEventHandler(DateTime curPLCDateTime);

        public delegate void ShowMessageEventHandler(string message, string caption, MessageBoxImage image);

        public event ShowMessageEventHandler ShowMessage;

        private delegate void ReadComplete(ushort[] res);

        private delegate void WriteComplete(bool res);

        #endregion

        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        private DateTime[] _clock = new DateTime[2];

        private ushort[] _value;
        private bool _isSystemTime;

        public ushort[] timeUshorts;
        public Time()
        {
            InitializeComponent();
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
        }

        #region IQueryMember

        public async Task Update()

        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {

                try
                {

                    ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x2100, 8);
                    if (value == null)
                        return;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetTimeForPicon2(value);
                        uiChangeTime.IsEnabled = uiSystemTime.IsEnabled = true;
                    });
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x1000, 16);
                Application.Current.Dispatcher.Invoke(() =>
                        {
                            SetTime(value);
                            uiChangeTime.IsEnabled = uiSystemTime.IsEnabled = true;
                        });
            }
            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}

        }

        private void SetTimeForPicon2(ushort[] value)
        {

            this._clock = new DateTime[2];
            try
            {
                DateTime dtR = new DateTime(Convert.ToInt32(value[1] + 2000), Convert.ToInt32(value[2])
                    , Convert.ToInt32(value[3]), Convert.ToInt32(value[4])
                    , Convert.ToInt32(value[5]), Convert.ToInt32(value[6]));


                var dateSrt = dtR.Day + "/" + dtR.Month + "/" + dtR.Year;
                var timeSrt =
                    ((dtR.Hour < 10) == true ? ("0" + dtR.Hour).ToString() : (dtR.Hour.ToString())).ToString() +
                    ":" +
                    ((dtR.Minute < 10) == true ? ("0" + dtR.Minute).ToString() : (dtR.Minute.ToString()))
                    .ToString() + ":" +
                    ((dtR.Second < 10 == true) ? ("0" + dtR.Second) : (dtR.Second.ToString()));

                this.uiRealDate.Content = dateSrt;
                this.uiRealTime.Content = timeSrt;
                this.uiLocalDate.Content = dateSrt;
                this.uiLocalTime.Content = timeSrt;

                this._clock[0] = dtR;
            }
            catch (Exception e)
            {
                this.uiRealDate.Content = "Ошибка";
                this.uiRealDate.ToolTip = "Не верный формат даты";
                this.uiRealTime.Content = "Ошибка";
                this.uiRealTime.ToolTip = "Не верный формат времени";
            }

        }


        public ushort[] Value
        {
            get { return this._value; }
            set
            {
                if (value != null && value.Length == 16)
                {
                    _value = value;
                    SetTime(_value);
                }
                else
                {
                    this.SetDefault();
                }
            }
        }

        private void SetTime(ushort[] _value)
        {
            var realTime = _value.ToList().GetRange(0, 8);
            var localTime = _value.ToList().GetRange(8, 8);
            this._clock = new DateTime[2];
            try
            {
                DateTime dtR = new DateTime(Convert.ToInt32(realTime[0]), Convert.ToInt32(realTime[1]),
                    Convert.ToInt32(realTime[2]), Convert.ToInt32(realTime[4]),
                    Convert.ToInt32(realTime[5]), Convert.ToInt32(realTime[6]));
                this.uiRealDate.Content = dtR.Day + "/" + dtR.Month + "/" + dtR.Year;
                this.uiRealTime.Content =
                    ((dtR.Hour < 10) == true ? ("0" + dtR.Hour).ToString() : (dtR.Hour.ToString())).ToString() +
                    ":" +
                    ((dtR.Minute < 10) == true ? ("0" + dtR.Minute).ToString() : (dtR.Minute.ToString()))
                    .ToString() + ":" +
                    ((dtR.Second < 10 == true) ? ("0" + dtR.Second) : (dtR.Second.ToString()));

                this._clock[0] = dtR;
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
                this.uiLocalTime.Content =
                    ((dtL.Hour < 10) == true ? ("0" + dtL.Hour).ToString() : (dtL.Hour.ToString())).ToString() +
                    ":" +
                    ((dtL.Minute < 10) == true ? ("0" + dtL.Minute).ToString() : (dtL.Minute.ToString()))
                    .ToString() + ":" +
                    ((dtL.Second < 10 == true) ? ("0" + dtL.Second) : (dtL.Second.ToString()));
                this._clock[1] = dtL;
            }
            catch (Exception e)
            {
                this.uiLocalDate.Content = "Ошибка";
                this.uiLocalDate.ToolTip = "Не верный формат даты";
                this.uiLocalTime.Content = "Ошибка";
                this.uiLocalTime.ToolTip = "Не верный формат времени";
            }
        }



        //public async Task<bool> WriteContextPicon2()
        //{


        //    var timeToWritePicon = 
        //}

        public void DateTimeTypePicon2()
        {

            timeUshorts = new[]

           {
                   
                //Picon2.Converters.BytesToInt16FormatterForPicon2(this._clock[1].Year),
                Convert.ToUInt16(0),

                Convert.ToUInt16(_clock[1].Year % 100),
                Convert.ToUInt16(this._clock[1].Month),
                Convert.ToUInt16(this._clock[1].Day),
                //Convert.ToUInt16((int) this._clock[1].DayOfWeek),
                Convert.ToUInt16(this._clock[1].Hour),
                Convert.ToUInt16(this._clock[1].Minute),
                Convert.ToUInt16(this._clock[1].Second),
                Convert.ToUInt16(this._clock[1].Millisecond)

            };

        }

        public void DateTimeType()
        {

            timeUshorts = new[]

            {
                   
                //Picon2.Converters.BytesToInt16FormatterForPicon2(this._clock[1].Year),
 
                Convert.ToUInt16(_clock[1].Year),
                Convert.ToUInt16(this._clock[1].Month) ,
                Convert.ToUInt16(this._clock[1].Day),
                Convert.ToUInt16((int) this._clock[1].DayOfWeek),
                Convert.ToUInt16(this._clock[1].Hour),
                Convert.ToUInt16(this._clock[1].Minute),
                Convert.ToUInt16(this._clock[1].Second),
                Convert.ToUInt16(this._clock[1].Millisecond)

            };

        }

        public async Task<bool> WriteContext()
        {
            //todo: посмотреть отображение времени в окне (отображает на час больше, чем реально), может зимнее время или еще чего
            if (_isSystemTime)
            {
                this._clock[1] = DateTime.Now;
            }
            else
            {
                var timeToWrite =
                    (InnerTime)this.Dispatcher.Invoke(new WriteTimeEventHandler(WriteTime), this._clock[0]);
                if (timeToWrite == null)
                {
                    uiChangeTime.IsEnabled = uiSystemTime.IsEnabled = true;
                    return false;
                }
                var tmp = this._clock;

                this._clock[1] =
                    new DateTime(timeToWrite.Year == null ? tmp[1].Year : Convert.ToInt32(timeToWrite.Year),
                        timeToWrite.Month == null ? tmp[1].Month : Convert.ToInt32(timeToWrite.Month),
                        timeToWrite.Day == null ? tmp[1].Day : Convert.ToInt32(timeToWrite.Day),
                        timeToWrite.Hour == null ? tmp[1].Hour : Convert.ToInt32(timeToWrite.Hour),
                        timeToWrite.Minute == null ? tmp[1].Minute : Convert.ToInt32(timeToWrite.Minute),
                        timeToWrite.Second == null ? tmp[1].Second : Convert.ToInt32(timeToWrite.Second));

            }

            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                DateTimeTypePicon2();
            }

            else
            {
                DateTimeType();
            }



            bool res = true;

            try
            {

                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                {


                    //await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x2100, timeUshorts);
                    for (ushort i = 0; i < 7; i += 1)
                    {
                        var r = timeUshorts.Skip(i).Take(1).ToArray();
                        await RTUConnectionGlobal.SendDataByAddressAsync(1,
                            (ushort)(0x2100 + i), r);
                    }


                }
                //await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x2100, timeUshorts);



                else
                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x1000, timeUshorts);
                }



            }
            catch (Exception e)
            {
                res = false;
            }

            return res;

        }

        #endregion

        private void RunWorkerCompleted(ushort[] value)
        {
            this.Value = value;
        }

        //private void WriteTimeComplete(bool res)
        //{
        //    if (res)
        //    {
        //        if (_isSystemTime)
        //        {
        //            this.ShowMessage("Установка системного времени прошла успешно", "Установка времени",
        //                MessageBoxImage.Asterisk);
        //        }
        //        else
        //        {
        //            this.ShowMessage("Установка времени прошла успешно", "Установка времени",
        //                MessageBoxImage.Information);
        //        }
        //    }
        //    else
        //    {
        //        this.ShowMessage("В процессе установки времени произошла ошибка", "Установка времени",
        //            MessageBoxImage.Error);
        //    }
        //    this.Dispatcher.BeginInvoke(new Action(() => uiChangeTime.IsEnabled = uiSystemTime.IsEnabled = true));
        //}

        //public void SetAutonomous()
        //{
        //    this.uiChangeTime.IsEnabled = false;
        //    this.uiLocalDate.Content = "Неизвестно";
        //    this.uiLocalTime.Content = "Неизвестно";
        //    this.uiRealTime.Content = "Неизвестно";
        //    this.uiRealDate.Content = "Неизвестно";
        //    this.uiSystemTime.IsEnabled = false;
        //}

        //public void DisableAutonomous()
        //{
        //    this.uiChangeTime.IsEnabled = true;
        //    this.uiSystemTime.IsEnabled = true;
        //}

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

        private async void uiChangeTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isSystemTime = false;

                if (await WriteContext())
                {
                    uiChangeTime.IsEnabled = uiSystemTime.IsEnabled = false;
                }



            }

            catch (Exception ex)
            {
            }




        }

        private async void uiSystemTime_Click(object sender, RoutedEventArgs e)
        {
            _isSystemTime = true;
            uiChangeTime.IsEnabled = uiSystemTime.IsEnabled = false;
            if (await WriteContext())
            {
                uiChangeTime.IsEnabled = uiSystemTime.IsEnabled = false;
            }

        }

        private InnerTime WriteTime(DateTime curPLCDateTime)
        {
            NewTimeDialog ntd = new NewTimeDialog(curPLCDateTime);
            //ntd.Owner = Application.Current.MainWindow;

            ntd.ShowDialog();

            if (ntd.ResultDialog != null)
            {
                return new InnerTime(ntd.ResultDialog.Year,
                    ntd.ResultDialog.Month, ntd.ResultDialog.Day,
                    ntd.ResultDialog.Hour, ntd.ResultDialog.Minute,
                    ntd.ResultDialog.Second);
            }
            else
                return null;
        }


        public void SetAutonomus()
        {
            this.uiChangeTime.IsEnabled = false;
            this.uiSystemTime.IsEnabled = false;
            SetDefault();
        }
    }
}
