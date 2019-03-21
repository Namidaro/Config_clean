using System;
using System.Runtime.Serialization;

namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Represents transformation form bynary array to <see cref="int" />
    ///     Форматирует байты аналогов в значение Энергии для счетчика
    /// </summary>
    [DataContract(Name = "bytesToIntEnergyFormatter")]
    public class BytesToIntEnergyFormatter : BinaryFormatterBase
    {
        #region [Private fields]

        [DataMember(Name = "index")]
        private int _index;
        [DataMember(Name = "bitNumber")]
        private int _bitNumber;

        #endregion [Private fields]

        #region [Ctor's]

        /// <summary>
        ///     Creates an instance of <see cref="BytesToIntVoltageFormatter" />. It is used ONLY for deserialization purpose.
        /// </summary>
        private BytesToIntEnergyFormatter()
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="BytesToIntVoltageFormatter" />
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bitNumber"></param>
        public BytesToIntEnergyFormatter(int index, int bitNumber = -1)
        {
            this._index = index;
            this._bitNumber = bitNumber;
        }

        #endregion [Ctor's]

        #region [Override members]

        /// <summary>
        ///     Provides backward formatting action over value
        /// </summary>
        /// <param name="value">Value to format backward</param>
        /// <param name="currentValue">Value to apply formatting to</param>
        /// <returns>Resulted formatted value</returns>
        protected override byte[] OnFormatBack(object currentValue, byte[] value)
        {
            var result = new byte[value.Length];
            value.CopyTo(result, 0);
            var intValue = Convert.ToInt32(currentValue); 
            var persistanceValue = (int)(intValue * 4294901760 / 999999); //эти числа мне сказали конструкторы(4294967296), изменил
            result[this._index - 1] = (byte)(persistanceValue / 256);
            result[this._index] = (byte)(persistanceValue % 256);
            return result;
        }

        /// <summary>
        ///     Provedes formatting action over value
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Resulted formatted value</returns>
        protected override object OnFormat(byte[] value)
        {

            if (this._bitNumber >= 0)
            {
                throw new ArgumentException("Биты здесь не нужны");
            }

            double a = value[this._index - 1] * 256;
            a = a + value[this._index];
            double b = value[_index + 1] * 256; //смещение на байт  
            b = (b + value[_index + 2]) * 65535; //на 2 байта
            a = b + a;  //складываем 2 слова в одно значение
            var result = ((a * 999999) / 4294901760);//поменял большое число, раньше стояло "4294967296",
                                                     //я хз откудо оно взялось, но оно давало погрешность в 0.16%, 
                                                     //что роляло на больших числах - сейчас погрешности практически нет, так что аллилуя
            return result; 
        }

        #endregion [Override members]

        #region [Help members]

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            /*none*/
        }

        #endregion [Help members]
    }
}