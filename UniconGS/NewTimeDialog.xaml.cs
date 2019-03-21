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
using System.Windows.Shapes;
using UniconGS.Enums;

namespace UniconGS
{
    /// <summary>
    /// Interaction logic for NewTimeDialog.xaml
    /// </summary>
    public partial class NewTimeDialog : Window
    {
        #region Natied types
        public class Result
        {
            public int? Year { get; set; }
            public int? Month { get; set; }
            public int? Day { get; set; }
            public int? Hour { get; set; }
            public int? Minute { get; set; }
            public int? Second { get; set; }

            public Result(int? year, int? month, int? day, int? hour, int? minute, int? second)
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

        public Result ResultDialog { get; set; }
        DateTime _dt = new DateTime();

        public NewTimeDialog()
        {
            _dt = DateTime.Now;
            InitializeComponent();
        }

        public NewTimeDialog(DateTime curDateTime)
        {
            InitializeComponent();
            this._dt = curDateTime;
        }

        void NewTimeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if(DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                this.uiYear.Text = _dt.Year.ToString();
                this.uiMonth.SelectedIndex = _dt.Month - 1;
                this.uiDay.SelectedIndex = _dt.Day - 1;
                //час+1
                this.uiHour.SelectedIndex = _dt.Hour;
                this.uiMinute.SelectedIndex = _dt.Minute;
                this.uiSecond.SelectedIndex = _dt.Second;
            }
            else
            {
                this.uiYear.Text = _dt.Year.ToString();
                this.uiMonth.SelectedIndex = _dt.Month - 1;
                this.uiDay.SelectedIndex = _dt.Day - 1;
                this.uiHour.SelectedIndex = _dt.Hour ;
                this.uiMinute.SelectedIndex = _dt.Minute;
                this.uiSecond.SelectedIndex = _dt.Second;
            }
        }

        void uiApply_Click(object sender, RoutedEventArgs e)
        {
            int? year = null;
            int? day = null;
            int? month = null;
            int? hour = null;
            int? minute = null;
            int? second = null;

            try
            {
                if (!string.IsNullOrEmpty(this.uiYear.Text))
                {
                    if ((int.Parse(this.uiYear.Text) < 2010 || int.Parse(this.uiYear.Text) > 9999))
                    {
                        throw new ArgumentException();
                    }
                    else
                    {
                        year = int.Parse(this.uiYear.Text);
                        //year = year; //вопрос для пикон2
                    }
                }
                if (this.uiDay.SelectedIndex != this.uiDay.Items.Count)
                {
                    day = this.uiDay.SelectedIndex + 1;
                }
                if (this.uiHour.SelectedIndex != this.uiHour.Items.Count )
                {
                    hour = this.uiHour.SelectedIndex;
                }
                if (this.uiMinute.SelectedIndex != this.uiMinute.Items.Count)
                {
                    minute = this.uiMinute.SelectedIndex;
                }
                if (this.uiMonth.SelectedIndex != this.uiMonth.Items.Count )
                {
                    month = this.uiMonth.SelectedIndex + 1;
                }
                if (this.uiSecond.SelectedIndex != this.uiSecond.Items.Count )
                {
                    second = this.uiSecond.SelectedIndex;
                }
                try
                {
                    /*считаем правильное ли число*/
                    if (month != null && day != null)
                        switch (month)
                        {

                            case 2:
                                /*Февраль*/
                                if (year != null && year%4 == 0 & day > 29)
                                    throw new ArgumentException();
                                else if (year != null && year%4 != 0 & day > 28)
                                    throw new ArgumentException();
                                break;
                            case 4: /*апрель*/
                                if (day == 31)
                                    throw new ArgumentException();
                                break;
                            case 6: /*июнь*/
                                if (day == 31)
                                    throw new ArgumentException();
                                break;
                            case 9: /*сентябрь*/
                                if (day == 31)
                                    throw new ArgumentException();
                                break;
                            case 11: /*ноябрь*/
                                if (day == 31)
                                    throw new ArgumentException();
                                break;
                            default:
                                break;
                        }
                    this.ResultDialog = new Result(year, month, day, hour, minute, second);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Неверно задан день", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (InvalidOperationException)
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неверно задан год. Значение должно быть в пределах [2010;9999]", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        void uiCancel_Click(object sender, RoutedEventArgs e)
        {
            this.ResultDialog = null;
            this.DialogResult = false;
            this.Close();
        }
    }
}
