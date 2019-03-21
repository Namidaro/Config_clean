using System;
using System.Runtime.Serialization;

namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Represents transformation form bynary array to <see cref="int" />
    ///     Форматирует байты аналогов в значение Энергии для счетчика
    /// </summary>
    [DataContract(Name = "BytesToIntEnergyFormatterForPicon2")]
    public class BytesToIntEnergyFormatterForPicon2 : BinaryFormatterBase
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
        private BytesToIntEnergyFormatterForPicon2()
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="BytesToIntVoltageFormatter" />
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bitNumber"></param>
        public BytesToIntEnergyFormatterForPicon2(int index, int bitNumber = -1)
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
            return new byte[0];
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
            byte[] bytes = new[] {value[_index-2], value[_index-3], value[_index], value[_index-1] };
            float fff = BitConverter.ToSingle(bytes, 0);
            return (double) fff;
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