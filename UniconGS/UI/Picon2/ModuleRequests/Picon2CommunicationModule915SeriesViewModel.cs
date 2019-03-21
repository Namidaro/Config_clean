using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification;

namespace UniconGS.UI.Picon2.ModuleRequests
{
    public class Picon2CommunicationModule915SeriesViewModel : BindableBase
    {
        #region [Private Fields]
        private string _title;
        private string _typeAddress;
        private byte _type;
        private byte _address;
        private ushort _responseAwait;
        private bool _isUpper;
        private List<long> _modbusSpeedList;
        private long _selectedSpeed;
        private bool _bitValues;
        private bool _parityOdd;
        private bool _parityExistence;
        private bool _stopBitsCount;
        private byte _ioTimeout;
        private ushort _transmitEnableDelay;
        private ushort _transmitDisableDelay;

        private bool _isCounter;
        private bool _isLuxmetr;

        private ushort[] _config;

        private ICommand _writeRequest;
        private ICommand _readRequest;
        private ICommand _closeWindow;
        #endregion

        #region [CONST]
        /// <summary>
        /// Стартовый адрес для запроса к модулю "с верхом"
        /// </summary>
        private const ushort UPPER_ADDRESS = 0x3004;
        /// <summary>
        /// Стартовый адрес для запроса к модулю "с низом"
        /// </summary>
        private const ushort LOWER_ADDRESS = 0x3009;
        /// <summary>
        /// Длина пакета для модуля связи
        /// </summary>
        private const ushort PACKAGE_LENGTH = 5;
        #endregion

        #region [Properties]
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Строка тип+адрес модуля для UI
        /// </summary>
        public string TypeAddress
        {
            get { return _typeAddress; }
            set
            {
                _typeAddress = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Байт типа модуля 
        /// </summary>
        public byte Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Байт адреса модуля
        /// </summary>
        public byte Address
        {
            get { return _address; }
            set
            {
                _address = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Ожидание ответа (0..65535)
        /// </summary>
        public ushort ResponseAwait
        {
            get { return _responseAwait; }
            set
            {
                _responseAwait = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Показатель работы модуля с верхним, либо нижним уровнем
        /// </summary>
        public bool IsUpper
        {
            get { return _isUpper; }
            set
            {
                _isUpper = value;
                ChangeTitle();
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Список скоростей обмена по Modbus
        /// </summary>
        public List<long> ModbusSpeedList
        {
            get { return _modbusSpeedList; }
            set
            {
                _modbusSpeedList = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Выбранное значение скорости обмена
        /// </summary>
        public long SelectedSpeed
        {
            get { return _selectedSpeed; }
            set
            {
                _selectedSpeed = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Чекбокс битов данных (8 бит / 7 бит)
        /// </summary>
        public bool BitValues
        {
            get { return _bitValues; }
            set
            {
                _bitValues = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Чекбокс паритета (нечет / чет)
        /// </summary>
        public bool ParityOdd
        {
            get { return _parityOdd; }
            set
            {
                _parityOdd = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Чекбокс паритета (есть / нет)
        /// </summary>
        public bool ParityExistence
        {
            get { return _parityExistence; }
            set
            {
                _parityExistence = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Чекбокс стоп битов (1 бит / 2 бита)
        /// </summary>
        public bool StopBitsCount
        {
            get { return _stopBitsCount; }
            set
            {
                _stopBitsCount = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Таймаут ввода/вывода (0..255)
        /// </summary>
        public byte IOTimeout
        {
            get { return _ioTimeout; }
            set
            {
                _ioTimeout = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Задержка включения передачи (0..65535)
        /// </summary>
        public ushort TransmitEnableDelay
        {
            get { return _transmitEnableDelay; }
            set
            {
                _transmitEnableDelay = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Задержка выключения передачи (0..65535)
        /// </summary>
        public ushort TransmitDisableDelay
        {
            get { return _transmitDisableDelay; }
            set
            {
                _transmitDisableDelay = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Подчиненным устройством выбран счетчик 
        /// </summary>
        public bool IsCounter
        {
            get { return _isCounter; }
            set
            {
                _isCounter = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Подчиненным устройством выбран люксметр
        /// </summary>
        public bool IsLuxmetr
        {
            get { return _isLuxmetr; }
            set
            {
                _isLuxmetr = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Массив слов с конфигурацией модуля связи для записи в устройство
        /// </summary>
        public ushort[] Config
        {
            get { return _config; }
            set
            {
                _config = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region [NavigateCommands]
        /// <summary>
        /// Команда записи конфигурации в устройство
        /// </summary>
        public ICommand WriteRequestCommand => this._writeRequest ??
            (this._writeRequest = new DelegateCommand(OnWriteRequestCommand));
        /// <summary>
        /// Команда чтения конфигурации из устройства
        /// </summary>
        public ICommand ReadRequestCommand => this._readRequest ??
            (this._readRequest = new DelegateCommand(OnReadRequestCommand));
        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public ICommand CloseWindowCommand => this._closeWindow ??
            (this._closeWindow = new DelegateCommand(OnCloseWindowCommand));
        #endregion

        #region [Ctor]
        /// <summary>
        /// Вьюмодель для модуля связи 915/917 в верхом/низом с инициализацией некоторых данных
        /// </summary>
        /// <param name="_typeIN">Тип модуля связи</param>
        /// <param name="_addressIN">Позиция на крейте</param>
        /// <param name="_isUpperIN">С верхом / с низом</param>
        /// <param name="_titleIN">Название модуля</param>
        public Picon2CommunicationModule915SeriesViewModel(byte _typeIN, byte _addressIN, string _titleIN)
        {
            Type = _typeIN;
            Address = _addressIN;
            TypeAddress = Converters.Convert.ConvertFromDecToHexStr((byte)((this.Type << 4) + this.Address)).ToString();
            this.Title = _titleIN + " с верхом";
            IsUpper = true;
            IsCounter = false;
            IsLuxmetr = false;
            ModbusSpeedList = new List<long>();
            InitializeModbusSpeedList();
            //SelectedSpeed = ModbusSpeedList.First();
            SelectedSpeed = 0;
            ResponseAwait = 0;
            BitValues = false;
            ParityOdd = false;
            ParityExistence = false;
            StopBitsCount = false;
            IOTimeout = 0;
            TransmitEnableDelay = 0;
            TransmitDisableDelay = 0;
            Config = new ushort[PACKAGE_LENGTH];
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Формирует массив слов, который будет записан в устройство
        /// </summary>
        /// <returns></returns>
        private ushort[] GenerateConfigForDevice()
        {
            List<byte> byteArr = new List<byte>();
            List<ushort> confToDevice = new List<ushort>();
            Config915Series conf915 = new Config915Series(SelectedSpeed, BitValues, ParityOdd, ParityExistence, StopBitsCount);

            SpreadTypeAddress();

            byteArr.Add(IOTimeout);
            byteArr.Add((byte)((Type << 4) + Address));
            confToDevice.Add(ArrayExtension.ByteArrayToUshortArray(byteArr.ToArray()).First());
            confToDevice.Add(ResponseAwait);
            confToDevice.Add(conf915.Config);
            confToDevice.Add(TransmitEnableDelay);
            confToDevice.Add(TransmitDisableDelay);

            return confToDevice.ToArray();
        }
        /// <summary>
        /// Читает массив слов из устройства и обновляет UI
        /// </summary>
        /// <param name="_configFormDevice"></param>
        private void AnalyzeConfigFromDevice(ushort[] _configFormDevice)
        {
            if (_configFormDevice == null)
                return;
            if (_configFormDevice.Count() == 5)
            {
                byte[] byteArray = ArrayExtension.UshortArrayToByteArray(_configFormDevice);
                IOTimeout = Converters.Convert.ConvertFromDecToHex(byteArray[0]);

                ResponseAwait = _configFormDevice[1];
                TransmitEnableDelay = _configFormDevice[3];
                TransmitDisableDelay = _configFormDevice[4];
                TypeAddress = Converters.Convert.ConvertFromDecToHexStr(byteArray[1]);
                IOTimeout = Converters.Convert.ConvertFromDecToHex(byteArray[0]);
                Config915Series config = new Config915Series(byteArray[5]);

                SelectedSpeed = config.ModbusSpeed;
                BitValues = config.BitValues;
                ParityOdd = config.ParityOdd;
                ParityExistence = config.ParityExistence;
                StopBitsCount = config.StopBitCount;
            }
            else
            {
                MessageBox.Show("Ошибка при чтении конфигурации.");
            }
        }
        /// <summary>
        /// Инициализация списка скоростей обменов
        /// </summary>
        private void InitializeModbusSpeedList()
        {
            ModbusSpeedList.Add(230400);
            ModbusSpeedList.Add(115200);
            ModbusSpeedList.Add(57600);
            ModbusSpeedList.Add(28800);
            ModbusSpeedList.Add(14400);
            ModbusSpeedList.Add(7200);
            ModbusSpeedList.Add(3600);
            ModbusSpeedList.Add(1800);
            ModbusSpeedList.Add(76800);
            ModbusSpeedList.Add(38400);
            ModbusSpeedList.Add(19200);
            ModbusSpeedList.Add(9600);
            ModbusSpeedList.Add(4800);
            ModbusSpeedList.Add(2400);
            ModbusSpeedList.Add(1200);
            ModbusSpeedList.Add(600);
        }
        /// <summary>
        /// Изменение типа и адреса из UI
        /// </summary>
        private void SpreadTypeAddress()
        {
            //refactor
            string _typeStr = TypeAddress.First().ToString();
            string _addressStr = TypeAddress.Last().ToString();

            Type = Converters.Convert.ConvertFromHexToDec((byte)Int32.Parse(_typeStr, System.Globalization.NumberStyles.HexNumber));
            Address = Converters.Convert.ConvertFromHexToDec((byte)Int32.Parse(_addressStr, System.Globalization.NumberStyles.HexNumber));
        }
        /// <summary>
        /// Изменение заголовка
        /// </summary>
        private void ChangeTitle()
        {
            if (IsUpper)
            {
                this.Title = Title.Split(' ').First() + " с верхом";
            }
            else
            {
                this.Title = Title.Split(' ').First() + " с низом";
            }
        }
        #endregion

        #region [UICommands]
        /// <summary>
        /// Запись конфигурации модуля связи в устройство
        /// </summary>
        private async void OnWriteRequestCommand()
        {
            Config = GenerateConfigForDevice();
            if (IsUpper)
            {
                try
                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, UPPER_ADDRESS, Config);
                    ShowMessage("Запись прошла успешно!", "Информация", MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ShowMessage("При записи в устройство произошла ошибка.", "Внимание", MessageBoxImage.Warning);

                }
            }
            else
            {
                try
                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, LOWER_ADDRESS, Config);
                    ShowMessage("Запись прошла успешно!", "Информация", MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    ShowMessage("При записи в устройство произошла ошибка.", "Внимание", MessageBoxImage.Warning);
                }
            }

        }
        /// <summary>
        /// Чтение конфигурации модуля связи из устройства
        /// </summary>
        private async void OnReadRequestCommand()
        {
            ushort[] result = null;
            if (IsUpper)
            {
                try
                {
                    result = await RTUConnectionGlobal.GetDataByAddress(1, UPPER_ADDRESS, PACKAGE_LENGTH);
                    ShowMessage("Чтение прошло успешно!", "Информация", MessageBoxImage.Information);
                    AnalyzeConfigFromDevice(result);
                }
                catch (Exception ex)
                {
                    ShowMessage("При чтении из устройства произошла ошибка.", "Внимание", MessageBoxImage.Warning);
                }
            }
            else
            {
                try
                {
                    result = await RTUConnectionGlobal.GetDataByAddress(1, LOWER_ADDRESS, PACKAGE_LENGTH);
                    ShowMessage("Чтение прошло успешно!", "Информация", MessageBoxImage.Information);
                    AnalyzeConfigFromDevice(result);
                }
                catch (Exception ex)
                {
                    ShowMessage("При чтении из устройства произошла ошибка.", "Внимание", MessageBoxImage.Warning);
                }
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
        /// Метод закрытия окна (не реализован, хз нужен он или нет, т.к. делается он четез костыль(или не костыль, но мне не нравится), пока обойдемся крестиком)
        /// </summary>
        private void OnCloseWindowCommand()
        {

        }
        #endregion
    }
}
