using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ULA.Common.Formatters
{
    /// <summary>
    /// получение из двух байт слово для квитирования неисправностей
    /// </summary>  
    [DataContract(Name = "BytesToFailAckFormatter")]
    public class BytesToFailAckFormatter:BinaryFormatterBase
    {
        #region [Private fields]

        [DataMember(Name = "index")]
        private int _index;

        #endregion [Private fields]




        #region [Ctor's]

        /// <summary>
        ///     Creates an instance of <see cref="BytesToFailAckFormatter" />. It is used ONLY for deserialization purpose.
        /// </summary>
        private BytesToFailAckFormatter()
        {
        }

        /// <summary>
        ///     Creates an instance of <see cref="BytesToFailAckFormatter" />
        /// </summary>
        /// <param name="index">номер байта</param>
        public BytesToFailAckFormatter(int index)
        {
            this._index = index;
        }

        #endregion [Ctor's]






        /// <summary>
        ///     Provides backward formatting action over value
        /// </summary>
        /// <param name="value">Value to format backward</param>
        /// <param name="currentValue">Value to apply formatting to</param>
        /// <returns>Resulted formatted value</returns>
        protected override byte[] OnFormatBack(object currentValue, byte[] value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Provedes formatting action over value
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <returns>Resulted formatted value</returns>
        protected override object OnFormat(byte[] value)
        {
            return new byte[] {value[_index],value[_index-1]};
        }
    }
}
