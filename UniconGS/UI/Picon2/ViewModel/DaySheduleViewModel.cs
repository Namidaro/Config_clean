using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace UniconGS.UI.Picon2.ViewModel
{
    /// <summary>
    ///     Представляет вью-модель дня для графика освещения
    /// </summary>
    public class DaySheduleViewModel :BindableBase
    {
        #region [Private fields]

        private int _dayNumber;
        private int _startHour;
        private int _startMinute;
        private int _stopHour;
        private int _stopMinute;
        private ObservableCollection<int> _rangeHour;
        private ObservableCollection<int> _rangeMinute;
        private bool _isEconomy = false;

        #endregion

        #region [Ctor's]

        /// <summary>
        ///     Конструктор
        /// </summary>
        public DaySheduleViewModel()
        {
            this.RangeHour = new ObservableCollection<int>();
            for (int i = 0; i < 24; i++)
            {
                this.RangeHour.Add(i);
            }
            this.RangeMinute = new ObservableCollection<int>();
            for (int i = 0; i < 60; i++)
            {
                this.RangeMinute.Add(i);
            }
        }

        #endregion  [Ctor's]

        #region [Properties]

        /// <summary>
        ///     Свойство с диапазоном часов [0-23]
        /// </summary>
        public ObservableCollection<int> RangeHour
        {
            get { return this._rangeHour; }
            private set
            {
                if (this._rangeHour != null && this._rangeHour.Equals(value)) return;
                this._rangeHour = value;
                OnPropertyChanged("RangeHour");
            }
        }

        /// <summary>
        ///     Свойство с диапазоном минут [0-59]
        /// </summary>
        public ObservableCollection<int> RangeMinute
        {
            get { return this._rangeMinute; }
            private set
            {
                if (this._rangeMinute != null && this._rangeMinute.Equals(value)) return;
                this._rangeMinute = value;
                OnPropertyChanged("RangeMinute");
            }
        }

        #endregion [Properties]

        #region [IDaySheduleViewModel]
        /// <summary>
        /// относится ли этот график к экономии
        /// </summary>
        public bool IsEconomy
        {
            get { return _isEconomy; }
            set
            {
                _isEconomy = value;
                OnPropertyChanged(nameof(IsEconomy));
            }
        }

        /// <summary>
        ///     месяц к которому относится день
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        ///     Календарное число месяца
        ///</summary>
        public int DayNumber
        {
            get { return this._dayNumber; }
            set
            {
                if (this._dayNumber.Equals(value)) return;
                this._dayNumber = value;
                OnPropertyChanged("DayNumber");
            }
        }

        /// <summary>
        ///     Время включения в часах
        ///     (На самом деле сейчас на него вяжется время отключения)
        /// </summary>
        public int StartHour
        {
            get { return this._startHour; }
            set
            {
                this._startHour = value;
                OnPropertyChanged("StartHour");
            }
        }

        /// <summary>
        ///     Время включения в минутах
        ///      (На самом деле сейчас на него вяжется время отключения)
        /// </summary>
        public int StartMinute
        {
            get { return this._startMinute; }
            set
            {
                this._startMinute = value;
                OnPropertyChanged("StartMinute");
            }
        }

        /// <summary>
        ///     Время отключения в часах
        ///      (На самом деле сейчас на него вяжется время включения)
        /// </summary>
        public int StopHour
        {
            get { return this._stopHour; }
            set
            {
                this._stopHour = value;
                OnPropertyChanged("StopHour");
            }
        }

        /// <summary>
        ///     Время отключения в минутах
        ///      (На самом деле сейчас на него вяжется время включения)
        /// </summary>
        public int StopMinute
        {
            get { return this._stopMinute; }
            set
            {
                this._stopMinute = value;
                OnPropertyChanged("StopMinute");
            }
        }

        #endregion
    }
}