using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification;

namespace UniconGS.UI.Picon2.ModuleRequests
{
    public class Picon2CommunicationModule910SeriesViewModel : BindableBase
    {
        #region [Private Fields]
        private string _title;
        private string _typeAddress;
        private byte _type;
        private byte _address;
        private ushort _responseAwait;
        private bool _isUpper;

        private bool _isRadio;
        private ObservableCollection<bool> _bitConfig;

        private byte _ioTimeout;
        private ushort _transmitEnableDelayUshort;
        private ushort _transmitDisableDelayUshort;
        private byte _transmitEnableDelayByte;
        private byte _transmitDisableDelayByte;
        private byte _transmitEnablePause;
        private byte _transmitDisablePause;

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
                RaisePropertyChanged();
                ChangeTitle();
            }
        }
        /// <summary>
        /// Показатель работы модуля по радио протоколу
        /// </summary>
        public bool IsRadio
        {
            get { return _isRadio; }
            set
            {
                _isRadio = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Конфигурация модуля (массив битов)
        /// </summary>
        public ObservableCollection<bool> BitConfig
        {
            get { return _bitConfig; }
            set
            {
                _bitConfig = value;
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
        public ushort TransmitEnableDelayUshort
        {
            get { return _transmitEnableDelayUshort; }
            set
            {
                _transmitEnableDelayUshort = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Задержка выключения передачи (0..65535)
        /// </summary>
        public ushort TransmitDisableDelayUshort
        {
            get { return _transmitDisableDelayUshort; }
            set
            {
                _transmitDisableDelayUshort = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Задержка включения передачи (0..255)
        /// </summary>
        public byte TransmitEnableDelayByte
        {
            get { return _transmitEnableDelayByte; }
            set
            {
                _transmitEnableDelayByte = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Задержка выключения передачи (0..255)
        /// </summary>
        public byte TransmitDisableDelayByte
        {
            get { return _transmitDisableDelayByte; }
            set
            {
                _transmitDisableDelayByte = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Пауза на включение (для радио)
        /// </summary>
        public byte TransmitEnablePause
        {
            get { return _transmitEnablePause; }
            set
            {
                _transmitEnablePause = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Пауза на выключение (для радио)
        /// </summary>
        public byte TransmitDisablePause
        {
            get { return _transmitDisablePause; }
            set
            {
                _transmitDisablePause = value;
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
        public Picon2CommunicationModule910SeriesViewModel(byte _typeIN, byte _addressIN, string _titleIN)
        {
            Type = _typeIN;
            Address = _addressIN;
            TypeAddress = Converters.Convert.ConvertFromDecToHexStr((byte)((this.Type << 4) + this.Address)).ToString();
            this.Title = _titleIN + " с верхом";
            IsUpper = true;
            IsRadio = false;
            ResponseAwait = 0;
            IOTimeout = 0;
            BitConfig = new ObservableCollection<bool>();
            InitializeBitConfig();
            TransmitEnableDelayUshort = 0;
            TransmitDisableDelayUshort = 0;
            TransmitEnableDelayByte = 0;
            TransmitDisableDelayByte = 0;
            TransmitDisablePause = 0;
            TransmitEnablePause = 0;
            Config = new ushort[PACKAGE_LENGTH];
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Инициализация коллекции
        /// </summary>
        private void InitializeBitConfig()
        {
            BitConfig.AddRange(new bool[8] { false, false, false, false, false, false, false, false });
        }
        /// <summary>
        /// Формирует массив слов, который будет записан в устройство
        /// </summary>
        /// <returns></returns>
        private ushort[] GenerateConfigForDevice()
        {
            List<byte> byteArr = new List<byte>();
            List<ushort> confToDevice = new List<ushort>();
            // можно переделать конструктор на прием массива бит, но мне лень, так что все в твоих руках, друг
            Config910Series conf910 = new Config910Series(BitConfig[0],
                                                          BitConfig[1],
                                                          BitConfig[2],
                                                          BitConfig[3],
                                                          BitConfig[4],
                                                          BitConfig[5],
                                                          BitConfig[6],
                                                          BitConfig[7]);
            SpreadTypeAddress();
            byteArr.Add(IOTimeout);
            byteArr.Add((byte)((Type << 4) + Address));
            confToDevice.Add(ArrayExtension.ByteArrayToUshortArray(byteArr.ToArray()).First());
            confToDevice.Add(ResponseAwait);
            confToDevice.Add(conf910.Config);
            if (IsRadio)
            {
                byteArr.Clear();
                byteArr.Add(TransmitEnablePause);
                byteArr.Add(TransmitEnableDelayByte);
                confToDevice.Add(ArrayExtension.ByteArrayToUshortArray(byteArr.ToArray()).First());
                byteArr.Clear();
                byteArr.Add(TransmitDisablePause);
                byteArr.Add(TransmitDisableDelayByte);
                confToDevice.Add(ArrayExtension.ByteArrayToUshortArray(byteArr.ToArray()).First());
            }
            else
            {
                confToDevice.Add(TransmitEnableDelayUshort);
                confToDevice.Add(TransmitDisableDelayUshort);
            }
            return confToDevice.ToArray();
        }
        /// <summary>
        /// Читает массив слов из устройства и обновляет UI
        /// </summary>
        /// <param name="_configFromDevice"></param>
        private void AnalyzeConfigFromDevice(ushort[] _configFromDevice)
        {
            if (_configFromDevice == null)
                return;
            if (_configFromDevice.Count() == 5)
            {
                byte[] byteArray = ArrayExtension.UshortArrayToByteArray(_configFromDevice);
                IOTimeout = Converters.Convert.ConvertFromDecToHex(byteArray[0]);
                TypeAddress = Converters.Convert.ConvertFromDecToHexStr(byteArray[1]);
                ResponseAwait = _configFromDevice[1];
                Config910Series config = new Config910Series(byteArray[5]);
                BitConfig[0] = config.Speed;
                BitConfig[1] = config.Protocol;
                BitConfig[2] = config.Filter;
                BitConfig[3] = config.Amplifier;
                BitConfig[4] = config.BitValues;
                BitConfig[5] = config.ParityOdd;
                BitConfig[6] = config.ParityExistence;
                BitConfig[7] = config.StopBitCount;
                if (IsRadio)
                {
                    TransmitEnableDelayByte = byteArray[7];
                    TransmitEnablePause = byteArray[6];
                    TransmitDisableDelayByte = byteArray[9];
                    TransmitDisablePause = byteArray[8];
                }
                else
                {
                    TransmitEnableDelayUshort = _configFromDevice[3];
                    TransmitDisableDelayUshort = _configFromDevice[4];
                }
            }
            else
            {
                MessageBox.Show("Ошибка при чтении конфигурации.");
            }
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
                    ShowMessage("При записи конфигурации произошла ошибка!", "Внимание", MessageBoxImage.Warning);
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
                    ShowMessage("При записи конфигурации произошла ошибка!", "Внимание", MessageBoxImage.Warning);
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
                    ShowMessage("При чтении конфигурации произошла ошибка!", "Внимание", MessageBoxImage.Warning);
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
                    ShowMessage("При чтении конфигурации произошла ошибка!", "Внимание", MessageBoxImage.Warning);
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
