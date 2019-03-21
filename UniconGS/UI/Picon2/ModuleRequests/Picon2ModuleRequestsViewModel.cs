using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Globalization;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using UniconGS.Enums;
using UniconGS.UI.Picon2.ModuleRequests.Resources;
using UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification;
using System.Threading;
using System.ComponentModel;
using Innovative.SolarCalculator;
using System.Collections;
using UniconGS.Source;


namespace UniconGS.UI.Picon2.ModuleRequests
{
    public class Picon2ModuleRequestsViewModel : BindableBase
    {
        #region [Private fields]
        private ObservableCollection<string> _moduleListForUI;
        private ObservableCollection<string> _moduleRequestsForUIList;
        private ObservableCollection<string> _moduleRequestsGeneratedFromUI;
        private ObservableCollection<ModuleRequest> _requestsFromDevice;
        private ObservableCollection<ModuleRequest> _requestsToWrite;
        private ushort _requestCount;
        private ushort _requestCountFromDevice;
        private ObservableCollection<string> _moduleList;
        private ModuleTypeList _moduleTypes;
        private ImageSRCList _imageSRC;
        private byte _msdCount;
        private byte _mrvCount;
        private byte _msaCount;
        private ObservableCollection<string> _imageSRCList;
        private ICommand _breakpointTestCommand;
        private ICommand _generateRequests;
        private ICommand _readFromDevice;
        private ICommand _writeToDevice;
        private ICommand _openFile;
        private ICommand _saveFile;
        private ICommand _deleteSlave;
        private bool _isToggleCrate918Checked;
        private string _crateID;
        private ObservableCollection<bool?> _moduleErrors;

        private bool _isAutonomus;
        #endregion

        #region [CONST]
        /// <summary>
        /// Адрес, по которому пишется число запросов к модулям
        /// </summary>
        private const ushort REQUEST_COUNT_ADDRESS = 0x300E;
        /// <summary>
        /// Адрес начала области памяти под запросы к модулям
        /// </summary>
        private const ushort MODULE_REQUEST_START_ADDRESS = 0x3080;
        /// <summary>
        /// Адрес модуля связи с верхом
        /// </summary>
        private const ushort UPPER_MODULE_ADDRESS = 0x3004;
        /// <summary>
        /// Адрес модуля связи с низом
        /// </summary>
        private const ushort LOWER_MODULE_ADDRESS = 0x3009;
        /// <summary>
        /// Адрес начала области памяти запросов к подчиненным устройствам
        /// </summary>
        private const ushort SLAVE_MODULE_REQUEST_START_ADDRESS = 0x3180;
        /// <summary>
        /// Адрес, по которому пишется число запросов к подчиненным устройствам
        /// </summary>
        private const ushort LOWER_MODULE_REQUEST_TO_SLAVES_COUNT_ADDRESS = 0x300F;

        private const string DECLARATION_VERSION = "1.0";
        private const string DECLARATION_ENCODING = "utf-8";
        #endregion

        #region [Properties]
        /// <summary>
        /// SortedList из всех типов модулей в паре ключ-значение, где ключ - строка кириллицей, значение - цифровое значение enum для дальнейшей обработки
        /// </summary>
        public ModuleTypeList ModuleTypes
        {
            get
            {
                return _moduleTypes;
            }
        }
        /// <summary>
        /// SortedList из всех типов модулей в паре ключ-значение, где ключ - цифровое значение enum, значение - строка--ссылка на изображение
        /// </summary>
        public ImageSRCList ImageSRC
        {
            get { return _imageSRC; }
        }
        /// <summary>
        /// Список модулей для комбобокса в UI
        /// </summary>
        public ObservableCollection<string> ModuleList
        {
            get { return _moduleList; }
            set
            {
                _moduleList = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Содержит список модулей, выбранных в UI, изначально весь список состоит из "Заглушек"
        /// </summary>
        public ObservableCollection<string> ModuleListForUI
        {
            get { return _moduleListForUI; }
            set
            {
                _moduleListForUI = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Содержит список запросов, выводимый на экран
        /// </summary>
        public ObservableCollection<string> ModuleRequestsForUIList
        {
            get { return _moduleRequestsForUIList; }
            set
            {
                _moduleRequestsForUIList = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Содержит список сгенерированных запросов, выводимый на экран
        /// </summary>
        public ObservableCollection<string> ModuleRequestsGeneratedFromUI
        {
            get { return _moduleRequestsGeneratedFromUI; }
            set
            {
                _moduleRequestsGeneratedFromUI = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Коллекция запросов для записи в устройство
        /// </summary>
        public ObservableCollection<ModuleRequest> RequestsToWrite
        {
            get { return _requestsToWrite; }
            set
            {
                _requestsToWrite = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Коллекция запросов из устройства
        /// </summary>
        public ObservableCollection<ModuleRequest> RequestsFromDevice
        {
            get { return _requestsFromDevice; }
            set
            {
                _requestsFromDevice = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Список ссылок на изображения модулей, должен быть привязан к выбраным модулям в UI
        /// </summary>
        public ObservableCollection<string> ImageSRCList
        {
            get { return _imageSRCList; }
            set
            {
                _imageSRCList = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Количество запросов. сгенерированных по схеме
        /// </summary>
        public ushort RequestCount
        {
            get { return _requestCount; }
            set
            {
                _requestCount = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Количество запросов, вычитанных из устройства
        /// </summary>
        public ushort RequestCountFromDevice
        {
            get { return _requestCountFromDevice; }
            set
            {
                _requestCountFromDevice = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Количество модулей МСД (для расчета адресов в базе)
        /// </summary>
        public byte MSDCount
        {
            get { return _msdCount; }
            set
            {
                _msdCount = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Количество модулей МРВ (для расчета адресов в базе)
        /// </summary>
        public byte MRVCount
        {
            get { return _mrvCount; }
            set
            {
                _mrvCount = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Количество модулей МСА (для расчета адресов в базе)
        /// </summary>
        public byte MSACount
        {
            get { return _msaCount; }
            set
            {
                _msaCount = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Переключатель вида крейта
        /// </summary>
        public bool IsToggleCrate918Checked
        {
            get { return _isToggleCrate918Checked; }
            set
            {
                _isToggleCrate918Checked = value;
                if (!value)
                {
                    try
                    {
                        for (int i = 9; i < 16; i++)
                        {
                            ModuleListForUI[i] = ModuleTypes.ModuleList.Keys.First();
                        }
                        OnGenerateRequestsCommand();
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        OnGenerateRequestsCommand();
                    }
                    catch
                    {

                    }
                }
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Тип крейта
        /// </summary>
        public string CrateID
        {
            get { return _crateID; }
            set
            {
                _crateID = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Коллекция, содержащая ошибки модулей
        /// </summary>
        public ObservableCollection<bool?> ModuleErrors
        {
            get { return _moduleErrors; }
            set
            {
                _moduleErrors = value;
                RaisePropertyChanged();
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
        #endregion

        #region [NavigateCommands]
        /// <summary>
        /// Кнопка для отладки
        /// </summary>
        public ICommand BreakpointTestCommand => this._breakpointTestCommand ??
            (this._breakpointTestCommand = new DelegateCommand(OnBreakpointTestCommand));
        //TODO: уточнить какие запросы сохранять в файл
        /// <summary>
        /// Сохранить сгенерированные(или не сгенерированные, уточнить) запросы в файл
        /// </summary>
        public ICommand SaveFileCommand => this._saveFile ??
            (this._saveFile = new DelegateCommand(OnSaveFileCommand));
        /// <summary>
        /// Открыть файл с запросами
        /// </summary>
        public ICommand OpenFileCommand => this._openFile ??
            (this._openFile = new DelegateCommand(OnOpenFileCommand));
        /// <summary>
        /// Команда генерации запросов к модулям
        /// </summary>
        public ICommand GenerateRequestsCommand => this._generateRequests ??
            (this._generateRequests = new DelegateCommand(OnGenerateRequestsCommand));
        /// <summary>
        /// Команда чтения запросов из устройства
        /// </summary>
        public ICommand ReadFromDeviceCommand => this._readFromDevice ??
            (this._readFromDevice = new DelegateCommand(OnReadFromDeviceCommand));
        /// <summary>
        /// Команда записи запросов в устройство
        /// </summary>
        public ICommand WriteToDeviceCommand => this._writeToDevice ??
            (this._writeToDevice = new DelegateCommand(OnWriteToDeviceCommand));
        /// <summary>
        /// Удалить запрос к подчиненному устройству
        /// </summary>
        public ICommand DeleteSlaveCommand => this._deleteSlave ??
            (this._deleteSlave = new DelegateCommand(OnDeleteSlaveCommand));
        #endregion

        #region [Ctor]
        public Picon2ModuleRequestsViewModel()
        {
            this._moduleTypes = new ModuleTypeList();
            this._moduleList = new ObservableCollection<string>();
            this._moduleListForUI = new ObservableCollection<string>();
            this._imageSRC = new ImageSRCList();
            this._imageSRCList = new ObservableCollection<string>();
            this._moduleRequestsForUIList = new ObservableCollection<string>();
            this._requestsFromDevice = new ObservableCollection<ModuleRequest>();
            this._requestsToWrite = new ObservableCollection<ModuleRequest>();
            this.ModuleRequestsGeneratedFromUI = new ObservableCollection<string>();
            this.RequestCount = 0;
            this.RequestCountFromDevice = 0;
            this.MRVCount = 0;
            this.MSDCount = 0;
            this.MSACount = 0;
            this.CrateID = string.Empty;
            this.IsToggleCrate918Checked = false;
            this.ModuleErrors = new ObservableCollection<bool?>();
            InitializeModuleList();
            InitializeImageList();
            InitializeModuleErrors();
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// инициализация коллекции ошибок модулей
        /// </summary>
        private void InitializeModuleErrors()
        {
            for (int i = 0; i < 17; i++)
            {
                ModuleErrors.Add(null);
            }
        }
        /// <summary>
        /// инициализация элементов списка для UI
        /// </summary>
        private void InitializeModuleList()
        {
            //ModuleList = ModuleTypes.ModuleList.Keys.ToList();
            foreach (var item in ModuleTypes.ModuleList.Keys)
            {
                ModuleList.Add(item);
            }
            for (byte i = 0; i < 16; i++)
            {
                ModuleListForUI.Add(ModuleList.First());
            }
        }
        /// <summary>
        /// инициализация списка изображений
        /// </summary>
        private void InitializeImageList()
        {
            for (byte i = 0; i < 16; i++)
            {
                ImageSRCList.Add(GetImageSRC((byte)ModuleSelectionEnum.MODULE_EMPTY));
            }
            ImageSRCList.Add(GetImageSRC((byte)ModuleSelectionEnum.MODULE_SERVICE_POWERSUPPLY));
            ImageSRCList.Add(GetImageSRC((byte)ModuleSelectionEnum.MODULE_SERVICE_CPU));

        }
        /// <summary>
        ///  Получить численное значение типа модуля по его имени, см ModuleSelectionEnum
        /// </summary>
        /// <param name="ModuleName"></param>
        /// <returns></returns>
        public byte GetModuleType(string ModuleName)
        {
            if (ModuleTypes.ModuleList.ContainsKey(ModuleName))
            {
                return (byte)ModuleTypes.ModuleList.ElementAt(ModuleTypes.ModuleList.IndexOfKey(ModuleName)).Value;
            }
            else
                return 0;
        }
        /// <summary>
        /// Получить строковое значение ссылки на изображение по его типу
        /// </summary>
        /// <param name="ModuleEnum"></param>
        /// <returns></returns>
        public string GetImageSRC(byte ModuleEnum)
        {
            if (ImageSRC.ImageList.ContainsKey(ModuleEnum))
            {
                return ImageSRC.ImageList.ElementAt(ImageSRC.ImageList.IndexOfKey(ModuleEnum)).Value;
            }
            else
                return "";
        }
        /// <summary>
        /// Зажигает индикаторы ошибок модулей
        /// </summary>
        /// <param name="val"></param>
        public async void SetModuleErrors()
        {
            try
            {
                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x1102, 1);
                BitArray array = Converter.GetBitsFromWord(value[0]);
                if (IsToggleCrate918Checked)
                {
                    //костыли по причине того, что всякие люди не могут договориться между собой иделают все по-своему
                    ModuleErrors[0] = false;
                    for (int i = 0; i < 16; i++)
                    {
                        ModuleErrors[i + 1] = array[i];
                    }
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ModuleErrors[i] = array[i];
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Ошибка чтения состояния модулей", "Ошибка", MessageBoxImage.Error);
            }

        }

        public void SetAutonomus()
        {
            for (int i = 0; i < 17; i++)
            {
                ModuleErrors[i] = null;
            }
            IsAutonomus = true;
        }
        #endregion

        #region [UICommands]
        /// <summary>
        /// команда для отладки и проверки значений свойств, удалить по окончании
        /// </summary>
        private async void OnBreakpointTestCommand()
        {
            //var win = new Picon2CommunicationModule910SeriesView();
            //win.DataContext = new Picon2CommunicationModule910SeriesViewModel(0x0E, 0x00, ModuleListForUI[0x00]);
            //win.Show();
        }
        /// <summary>
        /// Чтение запросов из устройства
        /// </summary>
        private async void OnReadFromDeviceCommand()
        {
            //херня, знаю, но смысл один и тот же. надо просто сделать недоступными кнопки на вермя чтения, а обращаться через имя я не хочу
            IsAutonomus = true;
            try
            {
                this.RequestsFromDevice.Clear();
                this.ModuleRequestsForUIList.Clear();
                // число запросов к модулям хранится по адресу 0х300Е, читаем 1 слово(2 байта)
                ushort[] RC = await RTUConnectionGlobal.GetDataByAddress(1, REQUEST_COUNT_ADDRESS, 1);
                RequestCountFromDevice = RC[0];
                // начальный адрес
                ushort currentAddress = MODULE_REQUEST_START_ADDRESS;
                for (byte i = 0; i < RC[0]; i++)
                {
                    RequestsFromDevice.Add(new ModuleRequest(await RTUConnectionGlobal.GetDataByAddress(1, currentAddress, 4)));
                    ModuleRequestsForUIList.Add(RequestsFromDevice[i].UIRequest);
                    currentAddress += 4;
                }
                ShowMessage("Чтение запросов прошло успешно!", "Информация", MessageBoxImage.Information);
                AnalyzeModulesFromDevice();
            }
            catch (Exception ex)
            {
                ShowMessage("При чтении запросов произошла ошибка.", "Внимание", MessageBoxImage.Warning);
            }
            IsAutonomus = false;
        }
        /// <summary>
        /// Анализ модулей, прочитанных из устройства + их расстановка по местам
        /// </summary>
        private async void AnalyzeModulesFromDevice()
        {
            byte _firstModulePos;
            if (IsToggleCrate918Checked)
            {
                _firstModulePos = 0;
            }
            else
            {
                _firstModulePos = 1;
            }
            /// читаем модуль с верхом
            try
            {
                ushort[] upperModule = await RTUConnectionGlobal.GetDataByAddress(1, UPPER_MODULE_ADDRESS, 5);
                if (upperModule != null)
                {
                    byte[] byteArrayUpper = ArrayExtension.UshortArrayToByteArray(upperModule);
                    string _typeAddressUpper = Converters.Convert.ConvertFromDecToHexStr(byteArrayUpper[1]);
                    byte _typeUpper = Converters.Convert.ConvertFromHexToDec((byte)Int32.Parse(_typeAddressUpper.First().ToString(), System.Globalization.NumberStyles.HexNumber));
                    byte _addressUpper = Converters.Convert.ConvertFromHexToDec((byte)Int32.Parse(_typeAddressUpper.Last().ToString(), System.Globalization.NumberStyles.HexNumber));
                    string upperName = "Заглушка";
                    if (_typeUpper != 0)
                    {
                        if (_typeUpper == 0x0E)
                        {
                            //немного костыли, но че уж поделать, если таски возникают после того, как ниписано практически все
                            Config915Series cfg915 = new Config915Series(byteArrayUpper[5]);
                            if (cfg915.ModbusSpeed == 115200)
                                upperName = "MС915";
                            if (cfg915.ModbusSpeed == 19200)
                                upperName = "MС917";
                        }
                        if (_typeUpper == 0x0F)
                        {
                            upperName = "MС911";
                        }
                        this.ModuleListForUI[_addressUpper - _firstModulePos] = upperName;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("При чтении запроса модуля связи с верхом произошла ошибка.", "Внимание", MessageBoxImage.Warning);
            }
            /// читаем модуль с низом
            try
            {
                ushort[] slaveCount = await RTUConnectionGlobal.GetDataByAddress(1, LOWER_MODULE_REQUEST_TO_SLAVES_COUNT_ADDRESS, 1);
                if (slaveCount[0] != 0)
                {
                    ushort[] lowerModule = await RTUConnectionGlobal.GetDataByAddress(1, LOWER_MODULE_ADDRESS, 5);
                    if (lowerModule != null)
                    {
                        byte[] byteArrayLower = ArrayExtension.UshortArrayToByteArray(lowerModule);
                        string _typeAddressLower = Converters.Convert.ConvertFromDecToHexStr(byteArrayLower[1]);
                        byte _typeLower = Converters.Convert.ConvertFromHexToDec((byte)Int32.Parse(_typeAddressLower.First().ToString(), System.Globalization.NumberStyles.HexNumber));
                        byte _addressLower = Converters.Convert.ConvertFromHexToDec((byte)Int32.Parse(_typeAddressLower.Last().ToString(), System.Globalization.NumberStyles.HexNumber));
                        string lowerName = "Заглушка";
                        if (_typeLower == 0x0E)
                        {
                            lowerName = "Люксметр";
                        }
                        this.ModuleListForUI[_addressLower - _firstModulePos] = lowerName;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("При чтении запроса модуля связи с низом произошла ошибка.", "Внимание", MessageBoxImage.Warning);
            }
            ///модули

            for (int i = 0; i < RequestsFromDevice.Count(); i++)
            {
                this.ModuleListForUI[RequestsFromDevice[i].CrateAddress - _firstModulePos] = GetModuleNameFromType(RequestsFromDevice[i].Type, RequestsFromDevice[i].ParameterCount);
            }
            ///верх


            ///низ

        }
        /// <summary>
        /// Возвращает имя модуля по его типу и количеству параметров
        /// </summary>
        /// <param name="_type">Тип модуля</param>
        /// <param name="_paramCount">Число параметров</param>
        /// <returns></returns>
        private string GetModuleNameFromType(byte _type, byte _paramCount)
        {
            switch (_type)
            {
                case (0x08):
                    {
                        return "MСД980";
                    }
                case (0x0A):
                    {
                        return "MРВ960";
                    }
                case (0x0B):
                    {
                        return "MРВ980";
                    }
                case (0x04):
                    {
                        return "MСА961";
                    }
                case (0x05):
                    {
                        if (_paramCount == 0x0C)
                        {
                            return "MСА962";
                        }
                        if (_paramCount == 0x0E)
                        {
                            return "MИИ901";
                        }
                        break;
                    }
                case (0x0E):
                    {
                        return "Счетчик";
                    }
            }
            return "Заглушка";
        }
        /// <summary>
        /// Удаление подчиненного устройства
        /// </summary>
        private async void OnDeleteSlaveCommand()
        {
            try
            {
                ushort[] LRC = new ushort[] { 0 };
                await RTUConnectionGlobal.SendDataByAddressAsync(1, LOWER_MODULE_REQUEST_TO_SLAVES_COUNT_ADDRESS, LRC);

                ushort[] req915lower = new ushort[] { 0, 0, 0, 0 };
                await RTUConnectionGlobal.SendDataByAddressAsync(1, LOWER_MODULE_ADDRESS, req915lower);

                ShowMessage("Удаление запроса прошло успешно.", "Информация", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage("Произошла ошибка.", "Внимание", MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Запись запросов в устройство
        /// </summary>
        private async void OnWriteToDeviceCommand()
        {
            try
            {
                ushort[] RC = new ushort[] { RequestCount };
                await RTUConnectionGlobal.SendDataByAddressAsync(1, REQUEST_COUNT_ADDRESS, RC);
                //заглушка
                //RequestsToWrite = RequestsFromDevice;
                ushort currentAddress = MODULE_REQUEST_START_ADDRESS;
                for (byte i = 0; i < RC[0]; i++)
                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, currentAddress, RequestsToWrite[i].RequestToDevice);
                    currentAddress += 4;
                }
                ShowMessage("Запись прошла успешно!", "Информация", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage("При записи запросов произошла ошибка.", "Внимание", MessageBoxImage.Warning);
            }
        }
        /// <summary>
        ///  Генерация запросов по данным из UI
        /// </summary>
        public void OnGenerateRequestsCommand()
        {
            try
            {
                MSDCount = 0;
                MSACount = 0;
                MRVCount = 0;

                byte MSDUsed = 0;
                byte MSAUsed = 0;
                byte MRVUsed = 0;

                RequestsToWrite.Clear();
                ModuleRequestsGeneratedFromUI.Clear();
                CalculateRequestCountForGeneration();

                // сначала смотрим количество модулей МСД/МСА/МРВ
                for (byte i = 0; i < RequestCount; i++)
                {
                    switch (GetModuleType(ModuleListForUI[i]))
                    {
                        case (byte)ModuleSelectionEnum.MODULE_MSD980:
                            {
                                MSDCount++;
                                break;
                            }
                        case (byte)ModuleSelectionEnum.MODULE_MSA961:
                            {
                                MSACount++;
                                break;
                            }
                        case (byte)ModuleSelectionEnum.MODULE_MSA962:
                            {
                                MSACount++;
                                break;
                            }
                        case (byte)ModuleSelectionEnum.MODULE_MII901:
                            {
                                MSACount++;
                                break;
                            }
                        case (byte)ModuleSelectionEnum.MODULE_MRV960:
                            {
                                MRVCount++;
                                break;
                            }
                        case (byte)ModuleSelectionEnum.MODULE_MRV980:
                            {
                                MRVCount++;
                                break;
                            }
                    }
                }
                // заполняем список запросов для записи в устройство 
                // есть нюанс, который я узнал, когда уже почти полностью сделал этот модуль
                // оказывается активные еще и 911-е крейты, а не только 918, а в 911-х нумерация запросов идет с МЦП
                // т.е. МЦП всегда будет на позиции 0х00, а первый модуль будет уже на позиции 0х01, когда в 918м вся  
                // нумерация модулей идет после МЦП и первый модуль будет на позиции 0х00
                // мне не хочется что-то тут переделывать, так что будет во так
                byte _firstModulePos;
                if (IsToggleCrate918Checked)
                {
                    _firstModulePos = 0;
                }
                else
                {
                    _firstModulePos = 1;
                }
                for (byte i = 0; i < RequestCount; i++)
                {
                    if (GetModuleType(ModuleListForUI[i]) != (byte)ModuleSelectionEnum.MODULE_EMPTY)
                    {
                        switch (GetModuleType(ModuleListForUI[i]))
                        {
                            case (byte)ModuleSelectionEnum.MODULE_MSD980:
                                {
                                    var req = new MSD980ModuleSpecification();
                                    RequestsToWrite.Add(new ModuleRequest(
                                        0x00,
                                        req.ModuleType,
                                        (byte)(i + _firstModulePos),
                                        req.Command,
                                        req.ModuleParameterAddress,
                                        (ushort)(req.FirstModuleDatabaseAddress + MSDUsed * req.ParameterCount),
                                        req.ParameterCount));
                                    MSDUsed++;
                                    break;
                                }
                            case (byte)ModuleSelectionEnum.MODULE_MSA961:
                                {
                                    var req = new MSA961ModuleSpecification();
                                    RequestsToWrite.Add(new ModuleRequest(
                                        0x00,
                                        req.ModuleType,
                                        (byte)(i + _firstModulePos),
                                        req.Command,
                                        req.ModuleParameterAddress,
                                        (ushort)(req.FirstModuleDatabaseAddress + MSAUsed * req.ParameterCount),
                                        req.ParameterCount));
                                    MSAUsed++;
                                    break;
                                }
                            case (byte)ModuleSelectionEnum.MODULE_MSA962:
                                {
                                    var req = new MSA962ModuleSpecification();
                                    RequestsToWrite.Add(new ModuleRequest(
                                        0x00,
                                        req.ModuleType,
                                        (byte)(i + _firstModulePos),
                                        req.Command,
                                        req.ModuleParameterAddress,
                                        (ushort)(req.FirstModuleDatabaseAddress + MSAUsed * req.ParameterCount),
                                        req.ParameterCount));
                                    MSAUsed++;
                                    break;
                                }
                            case (byte)ModuleSelectionEnum.MODULE_MII901:
                                {
                                    var req = new MII901ModuleSpecification();
                                    RequestsToWrite.Add(new ModuleRequest(
                                        0x00,
                                        req.ModuleType,
                                        (byte)(i + _firstModulePos),
                                        req.Command,
                                        req.ModuleParameterAddress,
                                        (ushort)(req.FirstModuleDatabaseAddress + MSAUsed * req.ParameterCount),
                                        req.ParameterCount));
                                    MSAUsed++;
                                    break;
                                }
                            case (byte)ModuleSelectionEnum.MODULE_MRV960:
                                {
                                    var req = new MRV960ModuleSpecification();
                                    RequestsToWrite.Add(new ModuleRequest(
                                        0x00,
                                        req.ModuleType,
                                        (byte)(i + _firstModulePos),
                                        req.Command,
                                        req.ModuleParameterAddress,
                                        (ushort)(req.FirstModuleDatabaseAddress + MRVUsed * req.ParameterCount),
                                        req.ParameterCount));
                                    MRVUsed++;
                                    break;
                                }
                            case (byte)ModuleSelectionEnum.MODULE_MRV980:
                                {
                                    var req = new MRV980ModuleSpecification();
                                    RequestsToWrite.Add(new ModuleRequest(
                                        0x00,
                                        req.ModuleType,
                                        (byte)(i + _firstModulePos),
                                        req.Command,
                                        req.ModuleParameterAddress,
                                        (ushort)(req.FirstModuleDatabaseAddress + MRVUsed * req.ParameterCount),
                                        req.ParameterCount));
                                    MRVUsed++;
                                    break;
                                }
                            case (byte)ModuleSelectionEnum.MODULE_MS915C:
                                {
                                    var req = new MS915CModuleSpecification();
                                    RequestsToWrite.Add(new ModuleRequest(
                                        0x00,
                                        req.ModuleType,
                                        (byte)(i + _firstModulePos),
                                        req.Command,
                                        req.ModuleParameterAddress,
                                        (ushort)(req.FirstModuleDatabaseAddress),
                                        req.ParameterCount));
                                    break;
                                }
                                //case (byte)ModuleSelectionEnum.MODULE_MS915L:
                                //    {
                                //        WriteLuxmetrRequest(i + _firstModulePos);
                                //        break;
                                //    }
                        }
                    }
                    else
                    {

                    }
                }
                //делается для случая, если посреди модулей стоит заглушка
                RequestCount = (ushort)RequestsToWrite.Count();
                // заполняем листбокс
                for (byte i = 0; i < RequestsToWrite.Count; i++)
                {
                    ModuleRequestsGeneratedFromUI.Add(RequestsToWrite[i].UIRequest);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("При генерации запросов к модулям произошла ошибка.", "Внимание", MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Запись запросов для люксметра (обожаю, когда вылазят новые требования, когда я уже всю логику взаимодействия продумал)
        /// </summary>
        /// <param name="_pos"></param>
        public async void WriteLuxmetrRequest(int _pos)
        {
            List<byte> byteArr = new List<byte>();
            List<ushort> confToDevice = new List<ushort>();
            Config915Series conf915 = new Config915Series(115200, false, false, false, false);
            byteArr.Add(100);
            byteArr.Add((byte)((0x0E << 4) + _pos));
            confToDevice.Add(ArrayExtension.ByteArrayToUshortArray(byteArr.ToArray()).First());
            confToDevice.Add(200);
            confToDevice.Add(conf915.Config);
            confToDevice.Add(40);
            confToDevice.Add(20);

            ushort[] req915lower = confToDevice.ToArray();

            byteArr.Clear();
            confToDevice.Clear();

            //запрос для люксметра, пишется в область памяти запросов к подчиненным устройствам
            byteArr.Add(0x00);//период
            byteArr.Add(0x01);//адрес+тип модуля
            byteArr.Add(0x00);//команда
            byteArr.Add(0x00);//адрес пар-ра в модуле 1
            byteArr.Add(0x00);//адрес пар-ра в модуле 2
            byteArr.Add(0x06);//адрес пар-ра в базе 1
            byteArr.Add(0x40);//ажрес пар-ра в базе 2
            byteArr.Add(0x01);//число пар-ров

            byte[] byteArray = byteArr.ToArray();

            ArrayExtension.SwapArrayItems(ref byteArray);

            confToDevice.AddRange(ArrayExtension.ByteArrayToUshortArray(byteArray));

            ushort[] reqLux = confToDevice.ToArray();
            ModuleRequest mr = new ModuleRequest(reqLux);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Будет произведена запись со следующими параметрами:");
            sb.AppendLine("Модуль связи: МС915 с низом на позицию " + _pos.ToString());
            sb.AppendLine("Ожидание ответа: 200 (*0,5 мс)");
            sb.AppendLine("Скорость: 115200");
            sb.AppendLine("Биты данных: 8 бит");
            sb.AppendLine("Паритет: нечет");
            sb.AppendLine("Паритет: нет");
            sb.AppendLine("Число стоп битов: 1 бит");
            sb.AppendLine("Таймаут ввода/вывода: 100 (*0,1 мс)");
            sb.AppendLine("Включение передачи: 40 (*0,1 мс)");
            sb.AppendLine("Выключение передачи: 20 (*0,1 мс)");

            sb.AppendLine("Также будет добавлен запрос к подчиненному устройтсву(люксметру):");
            sb.AppendLine(mr.UIRequest);
            sb.AppendLine("Продолжить?");

            if (MessageBox.Show(sb.ToString(), "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    ushort[] LRC = new ushort[] { 1 };//договор был изначально на одно устройство, так что ничего придумывать не буду
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, LOWER_MODULE_REQUEST_TO_SLAVES_COUNT_ADDRESS, LRC);
                    Thread.Sleep(100);
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, LOWER_MODULE_ADDRESS, req915lower);
                    Thread.Sleep(100);
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, SLAVE_MODULE_REQUEST_START_ADDRESS, reqLux);
                    Thread.Sleep(100);
                    ShowMessage("Запись произведена.", "Информация", MessageBoxImage.Information);
                }
                catch
                {
                    ShowMessage("При записи в устройство произошла ошибка.", "Внимение", MessageBoxImage.Warning);
                }
            }
            else
            {
                ShowMessage("Запись не была произведена", "Информация", MessageBoxImage.Information);
            }

        }
        /// <summary>
        /// Показ информации в окне
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="image"></param>
        private void ShowMessage(string message, string caption, MessageBoxImage image)
        {
            System.Windows.MessageBox.Show(message, caption, MessageBoxButton.OK, image);
        }
        /// <summary>
        /// Расчет количества запросов для генерирования (считает справа до первой НЕзаглушки)
        /// </summary>
        private void CalculateRequestCountForGeneration()
        {
            try
            {
                for (byte i = ((byte)(ModuleListForUI.Count - 1)); i < 16; i--)
                {
                    if (GetModuleType(ModuleListForUI[i]) != (byte)ModuleSelectionEnum.MODULE_EMPTY)
                    {
                        RequestCount = (byte)(i + 1);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Открыть файл запросов к модулям, формат *.mrf
        /// </summary>
        private void OnOpenFileCommand()
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Файл запросов к модулям|*.mrf",
                Title = "Открыть файл запросов"
            };
            if (fileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            for (byte i = 0; i < 16; i++)
            {
                ModuleListForUI[i] = ModuleTypes.ModuleList.Keys.First();
            }
            try
            {

                var doc = XDocument.Load(fileDialog.FileName);
                byte modulePosition = 0;
                foreach (XElement xElement in doc.Root.Elements())
                {
                    if (xElement.Name.ToString().Equals("Device"))
                    {
                        if (xElement.Value.ToString(CultureInfo.InvariantCulture) != "PICON2")
                        {
                            new Exception("Файл от устройства: " +
                                          xElement.Value.ToString(CultureInfo.InvariantCulture) +
                                          ". Требуется файл, соответствующий устройству: PICON2");
                            return;
                        }
                    }
                    if (xElement.Name.ToString().Equals("RequestCount"))
                    {
                        this.RequestCount = byte.Parse(xElement.Value.ToString(CultureInfo.InvariantCulture));
                    }
                    if (xElement.Name.ToString().Equals("ModuleType"))
                    {
                        this.ModuleListForUI[modulePosition] = xElement.Value.ToString();
                        modulePosition++;
                    }
                }
                ShowMessage("Файл открыт успешно!", "Информация", MessageBoxImage.Information);
                OnGenerateRequestsCommand();
            }
            catch
            {
                ShowMessage("При открытии файла произошла ошибка!", "Внимание", MessageBoxImage.Warning);
            }

        }
        /// <summary>
        /// Сохранить файл запросов к модулям, формат *.mrf
        /// </summary>
        private void OnSaveFileCommand()
        {
            var fileDialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "Файл запросов к модулям|*.mrf",
                Title = "Сохранение файла запросов"
            };
            if (fileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            //try
            //{
            //    var xDoc = new XDocument(new XDeclaration(DECLARATION_VERSION, DECLARATION_ENCODING, ""));
            //    var root = new XElement("Device", "PICON2");
            //    root.Add(new XElement("RequestCount", this.RequestCount));
            //    for (byte i = 0; i < RequestCount; i++)
            //    {
            //        root.Add(new XElement("ModuleType", ModuleListForUI[i]));
            //    }
            //    xDoc.Add(root);
            //    xDoc.Save(fileDialog.FileName);
            //    ShowMessage("Сохранено!", "Информация", MessageBoxImage.Information);
            //}
            try
            {
                var xDoc = new XDocument(new XDeclaration(DECLARATION_VERSION, DECLARATION_ENCODING, ""));
                var root = new XElement("Device", "PICON2");
                root.Add(new XElement("RequestCount", this.RequestCount));
                for (byte i = 0; i < ModuleListForUI.Count; i++)
                {
                    root.Add(new XElement("ModuleType", ModuleListForUI[i]));
                }
                xDoc.Add(root);
                xDoc.Save(fileDialog.FileName);
                ShowMessage("Сохранено!", "Информация", MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                ShowMessage("При сохранении файла произошла ошибка", "Внимание", MessageBoxImage.Warning);
            }
        }
        #endregion

        #region [Converters]

        #endregion

    }
}
