using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using UniconGS.Source;
using UniconGS.UI.Schedule.SolarSchedule;

namespace UniconGS.UI.Schedule
{
    /// <summary>
    /// Interaction logic for Schedule.xaml
    /// </summary>
    public partial class Schedule : INotifyPropertyChanged
    {
        #region Events
        public delegate object OpenFromFileEventHandler(Type type);
        public event OpenFromFileEventHandler OpenFromFile;

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void SaveInFileEventHandler(object value, Type type);
        public event SaveInFileEventHandler SaveInFile;

        public delegate void ShowMessageEventHandler(string message, string caption, MessageBoxImage image);
        public event ShowMessageEventHandler ShowMessage;

        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();

        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;

        private delegate void ReadComplete(ushort[] res);
        private delegate void WriteComplete(bool res);

        private double _latitude;
        private double _longitude;
        private ICommand _calculateSchedule;
        private Dictionary<string, CityCoordinates> _coordinatesDictionary;
        private List<string> _cityList;
        private string _selectedCity;
        #endregion

        #region Globals
        private GraphicValue _value;
        bool isAutonomus = false;

        #endregion
        public Schedule()
        {
            InitializeComponent();
            this._value = new GraphicValue(this.InitializeGraphic());
            this.uiMonther.SelectedIndex = 0;
            this.CityList = new List<string>();
            this.CoordinatesDictionary = new Dictionary<string, CityCoordinates>();
            InitializeCityDictionary();
            this.SelectedCity = CityList.First();
            this.UpdateBinding();


            this.uiLatitude.Text = Latitude.ToString();
            this.uiLongitude.Text = Longitude.ToString();
            this.uiCityList.ItemsSource = CityList.ToList();
            this.uiCityList.SelectedItem = CityList.First();
            this.uiCalculateButton.Command = CalculateScheduleCommand;

            //if(Name== "uiLightingSchedule")
            //{
            //    uiSolarExpander.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    uiSolarExpander.Visibility = Visibility.Collapsed;
            //}

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
                OnPropertyChanged("CityList");
            }
        }

        public string SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                //CityCoordinates cc = CoordinatesDictionary.ElementAt(GetCityIndex(value)).Value;
                //Latitude = cc.Latitude;
                //Longitude = cc.Longitude;
                OnPropertyChanged("SelectedCity");
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
                OnPropertyChanged("CoordinatesDictionary");
            }
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
                OnPropertyChanged("Latitude");
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
            }
        }
        public ICommand CalculateScheduleCommand
        {
            get
            {
                return this._calculateSchedule ??
                    (this._calculateSchedule = new DelegateCommand(OnCalculateScheduleCommand));
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

        public GraphicValue ScheduleValue
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

        public Visibility HasEconomy
        {
            get

            {
                if (Name == "uiEnergySchedule")
                {
                    uiSaving.Visibility = Visibility.Hidden;
                    PART_Savings.Visibility = Visibility.Hidden;
                }

                return this.uiSaving.Visibility;
            }
            set
            {
                if (Name == "uiEnergySchedule")
                {
                    uiSaving.Visibility = Visibility.Hidden;
                    PART_Savings.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.uiSaving.Visibility = value;

                }

            }
        }

        public void SetAutonomous()
        {


            this.uiExport.IsEnabled = false;
            this.uiImport.IsEnabled = false;
            this.uiClearAll.IsEnabled = false;


        }

        public void DisableAutonomous()
        {
            this.uiExport.IsEnabled = true;
            this.uiImport.IsEnabled = true;
            this.uiClearAll.IsEnabled = true;
        }

        private async void uiImport_Click(object sender, RoutedEventArgs e)
        {
            await this.UpdateState();


            if (this.ShowMessage != null)
            {


                if (Name == "uiLightingSchedule")
                {
                    this.ShowMessage("Чтение графика освещения прошло успешно",
                    "Чтение графика", MessageBoxImage.Information);
                }

                if (Name == "uiBacklightSchedule")
                {
                    this.ShowMessage("Чтение графика подсветки прошло успешно",
                   "Чтение графика", MessageBoxImage.Information);
                }
                if (Name == "uiIlluminationSchedule")
                {
                    this.ShowMessage("Чтение графика иллюминации прошло успешно",
                   "Чтение графика", MessageBoxImage.Information);
                }

                if (Name == "uiEnergySchedule")
                {
                    this.ShowMessage("Чтение графика энергосбережения прошло успешно",
                   "Чтение графика", MessageBoxImage.Information);
                }


            }
        }

        public async Task UpdateState()
        {
            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = false;

            if (Name == "uiLightingSchedule")
            {
                const ushort address = 0x8500;
                await this.ReadScheduleData(address);
            }

            if (Name == "uiBacklightSchedule")
            {
                const ushort address = 0x8802;
                await this.ReadScheduleData(address);
            }
            if (Name == "uiIlluminationSchedule")
            {
                const ushort address = 0x8B04;
                await this.ReadScheduleData(address);
            }

            if (Name == "uiEnergySchedule")
            {
                const ushort address = 0x8E06;
                await this.ReadScheduleData(address);
            }

            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = true;
        }

        private async Task ReadScheduleData(ushort startAddress)
        {
            var ushorts = new List<ushort>();
            for (ushort i = 0; i < 700; i += 100)
            {
                ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(startAddress + i), 100));
            }

            ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(startAddress + 700), 70));


            this.ImportComplete(ushorts.ToArray());
        }

        private void ImportComplete(ushort[] value)
        {
            //    if (value != null)
            //    {
            //if (value.First() == 0xFF)
            //{
            //    this.ShowMessage("График пуст",
            //           "Чтение графика", MessageBoxImage.Information);
            //}
            //else
            //{
            this._value = GraphicValue.SetValue(this.InitializeGraphic(), value);
            this.UpdateBinding();

            //}
            //}
            //else
            //{
            //    if (this.ShowMessage != null)
            //    {
            //        this.ShowMessage("Во время чтения графика из устройства произошла ошибка.",
            //            "Чтение графика", MessageBoxImage.Error);
            //    }
            //}
            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = true;
        }
        public async void uiExport_Click(object sender, RoutedEventArgs e)
        {
            await WriteAll();
            if (this.ShowMessage != null)
            {

                if (Name == "uiLightingSchedule")
                {

                    this.ShowMessage("Запись графика освещения в устройство прошла успешно.",
                        "Запись графика в устройство", MessageBoxImage.Information);
                }

                if (Name == "uiBacklightSchedule")
                {

                    this.ShowMessage("Запись графика подсветки в устройство прошла успешно.",
                        "Запись графика в устройство", MessageBoxImage.Information);
                }
                if (Name == "uiIlluminationSchedule")
                {

                    this.ShowMessage("Запись графика иллюминации в устройство прошла успешно.",
                        "Запись графика в устройство", MessageBoxImage.Information);
                }

                if (Name == "uiEnergySchedule")
                {

                    this.ShowMessage("Запись графика энергосбережения в устройство прошла успешно.",
                        "Запись графика в устройство", MessageBoxImage.Information);
                }


            }
        }

        public async Task WriteAll()
        {
            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiClearAll.IsEnabled = uiSave.IsEnabled = false;
            {
                try
                {
                    ushort baseAddr = 0;
                    if (Name == "uiLightingSchedule")
                    {
                        baseAddr = 0x8500;
                    }

                    if (Name == "uiBacklightSchedule")
                    {
                        baseAddr = 0x8802;
                    }
                    if (Name == "uiIlluminationSchedule")
                    {
                        baseAddr = 0x8B04;
                    }

                    if (Name == "uiEnergySchedule")
                    {
                        baseAddr = 0x8E06;
                    }

                    for (ushort i = 0; i < 700; i += 60)
                    {
                        var r = Value.Skip(i).Take(60).ToArray();
                        await RTUConnectionGlobal.SendDataByAddressAsync(1,
                            (ushort)(baseAddr + i), r);
                    }
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(baseAddr + 720), Value.Skip(720).Take(50).ToArray());

                    ExportComplete(true);
                }

                catch
                {
                    ExportComplete(false);
                }


            }
            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = true;
        }

        public void ExportComplete(bool res)
        {
            if (res)
            {

            }
            else
            {
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время записи графика в устройство произошла ошибка.",
                        "Запись графика в устройство", MessageBoxImage.Error);
                }
            }
            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = true;
        }

        private void uiOpen_Click(object sender, RoutedEventArgs e)
        {
            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = false;
            if (this.OpenFromFile != null)
            {
                var result = this.OpenFromFile(typeof(GraphicValue));
                if (result != null)
                {
                    if (this.StartWork != null)
                        this.StartWork();
                    this._value = (GraphicValue)result;
                    this.UpdateBinding();
                    if (this.StopWork != null)
                        this.StopWork();
                }
            }
            if (MainWindow.isAutonomus == true)
            {
                uiClearAll.IsEnabled = uiExport.IsEnabled = uiImport.IsEnabled = false;
                uiOpen.IsEnabled = uiSave.IsEnabled = true;
            }
            else
            {
                uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = true;
            }


        }

        private void uiSave_Click(object sender, RoutedEventArgs e)
        {
            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = false;
            if (this.SaveInFile != null)
            {
                this.SaveInFile(this._value, typeof(GraphicValue));
            }
            if (MainWindow.isAutonomus == true)
            {
                uiClearAll.IsEnabled = uiExport.IsEnabled = uiImport.IsEnabled = false;
                uiOpen.IsEnabled = uiSave.IsEnabled = true;
            }
            else
            {
                uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = true;
            }
        }

        private void uiMonther_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.StartWork != null)
                this.StartWork();
            this.uiDayter.DataContext =
                this._value.Month[this.uiMonther.SelectedIndex].Days.Where((w) => { return w.isVisible; })
                    .Select((s) => { return s; })
                    .ToList();
            if (this.StopWork != null)
                this.StopWork();
        }

        private void ItemAsCombo_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.Source is ComboBox)
            {
                ComboBox comboBox = (ComboBox)e.Source;
                this.uiDayter.SelectedItem = comboBox.DataContext;
            }
        }

        #region Privates
        private List<GraphicMonth> InitializeGraphic()
        {
            return new List<GraphicMonth>()
            {
                new GraphicMonth("Январь", 1,true),
                new GraphicMonth("Февраль", 2,null),
                new GraphicMonth("Март", 3,true),
                new GraphicMonth("Апрель", 4,false),
                new GraphicMonth("Май", 5,true),
                new GraphicMonth("Июнь", 6,false),
                new GraphicMonth("Июль", 7,true),
                new GraphicMonth("Август", 8,true),
                new GraphicMonth("Сентябрь", 9,false),
                new GraphicMonth("Октябрь", 10,true),
                new GraphicMonth("Ноябрь", 11, false),
                new GraphicMonth("Декабрь", 12,true),
            };
        }

        private void SetGraphicValue(ushort[] value)
        {
            /*Подправить для экономии*/
            if (value is Array)
            {
                this._value = GraphicValue.SetValue(this.InitializeGraphic(), value);
                this.UpdateBinding();
            }
        }

        public ushort[] GetGraphicValue()
        {
            return this._value.GetValue();
        }

        private void UpdateBinding()
        {
            if (this.uiDayter != null && this.PART_TURNOFFDATE != null &&
                this.PART_TURNONDATE != null && this.PART_TURNOFFTIME != null &&
                this.PART_TURNONTIME != null && this.uiSaving != null &&
                this._value != null && this.uiMonther.SelectedIndex != -1)
            {
                //if (this.StartWork != null)
                //try
                //{
                //    StartWork();
                //}
                //catch(Exception ex)

                //{
                //    throw;
                //}



                this.uiDayter.DataContext = this._value.Month[this.uiMonther.SelectedIndex].Days.Where((w) =>
                { return w.isVisible; }).Select((s) => { return s; }).ToList();
                this.PART_TURNOFFDATE.DataContext = this._value.YearSaving;
                this.PART_TURNONDATE.DataContext = this._value.YearSaving;
                /*Заглушка на экономию*/
                this.PART_TURNOFFTIME.DataContext = this._value.MonthSaving.TurnOffTime;
                this.PART_TURNONTIME.DataContext = this._value.MonthSaving.TurnOnTime;

                this.uiSaving.DataContext = this._value;



                //this.uiLatitude.DataContext = this.Latitude;
                //this.uiLongitude.DataContext = this.Longitude;
                //this.uiCityList.DataContext = this.CityList;
                //this.uiCityList.SelectedValue = this.SelectedCity;
                //this.uiCalculateButton.DataContext = this.CalculateScheduleCommand;
                //if (this.StopWork != null)
                //    this.StopWork();
            }
        }
        #endregion




        public ushort[] Value
        {
            get
            {
                return this.GetGraphicValue();
            }
            set
            {
                if (value != null)
                {
                    this.SetGraphicValue(value);
                }
            }
        }


        public ushort[] NullValue = new ushort[770];


        public async Task ClearAll()
        {
            for (int i = 0; i < 770; i++)
            {
                NullValue[i] = 0xFFFF;
            }

            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = false;
            {
                try
                {
                    ushort baseAddr = 0;
                    if (Name == "uiLightingSchedule")
                    {
                        baseAddr = 0x8500;
                    }

                    if (Name == "uiBacklightSchedule")
                    {
                        baseAddr = 0x8802;
                    }
                    if (Name == "uiIlluminationSchedule")
                    {
                        baseAddr = 0x8B04;
                    }

                    if (Name == "uiEnergySchedule")
                    {
                        baseAddr = 0x8E06;
                    }


                    for (ushort i = 0; i < 700; i += 60)
                    {
                        var r = NullValue.Skip(i).Take(60).ToArray();
                        await RTUConnectionGlobal.SendDataByAddressAsync(1,
                        (ushort)(baseAddr + i), r);

                    }

                    await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(baseAddr + 720), NullValue.Skip(720).Take(50).ToArray());


                    await ReadScheduleData(baseAddr);
                    UpdateBinding();
                    await UpdateState();
                    ExportComplete(true);
                }


                catch
                {
                    ExportComplete(false);
                }


            }
            uiExport.IsEnabled = uiImport.IsEnabled = uiOpen.IsEnabled = uiSave.IsEnabled = uiClearAll.IsEnabled = true;
        }

        private async void uiClearAll_Click(object sender, RoutedEventArgs e)
        {

            await ClearAll();

            if (this.ShowMessage != null)
            {

                if (Name == "uiLightingSchedule")
                {

                    this.ShowMessage("График освещения стерт.",
                        "Запись графика в устройство", MessageBoxImage.Information);
                }

                if (Name == "uiBacklightSchedule")
                {

                    this.ShowMessage("График подсветки стерт.",
                        "Запись графика в устройство", MessageBoxImage.Information);
                }
                if (Name == "uiIlluminationSchedule")
                {

                    this.ShowMessage("График иллюминации стерт.",
                        "Запись графика в устройство", MessageBoxImage.Information);
                }

                if (Name == "uiEnergySchedule")
                {

                    this.ShowMessage("График энергосбережения стерт",
                        "Запись графика в устройство", MessageBoxImage.Information);
                }


            }
        }


        private void OnCalculateScheduleCommand()
        {
            this._value = GraphicValue.SetSolarValue(this.InitializeGraphic(), Latitude, Longitude);
            this.UpdateBinding();
        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void uiCityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = e.Source as ComboBox;
            CityCoordinates cc = CoordinatesDictionary.ElementAt(GetCityIndex(cb.SelectedValue.ToString())).Value;
            Latitude = cc.Latitude;
            Longitude = cc.Longitude;
            uiLatitude.Text = cc.Latitude.ToString();
            uiLongitude.Text = cc.Longitude.ToString();
        }
    }
}
