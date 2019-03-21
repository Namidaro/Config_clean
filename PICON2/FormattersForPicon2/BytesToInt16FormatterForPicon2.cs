using System;
using System.Runtime.Serialization;

namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Represents transformation form bynary array to <see cref="short" />
    ///     Перевод массива байт в short
    /// </summary>
    [DataContract(Name = "bytesToInt16FormatterForPicon2")]
    public class BytesToInt16FormatterForPicon2 : BinaryFormatterBase
    {
        #region [Private fields]

        [DataMember(Name = "index")]
        private int _index;
        [DataMember(Name = "bitNumber")]
        private int _bitNumber;

        #endregion [Private fields]

        #region [Ctor's]

        /// <summary>
        ///     Creates an instance of <see cref="BytesToInt16FormatterForPicon2" />. It is used ONLY for deserialization purpose.
        /// </summary>
        private BytesToInt16FormatterForPicon2()
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="BytesToInt16FormatterForPicon2" />
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bitNumber"></param>
        public BytesToInt16FormatterForPicon2(int index, int bitNumber = -1)
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
            var int16Value = Convert.ToInt16(currentValue);
            int16Value -= 2000;//пикон воспринимает только два числа, не думаю, что через 200 лет этой скадой будут пользоваться
            result[this._index] = (byte)(int16Value % 256);
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
            return value[this._index - 1] * 256 + value[this._index]; //short - это 2 байта
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
