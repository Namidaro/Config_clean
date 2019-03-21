using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ULA.Common.Formatters
{
    /// <summary>
    /// Форматирование строк
    /// </summary>
    [DataContract(Name = "bytesToStringMeterDateTimeFormatter")]
    public class StringFormatter : BinaryFormatterBase
    {
        [DataMember(Name = "index")]
        private int _index;
        [DataMember(Name = "bitNumber")]
        private int _bitNumber;

        /// <summary>
        /// Без параметров
        /// </summary>
        public StringFormatter()
        {

        }
        /// <summary>
        /// Передаем индекс, если надо номер бита, для определения значения из массива
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bitNumber"></param>
        public StringFormatter(int index, int bitNumber = -1)
        {
            this._index = index;
            this._bitNumber = bitNumber;
        }

        /// <summary>
        /// Не используется!!
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override byte[] OnFormatBack(object currentValue, byte[] value)
        {
            return null;
        }

        /// <summary>
        /// Форматирование строки даты и время счетчика
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override object OnFormat(byte[] value)
        {
            ushort[] arr = new ushort[8];
            byte[] values = new byte[16];
            Array.ConstrainedCopy(value, _index, values, 0, 16);
            var j = 0;
            for (int i = 0; i < values.Length / 2; i++)
            {
                arr[i] = TOWORD(values[j], values[j + 1]);
                j += 2;
            }        
            string tmp = string.Empty;
            int a = 0;
            foreach (var item in arr)
            {
                foreach (var @byte in BitConverter.GetBytes(item))
                {
                    if (@byte == 0)
                    {
                        break;
                    }
                    else
                    {
                        tmp += Convert.ToChar(@byte);
                    }
                }
                a++;
            }
            if ((_index == 0) && (tmp.Length >= 4)) //Индекс даты
            {
                tmp = tmp.Remove(0, 3);
            }
            return tmp;
        }
        /// <summary>
        /// Изменение последовательности байт с преобразованием в слова
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public static ushort TOWORD(byte high, byte low)
        {
            UInt16 ret = (UInt16)high;
            return (ushort)((ushort)(ret << 8) + (ushort)low);
        }
    }
}
