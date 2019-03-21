using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NModBus4.Extensions
{
    public static class Extensions
    {

        /// <summary>
        /// Возвращает младший байт слова
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Мл.байт</returns>
        public static byte LOBYTE(int v)
        {
            return (byte)(v & 0xff);
        }
        /// <summary>
        /// Возвращает старший байт слова.
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Ст.байт</returns>
        public static byte HIBYTE(int v)
        {
            return (byte)(v >> 8);
        }



        /// <summary>
        /// Конвертирует слово в массив байт
        /// </summary>
        /// <param name="word">слово</param>
        /// <returns>массив байт</returns>
        public static byte[] UshortToBytes(this ushort word, bool bDirect = true)
        {
            byte[] buffer = new byte[2];
            if (bDirect)
            {
                buffer[0] = HIBYTE(word);
                buffer[1] = LOBYTE(word);
            }
            else
            {
                buffer[0] = LOBYTE(word);
                buffer[1] = HIBYTE(word);
            }
            return buffer;
        }
    }
}
