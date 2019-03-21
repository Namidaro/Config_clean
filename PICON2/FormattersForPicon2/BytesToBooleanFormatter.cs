using System;
using System.Runtime.Serialization;

namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Represents transformation form bynary array to <see cref="bool" />
    ///     ПРедставляет форматер байтового массива в <see cref="bool" />
    /// </summary>
    [DataContract(Name = "bytesToBooleanFormatter")]
    public class BytesToBooleanFormatter: BinaryFormatterBase
    {
        #region [Private fields]

        [DataMember(Name = "index")] private int _index;
        [DataMember(Name = "bitNumber")] private int _bitNumber;

        #endregion [Private fields]

        #region [Ctor's]

        /// <summary>
        ///     Creates an instance of <see cref="BytesToBooleanFormatter" />. It is used ONLY for deserialization purpose.
        /// </summary>
        private BytesToBooleanFormatter()
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="BytesToBooleanFormatter" />
        /// </summary>
        /// <param name="index">номер байта</param>
        /// <param name="bitNumber">номер бита</param>
        public BytesToBooleanFormatter(int index, int bitNumber = -1)
        {
            this._index = index;
            this._bitNumber = bitNumber;
        }

        #endregion [Ctor's]

        #region [Override members]

        /// <summary>
        ///     Provides backward formatting action over value
        ///     Значение currentValue будет поставлено по нужному месту в массив байт value
        /// </summary>
        /// <param name="value">Value to format backward</param>
        /// <param name="currentValue">Value to apply formatting to</param>
        /// <returns>Resulted formatted value</returns>
        protected override byte[] OnFormatBack(object currentValue, byte[] value)
        {
            var currentBoolValue = currentValue as bool?;
            if (!currentBoolValue.HasValue)
            {
                throw new InvalidCastException("CurrentCalue in OnFormatBack method in BytesToBooleanFormatter can be a bool");
            }

            var result = new byte[value.Length];
            value.CopyTo(result, 0);

            if (currentBoolValue.Value)
            {
                //left-shift 1, then bitwise OR
                result[_index] = (byte)(value[_index] | (1 << _bitNumber));
            }
            else
            {
                //left-shift 1, then take complement, then bitwise AND
                result[_index] = (byte)(value[_index] & ~(1 << _bitNumber));
            }
            return result;
        }

        /// <summary>
        ///     Provedes formatting action over value
        ///     Форматирование из массива байт в bool значение
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Resulted formatted value</returns>
        protected override object OnFormat(byte[] value)
        {
            if (this._bitNumber >= 0)
            {
                return ((value[this._index] & (1 << this._bitNumber)) != 0);
            }
            return Convert.ToBoolean(value[this._index]);
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
