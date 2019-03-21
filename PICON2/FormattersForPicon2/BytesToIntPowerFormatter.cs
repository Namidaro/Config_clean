using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Represents transformation form bynary array to <see cref="int" />
    ///     Форматирует байты аналогов в значение Мощности для счетчика
    /// </summary>
    [DataContract(Name = "bytesToIntPowerFormatter")]
    public class BytesToIntPowerFormatter : BinaryFormatterBase
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
        private BytesToIntPowerFormatter()
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="BytesToIntVoltageFormatter" />
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bitNumber"></param>
        public BytesToIntPowerFormatter(int index, int bitNumber = -1)
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
            var persistanceValue = (int)(intValue * 65535 / 400); //эти числа мне сказали конструкторы
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
            return ((double)value[this._index - 1] * 256 + value[this._index]) * 40 / 65535; //эти числа мне сказали конструкторы
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