using System;
using System.Runtime.Serialization;

namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Represents transformation form bynary array to <see cref="byte" />  
    ///     Представляет форматирование массива байт в один байт
    /// </summary>
    [DataContract(Name = "bytesToByteFormatter")]
    public class BytesToByteFormatter : BinaryFormatterBase
    {
        #region [Private fields]

        [DataMember(Name = "index")] 
        private int _index;
        [DataMember(Name = "bitNumber")] 
        private int _bitNumber;

        #endregion [Private fields]

        #region [Ctor's]

        /// <summary>
        ///     Creates an instance of <see cref="BytesToByteFormatter" />. It is used ONLY for deserialization purpose.
        /// </summary>
        private BytesToByteFormatter()
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="BytesToByteFormatter" />
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bitNumber"></param>
        public BytesToByteFormatter(int index,int bitNumber = -1)
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
            result[this._index] = Convert.ToByte(currentValue);
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
                return ((value[this._index] & (1 << this._bitNumber)) != 0);
            }
            return value[this._index].ToString("D2");
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