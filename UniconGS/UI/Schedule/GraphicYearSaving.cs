using System;

namespace UniconGS.UI.Schedule
{
    [Serializable]
    public class GraphicYearSaving
    {
        private int _turnOnDay = 0xff;
        private int _turnOffDay = 0xff;
        private int _turnOnMonth = 0xff;
        private int _turnOffMonth = 0xff;
        public int TurnOnMonth
        {
            get
            {
                return this._turnOnMonth;
            }
            set
            {
                this._turnOnMonth = value;
            }
        }
        public int TurnOnDay
        {
            get
            {
                return this._turnOnDay;
            }
            set
            {
                this._turnOnDay = value;
            }
        }
        public int TurnOffMonth
        {
            get
            {
                return this._turnOffMonth;
            }
            set
            {
                this._turnOffMonth = value;
            }
        }
        public int TurnOffDay
        {
            get
            {
                return this._turnOffDay;
            }
            set
            {
                this._turnOffDay = value;
            }
        }
        public GraphicYearSaving()
        {

        }
        public GraphicYearSaving(int turnOnMonth, int turnOnDay,
            int turnOffMonth, int turnOffDay)
        {
            this._turnOffDay = turnOffDay;
            this._turnOffMonth = turnOffMonth;
            this._turnOnDay = turnOnDay;
            this._turnOnMonth = turnOnMonth;
        }
    }
}
