using System;
using System.ComponentModel;

namespace UniconGS.UI.HeatingSchedule
{
    [Serializable]
    public class Date : INotifyPropertyChanged
    {
        private int _day = 0;
        private int _month = 0; //значения по умолчанию HeatingSchedule


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
            this.MonthNumber = 0;
            this.DayNumber = 0;
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
}
