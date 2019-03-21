using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ULA.Common.Formatters
{
    /// <summary>
    /// Форматирование строк
    /// </summary>
    [DataContract(Name = "bytesToStringFormatter")]
    public class BytesToStringFormatter: BinaryFormatterBase
    {
        #region Overrides of BinaryFormatterBase
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override byte[] OnFormatBack(object currentValue, byte[] value)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override object OnFormat(byte[] value)
        {
            string tmp = string.Empty;

            for (int i = 0; i < value.Length; i++)
            {
                int index = i;
                if ((i + 1) % 2 == 0)
                {
                    index--;
                }
                else
                {
                    index++;
                }
                if ((value[index] != byte.MaxValue) && (value[index] != byte.MinValue) && (value[index] != 37))
                    tmp += Convert.ToChar(value[index]);
            }
            return tmp;
        }

        #endregion
    }
}
