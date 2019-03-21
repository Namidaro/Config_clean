using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniconGS.Source;

namespace UniconGS.UI.Picon2.ModuleRequests.ModuleSpecification
{
    public class Config910Series
    {
        #region [Private Fields]
        private bool _speed;
        private bool _protocol;
        private bool _filter;
        private bool _amplifier;

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
        /// Скорость ((75 / 150)/1200)
        /// </summary>
        public bool Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
            }
        }
        /// <summary>
        /// Протокол (v.23 / Bell.202)
        /// </summary>
        public bool Protocol
        {
            get { return _protocol; }
            set
            {
                _protocol = value;
            }
        }
        /// <summary>
        /// Приемный фильтр (есть / нет)
        /// </summary>
        public bool Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
            }
        }
        /// <summary>
        /// Усиление (нет / +3 dB)
        /// </summary>
        public bool Amplifier
        {
            get { return _amplifier; }
            set
            {
                _amplifier = value;
            }
        }
        /// <summary>
        /// Биты данных (8 бит / 7 бит)
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
        public Config910Series(bool _speed, bool _protocol, bool _filter, bool _amplifier, bool _bitValues, bool _parityOdd, bool _parityExistence, bool _stopBitCount)
        {
            this.Speed = _speed;
            this.Protocol = _protocol;
            this.Filter = _filter;
            this.Amplifier = _amplifier;
            this.BitValues = _bitValues;
            this.ParityOdd = _parityOdd;
            this.ParityExistence = _parityExistence;
            this.StopBitCount = _stopBitCount;
            this.Config = 0;
            Config = GenerateConfig();
        }
        public Config910Series(byte _deviceByte)
        {
            this.Speed = false;
            this.Protocol = false;
            this.Filter = false;
            this.Amplifier = false;
            this.BitValues = false;
            this.ParityOdd = false;
            this.ParityExistence = false;
            this.StopBitCount = false;
            this._config = 0;
            Config = _deviceByte;
            SpreadConfig();
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Формируем байт конфигурации
        /// </summary>
        /// <returns>byte конфигурации</returns>
        private ushort GenerateConfig()
        {
            //TODO: refactor
            byte[] sp = new byte[1];

            BitArray _bA = new BitArray(sp);

            _bA.Set(7, StopBitCount);
            _bA.Set(6, ParityExistence);
            _bA.Set(5, ParityOdd);
            _bA.Set(4, BitValues);
            _bA.Set(3, Amplifier);
            _bA.Set(2, Filter);
            _bA.Set(1, Protocol);
            _bA.Set(0, Speed);

            return Converter.GetWordFromBits(_bA);
        }
        /// <summary>
        /// Разбор конфигурации из устройства
        /// </summary>
        private void SpreadConfig()
        {
            //refactor  
            byte[] workArray = new byte[2] { ArrayExtension.HIBYTE(Config), ArrayExtension.LOBYTE(Config) };
            BitArray workBits = new BitArray(workArray);
            Speed = workBits[8];
            Protocol = workBits[9];
            Filter = workBits[10];
            Amplifier = workBits[11];
            BitValues = workBits[12];
            ParityOdd = workBits[13];
            ParityExistence = workBits[14];
            StopBitCount = workBits[15];
        }
        #endregion
    }
}
