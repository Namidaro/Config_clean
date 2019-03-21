using System;

namespace UniconGS.Source
{
    /// <summary>
    /// Класс для создания modbus-запросов
    /// </summary>
    public static class Modbus
    {
        #region Реализованные 16,6,3,4,5 функции
        /// <summary>
        /// 16 функция : Запись n слов
        /// </summary>
        /// <param name="knNumber">Номер контроллера направлений</param>
        /// <param name="deviceNumber">Номер устройства</param>
        /// <param name="adress">Адрес начального слова [Вид : 1100]</param>
        /// <param name="values">Массив значений слов</param>
        /// <returns>Запрос : массив байт</returns>
        public static byte[] CreateWriteWordsArray(int knNumber, int deviceNumber, ushort adress, ushort[] values)
        {
            #region Проверка входных значений
            string deviceNumberStr = deviceNumber.ToString("X");
            string knNumberStr = knNumber.ToString("X");
            int adr = adress;//System.Int32.Parse(adress.ToString(), System.Globalization.NumberStyles.HexNumber);

            ushort[] value = values;
            string countBytesStr = (value.Length * 2).ToString("X");
            #endregion

            byte[] modbus = new byte[9 + value.Length * 2];
            modbus[0] = System.Byte.Parse(deviceNumberStr, System.Globalization.NumberStyles.HexNumber);
            modbus[1] = 0x10;
            modbus[2] = Common.HIBYTE(adr);
            modbus[3] = Common.LOBYTE(adr);
            modbus[4] = Common.HIBYTE(value.Length);
            modbus[5] = Common.LOBYTE(value.Length);
            modbus[6] = System.Byte.Parse(countBytesStr, System.Globalization.NumberStyles.HexNumber);
            int num = 6;
            for (int i = 0; i < value.Length; i++)
            {
                modbus[num + 1] = Common.HIBYTE(value[i]);
                modbus[num + 2] = Common.LOBYTE(value[i]);
                num += 2;
            }
            ushort crc = CRC16.CalcCrcFast(modbus, modbus.Length - 2);
            modbus[num + 1] = Common.HIBYTE(crc);
            modbus[num + 2] = Common.LOBYTE(crc);
            return modbus;
            //byte[] toPort = new byte[modbus.Length + 4];
            //toPort[0] = System.Byte.Parse(knNumberStr, System.Globalization.NumberStyles.HexNumber);
            //toPort[1] = 0x11;
            //for (int i = 0; i < modbus.Length; i++)
            //{
            //    toPort[i + 2] = modbus[i];
            //}
            //ushort crc2 = CRC16.CalcCrcFast(toPort, toPort.Length - 2);
            //toPort[toPort.Length - 2] = Common.HIBYTE(crc2);
            //toPort[toPort.Length - 1] = Common.LOBYTE(crc2);
            //return toPort;
        }

        /// <summary>
        /// 6 функция : Запись слова
        /// </summary>
        /// <param name="knNumber">Номер контроллера направлений</param>
        /// <param name="deviceNumber">Номер устройства</param>
        /// <param name="adress">Адрес слова [Вид : 1100]</param>
        /// <param name="value">Значение слова</param>
        /// <returns>Запрос : массив байт</returns>
        public static byte[] CreateWriteWordArray(int knNumber, int deviceNumber, ushort adress, ushort value)
        {
            #region Проверка входных значений
            string deviceNumberStr = deviceNumber.ToString("X");
            string knNumberStr = knNumber.ToString("X");
            int adr = adress;//System.Int32.Parse(adress.ToString(), System.Globalization.NumberStyles.HexNumber);
            #endregion

            byte[] modbus = new byte[8];
            modbus[0] = System.Byte.Parse(deviceNumberStr, System.Globalization.NumberStyles.HexNumber);
            modbus[1] = 0x06;
            modbus[2] = Common.HIBYTE(adr);
            modbus[3] = Common.LOBYTE(adr);
            modbus[4] = Common.HIBYTE(value);
            modbus[5] = Common.LOBYTE(value);
            ushort crc = CRC16.CalcCrcFast(modbus, modbus.Length - 2);
            modbus[6] = Common.HIBYTE(crc);
            modbus[7] = Common.LOBYTE(crc);

            return modbus;

            //byte[] toPort = new byte[8+4];
            //toPort[0] = System.Byte.Parse(knNumberStr, System.Globalization.NumberStyles.HexNumber);
            //toPort[1] = 0x11;
            //for (int i = 0; i < modbus.Length; i++)
            //{
            //    toPort[i + 2] = modbus[i];
            //}
            //ushort crc2 = CRC16.CalcCrcFast(toPort, toPort.Length - 2);
            //toPort[toPort.Length - 2] = Common.HIBYTE(crc2);
            //toPort[toPort.Length - 1] = Common.LOBYTE(crc2);

            //return toPort;
        }

        /// <summary>
        /// 3 или 4 функция : Чтение n слов
        /// </summary>
        /// <param name="knNumber">Номер контроллера направлений</param>
        /// <param name="deviceNumber">Номер устройства</param>
        /// <param name="adress">Адрес слова [Вид : 1100]</param>
        /// <param name="count">Количество слов для чтения</param>
        /// <returns>Запрос : массив байт</returns>
        public static byte[] CreateReadWordsArray(int knNumber, int deviceNumber, ushort adress, int count)
        {
            #region Проверка входных значений
            string deviceNumberStr = deviceNumber.ToString("X");
            string knNumberStr = knNumber.ToString("X");

            int adr = adress; //System.Int32.Parse(adress.ToString(), System.Globalization.NumberStyles.HexNumber);//
            #endregion

            byte[] modbus = new byte[8];
            byte[] toPort = new byte[modbus.Length+4];

            modbus[0] = System.Byte.Parse(deviceNumberStr, System.Globalization.NumberStyles.HexNumber);
            modbus[1] = 0x04;
            modbus[2] = Common.HIBYTE(adr);
            modbus[3] = Common.LOBYTE(adr);
            modbus[4] = Common.HIBYTE(count);
            modbus[5] = Common.LOBYTE(count);
            ushort crc1 = CRC16.CalcCrcFast(modbus, modbus.Length - 2);
            modbus[6] = Common.HIBYTE(crc1);
            modbus[7] = Common.LOBYTE(crc1);
            return modbus;
            //toPort[0] = System.Byte.Parse(knNumberStr, System.Globalization.NumberStyles.HexNumber);
            //toPort[1] = 0x11;
            //for (int i = 0; i < modbus.Length; i++)
            //{
            //    toPort[i + 2] = modbus[i];
            //}
            //ushort crc2 = CRC16.CalcCrcFast(toPort, toPort.Length - 2);
            //toPort[toPort.Length - 2] = Common.HIBYTE(crc2);
            //toPort[toPort.Length - 1] = Common.LOBYTE(crc2);

            //return toPort;
        }

        /// <summary>
        /// 5 функция : Запись бита
        /// </summary>
        /// <param name="knNumber">Номер контроллера направлений</param>
        /// <param name="deviceNumber">Номер устройства</param>
        /// <param name="adress">Адрев бита</param>
        /// <param name="value">Значение бита[true - 0, false - 1]</param>
        /// <returns>Запрос : массив байт</returns>
        public static byte[] CreateWriteBitArray(int knNumber, int deviceNumber, ushort adress, bool value)
        {
            #region Проверка входных значений
            string deviceNumberStr = deviceNumber.ToString("X");
            string knNumberStr = knNumber.ToString("X");
            int adr = adress;//System.Int32.Parse(adress.ToString(), System.Globalization.NumberStyles.HexNumber)
            #endregion

            byte[] modbus = new byte[8];
            modbus[0] = System.Byte.Parse(deviceNumberStr, System.Globalization.NumberStyles.HexNumber);
            modbus[1] = 0x05;
            modbus[2] = Common.HIBYTE(adr);
            modbus[3] = Common.LOBYTE(adr);
            if (value)
            {
                modbus[4] = 0x00;
            }
            else
            {
                modbus[4] = 0xFF;
            }

            modbus[5] = 0x00;
            ushort crc = CRC16.CalcCrcFast(modbus, modbus.Length - 2);
            modbus[6] = Common.HIBYTE(crc);
            modbus[7] = Common.LOBYTE(crc);

            //byte[] toPort = new byte[8+4];
            //toPort[0] = System.Byte.Parse(knNumberStr, System.Globalization.NumberStyles.HexNumber);
            //toPort[1] = 0x11;
            //for (int i = 0; i < modbus.Length; i++)
            //{
            //    toPort[i + 2] = modbus[i];
            //}
            //ushort crc2 = CRC16.CalcCrcFast(toPort, toPort.Length - 2);
            //toPort[toPort.Length - 2] = Common.HIBYTE(crc2);
            //toPort[toPort.Length - 1] = Common.LOBYTE(crc2);

            return modbus;
        }
        #endregion

        #region Не проверено 1 функция
        /// <summary>
        /// 1 функция : Чтение n бит (Не проверена)
        /// </summary>
        public static byte[] CreateReadBits(int deviceNumber, ushort adress, int count)
        {
            #region Проверка входных значений
            string _deviceNumberStr = deviceNumber.ToString("X");

            int _adress = adress;//System.Int32.Parse(adress.ToString(), System.Globalization.NumberStyles.HexNumber);
            #endregion
            byte[] _toPort = new byte[8];
            _toPort[0] = System.Byte.Parse(_deviceNumberStr, System.Globalization.NumberStyles.HexNumber);
            _toPort[1] = 0x01;
            _toPort[2] = Common.HIBYTE(_adress);
            _toPort[3] = Common.LOBYTE(_adress);
            _toPort[4] = Common.HIBYTE(count);
            _toPort[5] = Common.LOBYTE(count);
            ushort crc = CRC16.CalcCrcFast(_toPort, _toPort.Length - 2);
            _toPort[6] = Common.HIBYTE(crc);
            _toPort[7] = Common.LOBYTE(crc);
            return _toPort;
        }
        #endregion

        #region Не реализована 15 функция
        /// <summary>
        /// 15 функция : Запись n бит (Не реализована)
        /// </summary>
        public static byte[] WriteBits()
        {
            byte[] _toPort = new byte[1];

            return _toPort;
        }
        #endregion
    }
}
