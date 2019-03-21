using System;
using System.ComponentModel;

namespace UniconGS.UI.Schedule
{
    [Serializable]
    public class GraphicTime : INotifyPropertyChanged
    {
        private int _hour=0;
        private int _minute=0;
        public int Hour
        {
            get
            {
                return this._hour;
            }
            set
            {
                this._hour = value;
                onPropertyChanged("Hour");
            }
        }
        public int Minute
        {
            get
            {
                return this._minute;
            }
            set
            {
                this._minute = value;
                onPropertyChanged("Minute");
            }
        }
        public GraphicTime(int hour , int minute)
        {
            this.Hour = hour = 0xff;//
            this.Minute = minute = 0xff;//
        }
        public GraphicTime()
        { }

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
