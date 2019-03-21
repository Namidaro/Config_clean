using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.Utility;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using MessageBox = System.Windows.Forms.MessageBox;
using UniconGS.Enums;
using Innovative.SolarCalculator;
using UniconGS.UI.Schedule.SolarSchedule;

namespace UniconGS.UI.Picon2.ViewModel
{
    /// <summary>
    ///     Представляет вью-модель графика освещения
    /// </summary>
    public class Picon2LightingSheduleViewModel : BindableBase
    {
        #region [Const]
        public event Schedule.Schedule.ShowMessageEventHandler ShowMessage;

        private const string SEPTEMBER_NAME = "Сентябрь";
        private const string OCTOBER_NAME = "Октябрь";
        private const string NOVEMBER_NAME = "Ноябрь";
        private const string DECEMBER_NAME = "Декабрь";
        private const string JANUARY_NAME = "Январь";
        private const string FEBRUARY_NAME = "Февраль";
        private const string MARCH_NAME = "Март";
        private const string APRIL_NAME = "Апрель";
        private const string MAY_NAME = "Май";
        private const string JUNE_NAME = "Июнь";
        private const string JULY_NAME = "Июль";
        private const string AUGUST_NAME = "Август";
        private const ushort MAIN_PACKAGE_LENGHT_WORD = 0x64; // длина основных пакетов
        private const ushort LAST_PACKAGE_LENGHT = 0x44; // длина последнего пакета
        private const int COUNT_PACKAGE = 8; //количество пакетов

        private const int MONTH_LENGHT_INDEX = 32 * 4;
        //31 день + 4 индекса на (Экономия время по) . точнее Уточнять у конструктора

        private const int DAY_LENGHT_INDEX = 4; // Количесиво байт которые занимает один день в блоке данных
        private const int ECONOMY_ADDRESS_INDEX = MONTH_LENGHT_INDEX * 12 - 4; //Адрес начала данных режима экономии

        private const int MONTH_COUNT = 12;

        #endregion

        #region [Private Fields]

        private Dictionary<string, byte[]> _sheduleCache;

        private string _deviceName;
        private ICommand _sendLightingShedule;
        private ICommand _backToSchemeCommand;
        private ICommand _getLightingSheduleCommand;
        private ICommand _navigateToLightingShedule;
        private ICommand _navigateToBackLightShedule;
        private ICommand _navigateToStoregeEnergyShedule;
        private ICommand _getSheduleFromFileCommand;
        private ICommand _storeToFileCommand;
        private ICommand _clearScheduleCommand;

        private double _latitude;
        private double _longitude;
        private ICommand _calculateScheduleCommand;

        private bool _isMonthsEnabled = false;
        private string _currentMonthName;
        private int _currentMonthIndex;
        private ObservableCollection<DaySheduleViewModel> _currentMothDaysCollection;

        private readonly ObservableCollection<string> _mothNames = new ObservableCollection<string>
        {
            JANUARY_NAME,
            FEBRUARY_NAME,
            MARCH_NAME,
            APRIL_NAME,
            MAY_NAME,
            JUNE_NAME,
            JULY_NAME,
            AUGUST_NAME,
            SEPTEMBER_NAME,
            OCTOBER_NAME,
            NOVEMBER_NAME,
            DECEMBER_NAME
        };

        private readonly ObservableCollection<int> _rangeMoth = new ObservableCollection<int>
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12
        };

        private ObservableCollection<int> _rangeDaysInEconomyStopMonth;
        private ObservableCollection<int> _rangeDaysInEconomyStartMonth;
        private Dictionary<string, int> _monthsLenghtDictionary;
        private Dictionary<string, ObservableCollection<DaySheduleViewModel>> _monthsCollection;

        private Dictionary<string, CityCoordinates> _coordinatesDictionary;
        private List<string> _cityList;
        private string _selectedCity;

        private Dictionary<string, object> _navigationContext = new Dictionary<string, object>();
        private ushort _startAddress; // адрес начала блока данных на устройстве

        private string _title = String.Empty;

        private bool _isAutonomus;
        #endregion

        #region [Ctor's]

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Picon2LightingSheduleViewModel()
        {
            ReadFromDeviceAndRefreshCache = true;
            _sheduleCache = new Dictionary<string, byte[]>();

            this._monthsLenghtDictionary = new Dictionary<string, int>();
            for (int i = 0; i < MONTH_COUNT; i++)
            {
                //2012 год, т.к. он высокосный а на вьюшке у февраля д.б. 29 дней
                this._monthsLenghtDictionary.Add(this._mothNames[i], DateTime.DaysInMonth(2012, i + 1));
            }
            this._monthsCollection = new Dictionary<string, ObservableCollection<DaySheduleViewModel>>();
            this.CityList = new List<string>();
            this.CoordinatesDictionary = new Dictionary<string, CityCoordinates>();
            InitializeCityDictionary();
            this.SelectedCity = CityList.First();
        }
        #endregion

        #region [Properties]
        /// <summary>
        ///     Заголовок вьюхи
        /// </summary>
        public string Title
        {
            get { return this._title; }
            set
            {
                if (this._title != null && this._title.Equals(value)) return;
                this._title = value;
                OnPropertyChanged("Title");
            }
        }
        /// <summary>
        /// Костыль для обновления кэша графиков
        /// </summary>
        public bool ReadFromDeviceAndRefreshCache { get; set; }
        /// <summary>
        /// Стартовый адрес для графика
        /// </summary>
        public ushort StartAddress
        {
            get { return _startAddress; }
            set { _startAddress = value; }
        }
        /// <summary>
        /// Широта местности
        /// </summary>
        public double Latitude
        {
            get { return this._latitude; }
            set
            {
                this._latitude = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Долгота местности
        /// </summary>
        public double Longitude
        {
            get { return this._longitude; }
            set
            {
                this._longitude = value;
                OnPropertyChanged("Longitude");
                //RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Библиотека городов и их координат
        /// </summary>
        public Dictionary<string, CityCoordinates> CoordinatesDictionary
        {
            get { return _coordinatesDictionary; }
            set
            {
                _coordinatesDictionary = value;
                //RaisePropertyChanged();
                OnPropertyChanged("CoordinatesDictionary");
            }
        }
        /// <summary>
        /// Список городов
        /// </summary>
        public List<string> CityList
        {
            get { return _cityList; }
            set
            {
                _cityList = value;
                //RaisePropertyChanged();
                OnPropertyChanged("CityList");
            }
        }

        public string SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                CityCoordinates cc = CoordinatesDictionary.ElementAt(GetCityIndex(value)).Value;
                Latitude = cc.Latitude;
                Longitude = cc.Longitude;
                //RaisePropertyChanged();
                OnPropertyChanged("SelectedCity");

            }
        }

        public bool IsAutonomus
        {
            get { return _isAutonomus; }
            set
            {
                _isAutonomus = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        ///     Свойство для представления текущего месяца(выбранного на вьюшке)
        /// </summary>
        public string CurrentMonthName
        {
            get { return this._currentMonthName; }
            set
            {
                if (this._currentMonthName != null && this._currentMonthName.Equals(value)) return;
                this._currentMonthName = value;
                OnPropertyChanged("CurrentMonth");
                this.CurrentMonthDayCollection = this._monthsCollection[value];
                this.CurrentMothIndex = this._mothNames.IndexOf(value);
            }
        }

        /// <summary>
        ///     Индекс текущего месяца(выбранного в данный момент)
        /// </summary>
        public int CurrentMothIndex
        {
            get { return this._currentMonthIndex; }
            set
            {
                if (this._currentMonthIndex.Equals(value)) return;
                this._currentMonthIndex = value;
                OnPropertyChanged("CurrentMothIndex");
            }
        }

        /// <summary>
        ///     Коллекция с днями текцщего месяца
        /// </summary>
        public ObservableCollection<DaySheduleViewModel> CurrentMonthDayCollection
        {
            get { return this._currentMothDaysCollection; }
            set
            {
                if (this._currentMothDaysCollection != null && this._currentMothDaysCollection.Equals(value)) return;
                this._currentMothDaysCollection = value;
                OnPropertyChanged("CurrentMonthDayCollection");
            }
        }

        /// <summary>
        ///     Коллекция с названиями месяцев
        /// </summary>
        public ObservableCollection<string> MonthCollection
        {
            get { return this._mothNames; }
        }




        /// <summary>
        ///     Возвращает набор месяцев в численном виде [1-12]
        /// </summary>
        public ObservableCollection<int> RangeMonthInts
        {
            get { return this._rangeMoth; }
        }

        /// <summary>
        ///     ВОзвращает набор чисел месяца окончания режима экономии
        /// </summary>
        public ObservableCollection<int> RangeDaysEconomyStopMonth
        {
            get { return this._rangeDaysInEconomyStopMonth; }
            set
            {
                if (this._rangeDaysInEconomyStopMonth != null && this._rangeDaysInEconomyStopMonth.Equals(value))
                    return;
                this._rangeDaysInEconomyStopMonth = value;
                OnPropertyChanged("RangeDaysEconomyStopMonth");
            }
        }

        /// <summary>
        ///     ВОзвращает набор чисел месяца начала режима экономии
        /// </summary>
        public ObservableCollection<int> RangeDaysEconomyStartMonth
        {
            get { return this._rangeDaysInEconomyStartMonth; }
            set
            {
                if (this._rangeDaysInEconomyStartMonth != null && this._rangeDaysInEconomyStartMonth.Equals(value))
                    return;
                this._rangeDaysInEconomyStartMonth = value;
                OnPropertyChanged("RangeDaysEconomyStartMonth");
            }
        }

        public bool IsMonthsEnabled
        {
            get
            {
                return _isMonthsEnabled;
            }
            set
            {
                //TODO: доделать propertychanged
                if (this._isMonthsEnabled != null && this._isMonthsEnabled.Equals(value))
                    return;
                this._isMonthsEnabled = true;
                OnPropertyChanged("IsMonthsEnabled");
            }
        }

        #endregion

        #region [IlightingSheduleViewModel]

        /// <summary>
        ///     Свойство представляющие имя устройства, для которого производится конфигурация
        /// </summary>
        public string DeviceName
        {
            get { return this._deviceName; }
            set
            {
                if (this._deviceName != null && this._deviceName.Equals(value)) return;
                this._deviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }


        public ICommand ClearSchedule
        {
            get
            {
                return this._clearScheduleCommand ??
                    (this._clearScheduleCommand = new DelegateCommand(OnClearScheduleCommand));
            }
        }


        public ICommand CalculateScheduleCommand
        {
            get
            {
                return this._calculateScheduleCommand ??
                    (this._calculateScheduleCommand = new DelegateCommand(OnCalculateScheduleCommand));
            }
        }

        /// <summary>
        ///     Команда рагрузки графика освещения из файла
        /// </summary>
        public ICommand GetSheduleFromFileCommand
        {
            get
            {
                return this._getSheduleFromFileCommand ??
                       (this._getSheduleFromFileCommand = new DelegateCommand(OnGetScheduleFromFileCommand));
            }
        }

        /// <summary>
        ///     Команда записи текущего графика освещения в файл
        /// </summary>
        public ICommand SaveToFileCommand
        {
            get
            {
                return this._storeToFileCommand ??
                       (this._storeToFileCommand = new DelegateCommand(OnSaveToFileCommand));
            }
        }

        /// <summary>
        ///      Представляет команду отправки конфигурационных данных на устройство
        /// </summary>
        public ICommand SendLightingShedule
        {
            get
            {
                return this._sendLightingShedule ??
                       (this._sendLightingShedule = new DelegateCommand(OnSendLightingShedule));
            }
        }


        /// <summary>
        ///     Представляет команду считывания графика освещения с устройства
        /// </summary>
        public ICommand GetLightingShedule
        {
            //get
            //{
            //    return this._getLightingSheduleCommand ??
            //           (this._getLightingSheduleCommand = new DelegateCommand<byte[]>(this.InitializeOnNavigateTo));
            //}
            get
            {
                return this._getLightingSheduleCommand ??
                       (this._getLightingSheduleCommand = new DelegateCommand(OnGetLightningSchedule));
            }
        }

        #endregion

        #region [INavigationAware]

        /// <summary>
        ///     
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns></returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this._deviceName = null;
            this._sendLightingShedule = null;
        }

        public async void OnClearScheduleCommand()
        {
            await TryClearSchedule();
        }

        private async Task TryClearSchedule()
        {

            ushort[] NullValue = new ushort[770];
            for (int i = 0; i < 770; i++)
            {
                //NullValue[i] = 0xFFFF;
                NullValue[i] = 0;
            }

            {
                try
                {
                    if (Title == "График 1")
                    {
                        StartAddress = 0x3280;
                    }

                    if (Title == "График 2")
                    {
                        StartAddress = 0x3580;
                    }
                    if (Title == "График 3")
                    {
                        StartAddress = 0x3880;
                    }

                    //if (Title == "uiEnergySchedule")
                    //{
                    //    baseAddr = 0x8E06;
                    //}


                    for (ushort i = 0; i < 700; i += 60)
                    {
                        var r = NullValue.Skip(i).Take(60).ToArray();
                        await RTUConnectionGlobal.SendDataByAddressAsync(1,
                        (ushort)(StartAddress + i), r);

                    }

                    await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(StartAddress + 720), NullValue.Skip(720).Take(50).ToArray());

                    OnGetLightningSchedule();
                    //await ReadScheduleData(StartAddress);
                    //UpdateBinding();
                    //await UpdateState();
                    //ExportComplete(true);
                }


                catch
                {
                    //ExportComplete(false);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(int sheduleNumber)
        {

            //TODO: подумать, как переделать чтение графика, чтобы по кнопке "Прочитать" он читал из устройства, не заглядывая в кэш и обновлял кэш
            ReadFromDeviceAndRefreshCache = false;

            IsMonthsEnabled = true;

            if (sheduleNumber == (int)LightningScheduleEnum.SCHEDULE_LIGHTNING)
            {
                Title = "График 1";
                StartAddress = 0x3280;
            }
            if (sheduleNumber == (int)LightningScheduleEnum.SCHEDULE_ILLUMINATION)
            {
                Title = "График 2";
                StartAddress = 0x3580;
            }
            if (sheduleNumber == (int)LightningScheduleEnum.SCHEDULE_SUBLIGHT)
            {
                Title = "График 3";
                StartAddress = 0x3880;
            }
            if (this._sheduleCache.ContainsKey(this.Title))
            {
                this.InitializeOnNavigateTo(this._sheduleCache[this.Title]);
            }

            OnGetLightningSchedule();

            ReadFromDeviceAndRefreshCache = true;//надо подумать, пока такая заглушка
            // this._startAddress = ushort.Parse(navigationContext.Parameters["address"].ToString());
            //GetSchedule(sheduleNumber);
        }

        public async Task ReadAllSchedules()
        {
            OnGetLightningSchedule();
        }

        private async void OnGetLightningSchedule()
        {
            if (MainWindow.isAutonomus == false)
            {
                if (ReadFromDeviceAndRefreshCache == true)
                {
                    if (_sheduleCache.Count != 0)
                    {
                        if (_sheduleCache.ContainsKey(this.Title))
                        {
                            _sheduleCache.Remove(this.Title);
                        }
                    }
                    else
                    {
                        //дикие костыли, но ничего умнее пока не придумал, 
                        //тут все уже написано через лютейшую жопу блять, 
                        //велосипеды нахуй на костыльной тяге
                        IsMonthsEnabled = true;
                        ReadFromDeviceAndRefreshCache = false;
                        Title = "График 3";
                        StartAddress = 0x3880;
                        OnGetLightningSchedule();
                        await Task.Delay(1500);
                        Title = "График 2";
                        StartAddress = 0x3580;
                        OnGetLightningSchedule();
                        await Task.Delay(1500);
                        Title = "График 1";
                        StartAddress = 0x3280;
                        ReadFromDeviceAndRefreshCache = true;
                    }
                }

                InitializeMonthsCollection();
                //this.RangeDaysEconomyStartMonth = new ObservableCollection<int>();
                //this.RangeDaysEconomyStopMonth = new ObservableCollection<int>();

                //this._monthsCollection.Clear();
                //foreach (var mothName in this._mothNames)
                //{
                //    this._monthsCollection.Add(mothName, new ObservableCollection<DaySheduleViewModel>());
                //}
                //var monthLengthList = this._monthsLenghtDictionary.Values.ToArray();
                //for (int i = 0; i < MONTH_COUNT; i++)
                //{
                //    for (int j = 0; j < monthLengthList[i]; j++)
                //    {
                //        this._monthsCollection[this._mothNames[i]].Add(new DaySheduleViewModel
                //        {
                //            Month = this._mothNames[i],
                //            DayNumber = j + 1
                //        });
                //    }
                //    this._monthsCollection[this._mothNames[i]].Add(new DaySheduleViewModel
                //    {
                //        Month = this._mothNames[i],
                //        DayNumber = monthLengthList[i] + 1,
                //        IsEconomy = true

                //    });
                //}


                //this.CurrentMonthName = this._mothNames[DateTime.Now.Month - 1];

                //for (int i = 0; i < this.MonthCollection.Count; i++)
                //{
                //    string monthName = this._mothNames[i];
                //    for (int j = 0; j < this._monthsLenghtDictionary[monthName]; j++)
                //    {
                //        this._monthsCollection[monthName][j].StartHour = 0;
                //        this._monthsCollection[monthName][j].StartMinute = 0;
                //        this._monthsCollection[monthName][j].StopHour = 0;
                //        this._monthsCollection[monthName][j].StopMinute = 0;
                //    }
                //}

                //for (int i = 0; i < CurrentMonthDayCollection.Count; i++)
                //{
                //    this.CurrentMonthDayCollection[i].StartHour = 0;
                //    this.CurrentMonthDayCollection[i].StartMinute = 0;
                //    this.CurrentMonthDayCollection[i].StopHour = 0;
                //    this.CurrentMonthDayCollection[i].StopMinute = 0;
                //}

                if (this._sheduleCache.ContainsKey(this.Title))
                {
                    this.InitializeOnNavigateTo(this._sheduleCache[this.Title]);
                }
                else
                {
                    this.InitializeOnNavigateTo();
                }


                _navigationContext.Clear();
            }
            else
            {
                MessageBox.Show("Чтение невозможно, автономный режим", "Внимание!");
            }
        }
        #endregion

        #region [Help members]

        private void InitializeMonthsCollection()
        {
            this.RangeDaysEconomyStartMonth = new ObservableCollection<int>();
            this.RangeDaysEconomyStopMonth = new ObservableCollection<int>();

            this._monthsCollection.Clear();
            foreach (var mothName in this._mothNames)
            {
                this._monthsCollection.Add(mothName, new ObservableCollection<DaySheduleViewModel>());
            }
            var monthLengthList = this._monthsLenghtDictionary.Values.ToArray();
            for (int i = 0; i < MONTH_COUNT; i++)
            {
                for (int j = 0; j < monthLengthList[i]; j++)
                {
                    this._monthsCollection[this._mothNames[i]].Add(new DaySheduleViewModel
                    {
                        Month = this._mothNames[i],
                        DayNumber = j + 1
                    });
                }
                this._monthsCollection[this._mothNames[i]].Add(new DaySheduleViewModel
                {
                    Month = this._mothNames[i],
                    DayNumber = monthLengthList[i] + 1,
                    IsEconomy = true

                });
            }


            this.CurrentMonthName = this._mothNames[DateTime.Now.Month - 1];

            for (int i = 0; i < this.MonthCollection.Count; i++)
            {
                string monthName = this._mothNames[i];
                for (int j = 0; j < this._monthsLenghtDictionary[monthName]; j++)
                {
                    this._monthsCollection[monthName][j].StartHour = 0;
                    this._monthsCollection[monthName][j].StartMinute = 0;
                    this._monthsCollection[monthName][j].StopHour = 0;
                    this._monthsCollection[monthName][j].StopMinute = 0;
                }
            }

            for (int i = 0; i < CurrentMonthDayCollection.Count; i++)
            {
                this.CurrentMonthDayCollection[i].StartHour = 0;
                this.CurrentMonthDayCollection[i].StartMinute = 0;
                this.CurrentMonthDayCollection[i].StopHour = 0;
                this.CurrentMonthDayCollection[i].StopMinute = 0;
            }
        }


        private void InitializeCityDictionary()
        {


            CityList.Add("Минск");
            CityList.Add("Брест");
            CityList.Add("Гродно");
            CityList.Add("Гомель");
            CityList.Add("Витебск");
            CityList.Add("Могилев");
            CityList.Add("Бобруйск");
            CityList.Add("Барановичи");
            CityList.Add("Орша");
            CityList.Add("Лида");

            CoordinatesDictionary.Add("Минск", new CityCoordinates(53.902, 27.561));
            CoordinatesDictionary.Add("Брест", new CityCoordinates(52.093, 23.718));
            CoordinatesDictionary.Add("Гродно", new CityCoordinates(53.675, 23.864));
            CoordinatesDictionary.Add("Гомель", new CityCoordinates(52.435, 30.998));
            CoordinatesDictionary.Add("Витебск", new CityCoordinates(55.183, 30.201));
            CoordinatesDictionary.Add("Могилев", new CityCoordinates(53.898, 30.328));
            CoordinatesDictionary.Add("Бобруйск", new CityCoordinates(53.140, 29.220));
            CoordinatesDictionary.Add("Барановичи", new CityCoordinates(53.131, 26.015));
            CoordinatesDictionary.Add("Орша", new CityCoordinates(54.50, 30.411));
            CoordinatesDictionary.Add("Лида", new CityCoordinates(53.885, 25.289));
        }

        private int GetCityIndex(string _name)
        {
            for (int i = 0; i < CityList.Count; i++)
                if (CityList[i].Equals(_name)) return i;

            return 0;
        }

        private void OnGetScheduleFromFileCommand()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Schedule files (*.schld)|*.schld",
                Title = "Выберите файл"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var doc = new XmlDocument();
                doc.Load(fileDialog.FileName);
                var tempMonthSheduleCollection = new Dictionary<string, ObservableCollection<DaySheduleViewModel>>();
                foreach (var mothName in this._mothNames)
                {
                    tempMonthSheduleCollection.Add(mothName, new ObservableCollection<DaySheduleViewModel>());
                }
                foreach (XmlElement xmlElement in doc.DocumentElement)
                {
                    if (xmlElement.Name.Equals("Month"))
                    {
                        var days = new ObservableCollection<DaySheduleViewModel>();
                        var mothName = String.Empty;
                        int mothNumber = -1;
                        foreach (XmlElement element in xmlElement)
                        {
                            if (element.Name.Equals("MonthName"))
                            {
                                mothName = element.Value;
                            }
                            if (element.Name.Equals("Number"))
                            {
                                mothNumber = Int32.Parse(element.Value ?? element.InnerText);
                            }
                            if (element.Name.Equals("Day"))
                            {
                                foreach (XmlElement xmlDay in element)
                                {
                                    if (xmlDay.Name.Equals("GraphicDay"))
                                    {
                                        if (xmlDay["isVisible"].InnerText.
                                            ToLower(CultureInfo.InvariantCulture).Equals("false"))
                                        {
                                            continue;
                                        }
                                        var day = new DaySheduleViewModel();
                                        foreach (XmlElement dayNodes in xmlDay)
                                        {
                                            if (dayNodes.Name.Equals("Number"))
                                            {
                                                var dayNumberString = dayNodes.Value ?? dayNodes.InnerText;
                                                day.DayNumber = Int32.Parse(dayNumberString.Split(' ')[0]);
                                            }
                                            if (dayNodes.Name.Equals("TurnOnTime"))
                                            {
                                                day.StopHour = Int32.Parse(dayNodes["Hour"].InnerText);
                                                day.StopMinute = Int32.Parse(dayNodes["Minute"].InnerText);
                                            }
                                            if (dayNodes.Name.Equals("TurnOffTime"))
                                            {
                                                day.StartHour = Int32.Parse(dayNodes["Hour"].InnerText);
                                                day.StartMinute = Int32.Parse(dayNodes["Minute"].InnerText);
                                            }
                                            if (dayNodes.Name.Equals("IsEconomy"))
                                            {
                                                day.IsEconomy = bool.Parse(dayNodes.InnerText);
                                            }
                                        }
                                        days.Add(day);
                                    }
                                    if (xmlDay.Name.Equals("MonthSaving"))
                                    {
                                        foreach (XmlElement dayNodes in xmlDay)
                                        {
                                            if (dayNodes.Name.Equals("TurnOnTime"))
                                            {
                                                //day.StartHour = Int32.Parse(dayNodes["Hour"].Value);
                                                //day.StartMinute = Int32.Parse(dayNodes["Minute"].Value);
                                            }
                                            if (dayNodes.Name.Equals("TurnOffTime"))
                                            {
                                                //day.StopHour = Int32.Parse(dayNodes["Hour"].Value);
                                                //day.StopMinute = Int32.Parse(dayNodes["Minute"].Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        tempMonthSheduleCollection[this._mothNames[mothNumber - 1]] = days;
                    }
                    IsMonthsEnabled = true;
                }
                this._monthsCollection = tempMonthSheduleCollection;
                this.CurrentMonthName = this._mothNames[DateTime.Now.Month - 1];
                var curMonthData = _monthsCollection[_currentMonthName];
                for (int i = 0; i < CurrentMonthDayCollection.Count; i++)
                {
                    this.CurrentMonthDayCollection[i].StartHour = curMonthData[i].StartHour;
                    this.CurrentMonthDayCollection[i].StartMinute = curMonthData[i].StartMinute;
                    this.CurrentMonthDayCollection[i].StopHour = curMonthData[i].StopHour;
                    this.CurrentMonthDayCollection[i].StopMinute = curMonthData[i].StopMinute;
                }
            }
            catch
            {
                this.InteractWithError(new Exception("Файл не может быть прочитан"));
            }
        }

        private const string DECLARATION_VERSION = "1.0";
        private const string DECLARATION_ENCODING = "utf-8";
        private void OnSaveToFileCommand()
        {
            ////test кароч была какая-то херня, при пером прочтении графиков. Если в текущем месяце что=то поменять(не переходя на другие месяцы),
            //      то текущий месяц не записывался, но если перейти на какой-то другой месяц - коллекции синхронизируются
            //      хз что это было, но пока пусть будет так, если будет не лень - поищу в чем была проблемы. пока такой костыль
            _monthsCollection[CurrentMonthName] = CurrentMonthDayCollection;

            var fileDialog = new SaveFileDialog()
            {
                Filter = "Schedule files (*.schld)|*.schld",
                Title = "Сохраните файл"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            var xMonthSaving = new XElement("MonthSaving");
            var xTurnOnTimeSav = new XElement("TurnOnTime");
            var xOnHourSav = new XElement("Hour");
            var xOnMinuteSav = new XElement("Minute");
            //if (this.IsEconomyMode)
            //{
            //    xOnHourSav.SetValue(this.StartEconomyHour);
            //    xOnMinuteSav.SetValue(this.StartEconomyMinute);
            //}
            //else
            {
                xOnHourSav.SetValue(0);
                xOnMinuteSav.SetValue(0);
            }

            xTurnOnTimeSav.Add(xOnHourSav);
            xTurnOnTimeSav.Add(xOnMinuteSav);
            xMonthSaving.Add(xTurnOnTimeSav);

            var xTurnOffTimeSav = new XElement("TurnOffTime");
            var xOffHourSav = new XElement("Hour");
            var xOffMinuteSav = new XElement("Minute");
            {
                xOffHourSav.SetValue(0);
                xOffMinuteSav.SetValue(0);
            }

            xTurnOffTimeSav.Add(xOffHourSav);
            xTurnOffTimeSav.Add(xOffMinuteSav);
            xMonthSaving.Add(xTurnOffTimeSav);

            try
            {
                var xDoc = new XDocument(new XDeclaration(DECLARATION_VERSION, DECLARATION_ENCODING, ""));
                var root = new XElement("Graphic");
                for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
                {
                    var xMonth = new XElement("Month");
                    var xMonthName = new XElement("MonthName");
                    xMonthName.SetValue(this._mothNames[monthIndex - 1]);
                    xMonth.Add(xMonthName);
                    var xMonthNumber = new XElement("Number");
                    xMonthNumber.SetValue(monthIndex);
                    xMonth.Add(xMonthNumber);
                    var xDays = new XElement("Day");
                    for (int dayIndex = 1; dayIndex <= 32; dayIndex++)
                    {
                        var xDay = new XElement("GraphicDay");
                        var xVisible = new XElement("isVisible");
                        var xIsEconomy = new XElement("IsEconomy");
                        var vis = false;
                        if (this._monthsLenghtDictionary[this._mothNames[monthIndex - 1]] + 1 >= dayIndex)
                        {
                            vis = true;
                        }
                        else
                        {
                            vis = false;
                        }
                        xVisible.SetValue(vis);
                        xDay.Add(xVisible);

                        bool isEconomy = _monthsLenghtDictionary[this._mothNames[monthIndex - 1]] + 1 == dayIndex;
                        xIsEconomy.SetValue(isEconomy);
                        xDay.Add(xIsEconomy);


                        var xDayNumber = new XElement("Number");
                        xDayNumber.SetValue(dayIndex.ToString(CultureInfo.InvariantCulture) + " Число");
                        xDay.Add(xDayNumber);

                        var xTurnOnTime = new XElement("TurnOnTime");
                        var xOnHour = new XElement("Hour");
                        var xOnMinute = new XElement("Minute");
                        if (vis)
                        {
                            xOnHour.SetValue(this._monthsCollection[this._mothNames[monthIndex - 1]][dayIndex - 1].StartHour);
                            xOnMinute.SetValue(this._monthsCollection[this._mothNames[monthIndex - 1]][dayIndex - 1].StartMinute);
                        }
                        else
                        {
                            xOnHour.SetValue(0);
                            xOnMinute.SetValue(0);
                        }

                        var xTurnOffTime = new XElement("TurnOffTime");
                        xTurnOffTime.Add(xOnHour);
                        xTurnOffTime.Add(xOnMinute);
                        xDay.Add(xTurnOnTime);


                        var xOffHour = new XElement("Hour");
                        var xOffMinute = new XElement("Minute");
                        if (vis)
                        {
                            xOffHour.SetValue(this._monthsCollection[this._mothNames[monthIndex - 1]][dayIndex - 1].StopHour);
                            xOffMinute.SetValue(this._monthsCollection[this._mothNames[monthIndex - 1]][dayIndex - 1].StopMinute);
                        }
                        else
                        {
                            xOffHour.SetValue(0);
                            xOffMinute.SetValue(0);
                        }

                        xTurnOnTime.Add(xOffHour);
                        xTurnOnTime.Add(xOffMinute);
                        xDay.Add(xTurnOffTime);

                        xDays.Add(xDay);
                    }
                    xMonth.Add(xDays);

                    xMonth.Add(xMonthSaving);
                    root.Add(xMonth);
                }

                var xYearSaving = new XElement("YearSaving");

                {
                    xYearSaving.Add(new object[] { new XElement("TurnOnMonth", 1), new XElement("TurnOnDay", 1),
                        new XElement("TurnOffMonth", 1), new XElement("TurnOffDay", 1) });
                }
                root.Add(xYearSaving);

                root.Add(xMonthSaving);
                xDoc.Add(root);
                xDoc.Save(fileDialog.FileName);
            }
            catch (Exception err)
            {
                this.InteractWithError(err);
            }
        }


        private async void OnSendLightingShedule()
        {
            try
            {
                int countMainWritePachage = 12;
                ushort lenghtMainPackage = 0x40;
                //var tasks = new Task[countMainWritePachage + 1];


                byte[] initializingData = this.GetDeviceDataFromView();
                for (int i = 0; i < countMainWritePachage; i++)
                {
                    int startIndexPackage = i * lenghtMainPackage * 2; //*2 - т.к. здесь байты, а там слова
                    await RTUConnectionGlobal.SendDataByAddressAsync(1,
                        (ushort)(StartAddress + lenghtMainPackage * i),
                        ArrayExtension.ByteArrayToUshortArray(this.GetPackageFromAllDAta(startIndexPackage, lenghtMainPackage * 2, initializingData)));
                    await Task.Delay(100);  //задержка для записи графиков,
                                            //видимо иногда устройство не успевает обработать
                                            //предыдущий пакет, когда в него уже запихивают новый
                }

                MessageBox.Show("Запись графиков прошла успешно", "Запись графиков", MessageBoxButtons.OK);
                //запись графика в кэш
                //SetCachedSchedule(Title, initializingData);

            }
            catch (Exception error)
            {
                this.InteractWithError(error);
            }

        }

        private byte[] GetPackageFromAllDAta(int start, int lenght, byte[] allData)
        {
            var result = new byte[lenght];
            for (int i = 0; i < lenght; i++)
            {
                result[i] = allData[start + i];
            }
            return result;
        }

        private byte[] GetDeviceDataFromView()
        {
            byte[] result = new byte[1536];
            //test кароч была какая-то херня, при пером прочтении графиков. Если в текущем месяце что-то поменять(не переходя на другие месяцы),
            //      то текущий месяц не записывался, но если перейти на какой-то другой месяц - коллекции синхронизируются
            //      хз что это было, но пока пусть будет так, если будет не лень - поищу в чем была проблемы. пока такой костыль
            _monthsCollection[CurrentMonthName] = CurrentMonthDayCollection;

            for (int i = 0; i < this.MonthCollection.Count; i++)
            {
                string monthName = this._mothNames[i];
                int monthStartIndex = MONTH_LENGHT_INDEX * i;

                DaySheduleViewModel economyDaySheduleViewModel = this._monthsCollection[monthName].Last();


                result[monthStartIndex + 1] = (byte)economyDaySheduleViewModel.StartHour;
                result[monthStartIndex] = (byte)economyDaySheduleViewModel.StartMinute;
                result[monthStartIndex + 3] = (byte)economyDaySheduleViewModel.StopHour;
                result[monthStartIndex + 2] = (byte)economyDaySheduleViewModel.StopMinute;

                for (int j = 0; j < this._monthsLenghtDictionary[monthName]; j++)
                {
                    int dayStartIndex = monthStartIndex + j * DAY_LENGHT_INDEX;
                    result[dayStartIndex + 5] = (byte)this._monthsCollection[monthName][j].StartHour;
                    result[dayStartIndex + 4] = (byte)this._monthsCollection[monthName][j].StartMinute;
                    result[dayStartIndex + 7] = (byte)this._monthsCollection[monthName][j].StopHour;
                    result[dayStartIndex + 6] = (byte)this._monthsCollection[monthName][j].StopMinute;
                }
            }
            List<byte> resAsHex = ConvertFromIntAsDecToIntAsHex(result);
            return resAsHex.ToArray();
        }

        /// <summary>
        /// Составление графиков, основываясь на времени гражданских сумерек данной местности
        /// </summary>
        /// <returns>byte[] array</returns>
        private void GetScheduleDataFromSolar()
        {
            //тут много в принципе не нужных try-catch конструкций, которые я поставил, чтобы отловить ошибку, которая появлялась только в exe-шнике
            //оказалось проблема вообще не в этом была, но я оставлю все так, если будет желание - потом как-нибудь почищу
            this.IsMonthsEnabled = true;
            SolarTimes solarTimes = new SolarTimes();
            TimeSpan sunriseTime = new TimeSpan();
            TimeSpan sunsetTime = new TimeSpan();
            TimeSpan civilDuskM = new TimeSpan();
            TimeSpan civilDuskE = new TimeSpan();
            byte[] result = new byte[1536];

            this._monthsCollection.Clear();
            try
            {

                foreach (var mothName in this._mothNames)
                {
                    this._monthsCollection.Add(mothName, new ObservableCollection<DaySheduleViewModel>());
                }
                var monthLengthList = this._monthsLenghtDictionary.Values.ToArray();
                for (int i = 0; i < MONTH_COUNT; i++)
                {
                    for (int j = 0; j < monthLengthList[i]; j++)
                    {
                        this._monthsCollection[this._mothNames[i]].Add(new DaySheduleViewModel
                        {
                            Month = this._mothNames[i],
                            DayNumber = j + 1
                        });
                    }
                    this._monthsCollection[this._mothNames[i]].Add(new DaySheduleViewModel
                    {
                        Month = this._mothNames[i],
                        DayNumber = monthLengthList[i] + 1,
                        IsEconomy = true

                    });
                }
                this.CurrentMonthName = this._mothNames[DateTime.Now.Month - 1];
            }
            catch (Exception ex)
            {
                ShowMessage("Заполнение коллекции месяцев", "ошибка", MessageBoxImage.Error);
            }
            try
            {
                for (int i = 0; i < this.MonthCollection.Count; i++)
                {

                    try
                    {

                        string monthName = this._mothNames[i];
                        int monthStartIndex = MONTH_LENGHT_INDEX * i;

                        //что делать с экономией
                        DaySheduleViewModel economyDaySheduleViewModel = this._monthsCollection[monthName].Last();

                        result[monthStartIndex + 1] = (byte)economyDaySheduleViewModel.StartHour;
                        result[monthStartIndex] = (byte)economyDaySheduleViewModel.StartMinute;
                        result[monthStartIndex + 3] = (byte)economyDaySheduleViewModel.StopHour;
                        result[monthStartIndex + 2] = (byte)economyDaySheduleViewModel.StopMinute;

                        for (int j = 0; j < this._monthsLenghtDictionary[monthName]; j++)
                        {
                            try
                            {
                                solarTimes = new SolarTimes(new DateTime(DateTime.Today.Year, i + 1, j + 1), Latitude, Longitude);
                                sunriseTime = solarTimes.Sunrise.TimeOfDay;
                                sunsetTime = solarTimes.Sunset.TimeOfDay;

                                Solar solar = new Solar(Latitude, solarTimes.SolarDeclination);

                                TimeSpan civilDelta = new TimeSpan((int)Math.Abs(Math.Floor(solar.TCivil)),
                                                                   (int)Math.Abs((solar.TCivil - Math.Truncate(solar.TCivil)) * 60),
                                                                   0);

                                civilDuskM = (sunriseTime - civilDelta);
                                civilDuskE = (sunsetTime + civilDelta);

                                int dayStartIndex = monthStartIndex + j * DAY_LENGHT_INDEX;
                                result[dayStartIndex + 5] = (byte)civilDuskE.Hours;
                                result[dayStartIndex + 4] = (byte)civilDuskE.Minutes;
                                result[dayStartIndex + 7] = (byte)civilDuskM.Hours;
                                result[dayStartIndex + 6] = (byte)civilDuskM.Minutes;
                            }
                            catch { }
                        }
                    }
                    catch (Exception ex1)
                    {
                        ShowMessage("расчет", "ошибка", MessageBoxImage.Error);
                    }

                }
            }
            catch (Exception ex3)
            {
                ShowMessage("цикл", "ошибка", MessageBoxImage.Error);
            }


            try
            {
                InitializeOnNavigateTo(result);

            }
            catch (Exception ex2)
            {
                ShowMessage("заполнение ui", "ошибка", MessageBoxImage.Error);

            }
        }

        private void OnCalculateScheduleCommand()
        {
            try
            {
                GetScheduleDataFromSolar();

            }
            catch (Exception ex)
            {
                ShowMessage("Вызов метода", "ошибка", MessageBoxImage.Error);

            }
        }

        private async Task<byte[]> GetLightingSheduleDataFromDeviceAsync()
        {
            List<byte[]> dataToRead = new List<byte[]>(COUNT_PACKAGE);



            for (int i = 0; i < COUNT_PACKAGE - 1; i++)
            {

                dataToRead.Add(ArrayExtension.UshortArrayToByteArray(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(StartAddress + MAIN_PACKAGE_LENGHT_WORD * i),
                    MAIN_PACKAGE_LENGHT_WORD)));
            }
            dataToRead.Add(ArrayExtension.UshortArrayToByteArray(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(StartAddress + 700),
                LAST_PACKAGE_LENGHT)));

            var result = new List<byte>();



            foreach (byte[] data in dataToRead)
            {
                if (data == null)
                {
                    throw new Exception("2.Чтение данных из устройства завершилось с ошибкой.");
                }
                result.AddRange(data);

            }
            var arrayResult = ConvertFromIntAsHexToIntAsDec(result).ToArray();
            if (this._sheduleCache.ContainsKey(this.Title))
            {
                this._sheduleCache[this.Title] = arrayResult;

            }
            else
            {
                this._sheduleCache.Add(this.Title, arrayResult);
            }
            return arrayResult;
        }

        private async void InitializeOnNavigateTo(byte[] data = null)
        {
            bool res = false;

            try
            {
                byte[] initializingData = null;
                if (data == null)
                {
                    initializingData = await this.GetLightingSheduleDataFromDeviceAsync();
                }
                else
                {
                    initializingData = data;
                }
                this.InitializeDaysFromDeviceData(initializingData);
                var curMonthData = _monthsCollection[_currentMonthName];
                for (int i = 0; i < CurrentMonthDayCollection.Count; i++)
                {
                    this.CurrentMonthDayCollection[i].StartHour = curMonthData[i].StartHour;
                    this.CurrentMonthDayCollection[i].StartMinute = curMonthData[i].StartMinute;
                    this.CurrentMonthDayCollection[i].StopHour = curMonthData[i].StopHour;
                    this.CurrentMonthDayCollection[i].StopMinute = curMonthData[i].StopMinute;
                }

                //res = true;

            }
            catch (Exception error)
            {
                this.InteractWithError(error);
            }


            //if (res == true)
            //{
            //    MessageBox.Show("Чтение графиков прошло успешно", "Чтение графиков", MessageBoxButtons.OK);
            //}

        }

        private void InitializeDaysFromDeviceData(byte[] deviceData)
        {
            if (deviceData.Length != 1536)
            {
                throw new Exception("3.Чтение данных из устройства завершилось с ошибкой.");
            }

            //else
            //{
            //    this.ShowMessage("Чтение графика прошло успешно",
            //        "Чтение графика", MessageBoxImage.Information);
            //}


            for (int i = 0; i < this.MonthCollection.Count; i++)
            {
                string monthName = this._mothNames[i];
                int monthStartIndex = MONTH_LENGHT_INDEX * i;
                //инициализировать коллекцию месяцев
                if (this._monthsCollection.Count == 0)
                {
                    InitializeMonthsCollection();
                    IsMonthsEnabled = true;
                }
                DaySheduleViewModel economyDaySheduleViewModel = this._monthsCollection[monthName].Last();

                economyDaySheduleViewModel.StartHour = deviceData[monthStartIndex + 1];
                economyDaySheduleViewModel.StartMinute = deviceData[monthStartIndex];
                economyDaySheduleViewModel.StopHour = deviceData[monthStartIndex + 3];
                economyDaySheduleViewModel.StopMinute = deviceData[monthStartIndex + 2];


                for (int j = 0; j < this._monthsLenghtDictionary[monthName]; j++)
                {
                    int dayStartIndex = monthStartIndex + j * DAY_LENGHT_INDEX;
                    this._monthsCollection[monthName][j].StartHour = deviceData[dayStartIndex + 5];
                    this._monthsCollection[monthName][j].StartMinute = deviceData[dayStartIndex + 4];
                    this._monthsCollection[monthName][j].StopHour = deviceData[dayStartIndex + 7];
                    this._monthsCollection[monthName][j].StopMinute = deviceData[dayStartIndex + 6];

                }
            }


        }

        public byte[] GetCachedSchedule(string _name)
        {
            byte[] _val;
            _sheduleCache.TryGetValue(_name, out _val);
            return _val;
        }

        public void SetCachedSchedule(string _name, byte[] _schedule)
        {
            if (this._sheduleCache.ContainsKey(_name))
            {

                this._sheduleCache.Remove(_name);
                this._sheduleCache.Add(_name, _schedule);

            }
            else
            {
                this._sheduleCache.Add(_name, _schedule);
            }
            InitializeOnNavigateTo(_sheduleCache[_name]);
        }

        private void InteractWithError(Exception error)
        {
            MessageBox.Show(error.Message);
        }

        #endregion

        #region [Navigate]

        /// <summary>
        ///     Команда навигации на вьюху графика освещения
        /// </summary>
        public ICommand NavigateToSheduler1Command
        {
            get
            {
                return this._navigateToLightingShedule ??
                       (this._navigateToLightingShedule = new DelegateCommand(() => OnNavigatedTo((int)LightningScheduleEnum.SCHEDULE_LIGHTNING)));
            }
        }

        /// <summary>
        ///     Команда навигации на вьюху графика подсветки
        /// </summary>
        public ICommand NavigateToSheduler2Command
        {
            get
            {
                return this._navigateToBackLightShedule ??
                       (this._navigateToBackLightShedule = new DelegateCommand(() => OnNavigatedTo((int)LightningScheduleEnum.SCHEDULE_ILLUMINATION)));
            }
        }

        /// <summary>
        ///     Команда навигации на вьюху графика энергосбережения
        /// </summary>
        public ICommand NavigateToSheduler3Command
        {
            get
            {
                return this._navigateToStoregeEnergyShedule ??
                       (this._navigateToStoregeEnergyShedule = new DelegateCommand(() => OnNavigatedTo((int)LightningScheduleEnum.SCHEDULE_SUBLIGHT)));
            }
        }





        // интересная история: в пиконе2 значения времени хранятся в хексе, 
        // но если читать это хексовое значение и предполагать, что на самом деле ты видишь десятичное, то это и будет действительное значение времени в десятичном варианте
        // то есть видишь 22 в хексе значит это 22 в дэке, но с устройства получаем уже преобразованные из хекса в дэк байты, поэтому так:
        private List<byte> ConvertFromIntAsHexToIntAsDec(List<byte> intsAsHex)
        {
            List<byte> intsAsDec = new List<byte>();
            foreach (var intAsHex in intsAsHex)
            {
                string hexValueStr = intAsHex.ToString("X");
                byte decValue = Convert.ToByte(hexValueStr);
                intsAsDec.Add(decValue);
            }

            return intsAsDec;
        }
        // наоборот
        private List<byte> ConvertFromIntAsDecToIntAsHex(byte[] intsAsDec)
        {
            List<byte> intsAsHex = new List<byte>();
            foreach (var intAsHex in intsAsDec)
            {
                string decValueStr = intAsHex.ToString("D");
                byte hexValue = Convert.ToByte(decValueStr, 16);
                intsAsHex.Add(hexValue);
            }

            return intsAsHex;
        }




        #endregion
    }
}

