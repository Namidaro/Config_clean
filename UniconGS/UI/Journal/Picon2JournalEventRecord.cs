using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniconGS.UI.Picon2;

namespace UniconGS.UI.Journal
{
    /// <summary>
    /// Класс записи журнала системы для Пикона2. Будем читать символьную по определенным причинам.
    /// </summary>
    public class Picon2JournalEventRecord
    {
        #region [Privates]
        //Ситуация такая: в двоичной области все коды ошибок, в которых есть символы е[A..F] из 16ричной системы исчисления,
        //                  из самого устройства 3й функцией читается как 0, что не соответствует реальности
        //                  В двоично-десятичной нужны левые преобразования, которые я делать не хочу, поэтому будет проще сделать некоторые действия и читать символьную область.
        //Дата время символьная (структура одной записи)        16 байт
        //
        //код(= 03030h - журнал пуст, = 04646h нет сообщения)   2 байта
        // год                                                  2 байта
        // месяц                                                2 байта
        // число                                                2 байта
        // часы                                                 2 байта
        // минуты                                               2 байта
        // секунды                                              2 байта
        // миллисекунды                                         2 байта

        private ushort _errorCode;
        private ushort _year;
        private ushort _month;
        private ushort _day;
        private ushort _hour;
        private ushort _minute;
        private ushort _second;
        private ushort _millisecond;

        private string _date;
        private string _time;
        private string _error;

        private Dictionary<int, string> _errorCodeDictionary;

        private string _journalRecord;
        #endregion

        #region [Properties]
        /// <summary>
        /// Строка записи журнала системы
        /// </summary>
        public string JournalRecord
        {
            get { return _journalRecord; }
            set { _journalRecord = value; }
        }

        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }
        /// <summary>
        /// Словарь кодов ошибок
        /// </summary>
        public Dictionary<int, string> ErrorCodeDictionary
        {
            get { return _errorCodeDictionary; }
            set { _errorCodeDictionary = value; }
        }
        #endregion

        #region [C'tors]
        /// <summary>
        /// Конструктор для обработки символьного массива слов
        /// </summary>
        /// <param name="_inputArray">Массив слов символьной записи</param>
        public Picon2JournalEventRecord(ushort[] _inputArray)
        {
            byte[] _inputByteArray = ArrayExtension.UshortArrayToByteArray(_inputArray);
            ArrayExtension.SwapArrayItems(ref _inputArray);

            // очень важное колдунство, вычисляем код ошибки
            byte HIErrorByte = _inputByteArray[0];
            byte LOErrorByte = _inputByteArray[1];
            _errorCode = (ushort)((HIErrorByte - 0x30) * 16 + (LOErrorByte - 0x30));

            _year = Convert.ToUInt16(Encoding.UTF8.GetString(_inputByteArray.Skip(2).Take(2).ToArray()));
            _month = Convert.ToUInt16(Encoding.UTF8.GetString(_inputByteArray.Skip(4).Take(2).ToArray()));
            _day = Convert.ToUInt16(Encoding.UTF8.GetString(_inputByteArray.Skip(6).Take(2).ToArray()));
            _hour = Convert.ToUInt16(Encoding.UTF8.GetString(_inputByteArray.Skip(8).Take(2).ToArray()));
            _minute = Convert.ToUInt16(Encoding.UTF8.GetString(_inputByteArray.Skip(10).Take(2).ToArray()));
            _second = Convert.ToUInt16(Encoding.UTF8.GetString(_inputByteArray.Skip(12).Take(2).ToArray()));
            _millisecond = Convert.ToUInt16(Encoding.UTF8.GetString(_inputByteArray.Skip(14).Take(2).ToArray()));

            this.ErrorCodeDictionary = new Dictionary<int, string>();
            this.InitializeErrorCodeDictionary();
            this.GetErrorRecord();
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Инициализация словаря кодов ошибок
        /// </summary>
        private void InitializeErrorCodeDictionary()
        {
            ErrorCodeDictionary.Add(0, "Журнал пуст");
            ErrorCodeDictionary.Add(1, "Устройство выключено");
            ErrorCodeDictionary.Add(2, "Устройство включено");
            ErrorCodeDictionary.Add(3, "Ошибка CRC ПЗУ");

            ErrorCodeDictionary.Add(5, "Ошибка FLASH");
            ErrorCodeDictionary.Add(6, "Питание выключено");
            ErrorCodeDictionary.Add(7, "Питание включено");
            ErrorCodeDictionary.Add(8, "Ошибка часов");
            ErrorCodeDictionary.Add(9, "Норма часов");
            ErrorCodeDictionary.Add(10, "Сброс контроллера");

            ErrorCodeDictionary.Add(16, "Модуль №0 отказ");
            ErrorCodeDictionary.Add(17, "Модуль №1 отказ");
            ErrorCodeDictionary.Add(18, "Модуль №2 отказ");
            ErrorCodeDictionary.Add(19, "Модуль №3 отказ");
            ErrorCodeDictionary.Add(20, "Модуль №4 отказ");
            ErrorCodeDictionary.Add(21, "Модуль №5 отказ");
            ErrorCodeDictionary.Add(22, "Модуль №6 отказ");
            ErrorCodeDictionary.Add(23, "Модуль №7 отказ");
            ErrorCodeDictionary.Add(24, "Модуль №8 отказ");
            ErrorCodeDictionary.Add(25, "Модуль №9 отказ");
            ErrorCodeDictionary.Add(26, "Модуль №10 отказ");
            ErrorCodeDictionary.Add(27, "Модуль №11 отказ");
            ErrorCodeDictionary.Add(28, "Модуль №12 отказ");
            ErrorCodeDictionary.Add(29, "Модуль №13 отказ");
            ErrorCodeDictionary.Add(30, "Модуль №14 отказ");
            ErrorCodeDictionary.Add(31, "Модуль №15 отказ");

            ErrorCodeDictionary.Add(32, "Модуль №0 норма");
            ErrorCodeDictionary.Add(33, "Модуль №1 норма");
            ErrorCodeDictionary.Add(34, "Модуль №2 норма");
            ErrorCodeDictionary.Add(35, "Модуль №3 норма");
            ErrorCodeDictionary.Add(36, "Модуль №4 норма");
            ErrorCodeDictionary.Add(37, "Модуль №5 норма");
            ErrorCodeDictionary.Add(38, "Модуль №6 норма");
            ErrorCodeDictionary.Add(39, "Модуль №7 норма");
            ErrorCodeDictionary.Add(40, "Модуль №8 норма");
            ErrorCodeDictionary.Add(41, "Модуль №9 норма");
            ErrorCodeDictionary.Add(42, "Модуль №10 норма");
            ErrorCodeDictionary.Add(43, "Модуль №11 норма");
            ErrorCodeDictionary.Add(44, "Модуль №12 норма");
            ErrorCodeDictionary.Add(45, "Модуль №13 норма");
            ErrorCodeDictionary.Add(46, "Модуль №14 норма");
            ErrorCodeDictionary.Add(47, "Модуль №15 норма");

            ErrorCodeDictionary.Add(48, "Ошибка запросов модулей");
            ErrorCodeDictionary.Add(49, "Норма запросов модулей");
            ErrorCodeDictionary.Add(50, "Ошибка запросов к низу");
            ErrorCodeDictionary.Add(51, "Норма запросов к низу");

            ErrorCodeDictionary.Add(255, "Нет сообщения");
        }
        /// <summary>
        /// Возвращает скомпонованную запись журнала системы
        /// </summary>
        /// <returns></returns>
        private void GetErrorRecord()
        {
            StringBuilder sb = new StringBuilder();
            string _errorText;
            ErrorCodeDictionary.TryGetValue(_errorCode, out _errorText);
            DateTime dateTime = new DateTime(2000 + _year, _month, _day, _hour, _minute, _second);

            Date = dateTime.ToShortDateString();
            Time = dateTime.ToLongTimeString();
            Error = _errorText;
            sb.Append(
                Date + 
                " " +
                Time +
                " - " + Error
                );

            JournalRecord = sb.ToString();
        }
        #endregion

        #region [Help members]
        #endregion
    }
}
