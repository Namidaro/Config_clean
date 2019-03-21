using System.Runtime.Serialization;

namespace ULA.Common.Formatters
{
    /// <summary>
    ///     Represents basic binary value formatting functionality
    ///     Описывает базовый бинарный форматер
    /// </summary>
    [DataContract(Name = "binaryFormatter")]
    public abstract class BinaryFormatterBase : IValueFormatter
    {
        #region [IValueFormatter]
       

        /// <summary>
        ///     Provedes formatting action over value
        ///     Метод форматирования значения(предварительная обработка)
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Resulted formatted value</returns>
        public object Format(object value)
        {
            if (value == null) return null;
            var typedValue = value as byte[];
            return typedValue == null ? null : OnFormat(typedValue);
        }

        /// <summary>
        ///     Provides backward formatting action over value
        ///     Метод обратного форматирования(предварительная обработка)
        /// </summary>
        /// <param name="value">Value to format backward</param>
        /// <param name="currentValue">Value to apply formatting to</param>
        /// <returns>Resulted formatted value</returns>
        public object FormatBack(object value, object currentValue)
        {
            if (value == null) return null;
            var typedValue = currentValue as byte[];
            return typedValue == null ? null : this.OnFormatBack(value, typedValue);
        }


        #endregion [IValueFormatter]

        #region [Abstract members]

        /// <summary>
        ///     Provides backward formatting action over value
        ///     Сам функционал обратного форматирования
        /// </summary>
        /// <param name="value">Value to format backward</param>
        /// <param name="currentValue">Value to apply formatting to</param>
        /// <returns>Resulted formatted value</returns>
        protected abstract byte[] OnFormatBack(object currentValue, byte[] value);

        /// <summary>
        ///     Provedes formatting action over value
        ///     Сам функционал форматирования
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Resulted formatted value</returns>
        protected abstract object OnFormat(byte[] value);



        #endregion [Abstract members]

    }
}