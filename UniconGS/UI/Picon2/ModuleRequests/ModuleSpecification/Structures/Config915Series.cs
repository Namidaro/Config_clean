using System;
using System.Collections.Generic;
using System.Collections;
using UniconGS.Source;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification
{
    public class Config915Series
    {
        #region [Private Fields]
        private Dictionary<long, byte> _modbusSpeedDictionary;
        private long _modbusSpeed;
        private bool _bitValues;
        private bool _parityOdd;
        private bool _parityExistence;
        private bool _stopBitCount;
        private ushort _config;
        #endregion

        #region [Properties]
        /// <summary>
        /// Конфигурация модуля
        /// </summary>
        public ushort Config
        {
            get { return _config; }
            set
            {
                _config = value;
            }
        }
        /// <summary>
        /// Словарь скоростей
        /// </summary>
        private Dictionary<long, byte> ModbusSpeedDictionary
        {
            get { return _modbusSpeedDictionary; }
            set
            {
                _modbusSpeedDictionary = value;
            }
        }
        /// <summary>
        /// Скорость обменов
        /// </summary>
        public long ModbusSpeed
        {
            get { return _modbusSpeed; }
            set
            {
                _modbusSpeed = value;
            }
        }
        /// <summary>
        /// Биты данных (8 би / 7 бит)
        /// </summary>
        public bool BitValues
        {
            get { return _bitValues; }
            set
            {
                _bitValues = value;
            }
        }
        /// <summary>
        /// Паритен (нечет / чет)
        /// </summary>
        public bool ParityOdd
        {
            get { return _parityOdd; }
            set
            {
                _parityOdd = value;
            }
        }
        /// <summary>
        /// Паритет (нет / есть)
        /// </summary>
        public bool ParityExistence
        {
            get { return _parityExistence; }
            set
            {
                _parityExistence = value;
            }
        }
        /// <summary>
        /// Стоп биты (1 бит / 2 бита)
        /// </summary>
        public bool StopBitCount
        {
            get { return _stopBitCount; }
            set
            {
                _stopBitCount = value;
            }
        }
        #endregion

        #region [Ctor]
        public Config915Series(long _modbusSpeed, bool _bitValues, bool _parityOdd, bool _parityExistence, bool _stopBitCount)
        {
            this.ModbusSpeed = _modbusSpeed;
            this.BitValues = _bitValues;
            this.ParityOdd = _parityOdd;
            this.ParityExistence = _parityExistence;
            this.StopBitCount = _stopBitCount;
            this.Config = 0;
            this.ModbusSpeedDictionary = new Dictionary<long, byte>();
            InitializeSpeedDictionary();
            Config = GenerateConfig();
        }
        public Config915Series(byte _deviceByte)
        {
            this.ModbusSpeed = 0;
            this.BitValues = false;
            this.ParityOdd = false;
            this.ParityExistence = false;
            this.StopBitCount = false;
            this._config = 0;
            this.ModbusSpeedDictionary = new Dictionary<long, byte>();
            InitializeSpeedDictionary();
            Config = _deviceByte;
            SpreadConfig();
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Заполнение словаря скоростей
        /// </summary>
        private void InitializeSpeedDictionary()
        {
            this.ModbusSpeedDictionary.Add(230400, 0);
            this.ModbusSpeedDictionary.Add(115200, 1);
            this.ModbusSpeedDictionary.Add(57600, 2);
            this.ModbusSpeedDictionary.Add(28800, 3);
            this.ModbusSpeedDictionary.Add(14400, 4);
            this.ModbusSpeedDictionary.Add(7200, 5);
            this.ModbusSpeedDictionary.Add(3600, 6);
            this.ModbusSpeedDictionary.Add(1800, 7);
            this.ModbusSpeedDictionary.Add(76800, 8);
            this.ModbusSpeedDictionary.Add(38400, 9);
            this.ModbusSpeedDictionary.Add(19200, 10);
            this.ModbusSpeedDictionary.Add(9600, 11);
            this.ModbusSpeedDictionary.Add(4800, 12);
            this.ModbusSpeedDictionary.Add(2400, 13);
            this.ModbusSpeedDictionary.Add(1200, 14);
            this.ModbusSpeedDictionary.Add(600, 15);
        }
        /// <summary>
        /// Формируем байт конфигурации
        /// </summary>
        /// <returns>byte конфигурации</returns>
        private ushort GenerateConfig()
        {
            //TODO: refactor
            byte[] sp = new byte[1];
            if (ModbusSpeedDictionary.ContainsKey(ModbusSpeed))
            {
                ModbusSpeedDictionary.TryGetValue(ModbusSpeed, out sp[0]);
            }
            BitArray _bA = new BitArray(sp);

            _bA.Set(7, StopBitCount);
            _bA.Set(6, ParityExistence);
            _bA.Set(5, ParityOdd);
            _bA.Set(4, BitValues);

            return Converter.GetWordFromBits(_bA);
        }
        /// <summary>
        /// Разбор конфигурации из устройства
        /// </summary>
        private void SpreadConfig()
        {
            byte[] workArray = new byte[2] { ArrayExtension.HIBYTE(Config), ArrayExtension.LOBYTE(Config) };
            BitArray workBits = new BitArray(workArray);
            BitValues = workBits[12];
            ParityOdd = workBits[13];
            ParityExistence = workBits[14];
            StopBitCount = workBits[15];
            byte speedbyte = SpeedByteFromBits(workBits[8], workBits[9], workBits[10], workBits[11]);
            ModbusSpeed = ModbusSpeedDictionary.FirstOrDefault(x => x.Value == speedbyte).Key;
        }
        /// <summary>
        /// Формируем скорость из набора бит
        /// </summary>
        /// <param name="rankOne"></param>
        /// <param name="rankTwo"></param>
        /// <param name="rankThree"></param>
        /// <param name="rankFour"></param>
        /// <returns></returns>
        private byte SpeedByteFromBits(bool rankOne, bool rankTwo, bool rankThree, bool rankFour)
        {
            byte result = (byte)(BoolToByte(rankOne) * 1 +
                                 BoolToByte(rankTwo) * 2 +
                                 BoolToByte(rankThree) * 4 +
                                 BoolToByte(rankFour) * 8);
            return result;
        }
        /// <summary>
        /// 0/1 byte from bool
        /// </summary>
        /// <param name="IN"></param>
        /// <returns></returns>
        private byte BoolToByte(bool IN)
        {
            if (IN)
                return 1;
            else
                return 0;
        }
        #endregion
    }
}
