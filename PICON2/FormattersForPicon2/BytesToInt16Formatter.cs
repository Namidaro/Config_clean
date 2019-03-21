using System;
using System.Runtime.Serialization;

namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Represents transformation form bynary array to <see cref="short" />
    ///     Перевод массива байт в short
    /// </summary>
    [DataContract(Name = "bytesToInt16Formatter")]
    public class BytesToInt16Formatter : BinaryFormatterBase
    {
        #region [Private fields]

        [DataMember(Name = "index")] 
        private int _index;
        [DataMember(Name = "bitNumber")] 
        private int _bitNumber;

        #endregion [Private fields]

        #region [Ctor's]

        /// <summary>
        ///     Creates an instance of <see cref="BytesToInt16Formatter" />. It is used ONLY for deserialization purpose.
        /// </summary>
        private BytesToInt16Formatter()
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="BytesToInt16Formatter" />
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bitNumber"></param>
        public BytesToInt16Formatter(int index, int bitNumber = -1)
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
            result[this._index - 1] = (byte)(int16Value/256); //short - это 2 байта, вот в 2 и пишем
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
            return value[this._index-1]*256+value[this._index]; //short - это 2 байта
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
