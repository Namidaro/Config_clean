using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2.ModuleRequests.Converters
{
    public class Convert
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
        /// Возвращает младший байт слова
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Мл.байт</returns>
        public static byte LOHALFBYTE(int v)
        {
            return (byte)(v & 0xf);
        }
        /// <summary>
        /// Возвращает старший байт слова.
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Ст.байт</returns>
        public static byte HIHALFBYTE(int v)
        {
            return (byte)(v >> 4);
        }


        //todo: просмотреть конвертеры, че-то мне в них не нравится

        /// <summary>
        /// Перевод из Hex в Dec (может понадобиться)
        /// </summary>
        /// <param name="Hex"></param>
        /// <returns></returns>
        public static byte ConvertFromHexToDec(byte Hex)
        {
            string hexValueStr = Hex.ToString("X");
            byte decValue = byte.Parse(hexValueStr, System.Globalization.NumberStyles.HexNumber);
            return decValue;
        }
        /// <summary>
        /// Перевод из Dec в Hex (может понадобиться)
        /// </summary>
        /// <param name="Dec"></param>
        /// <returns></returns>
        public static byte ConvertFromDecToHex(byte Dec)
        {
            string hexValueStr = Dec.ToString("X");
            byte hexValue = byte.Parse(hexValueStr, System.Globalization.NumberStyles.AllowHexSpecifier);
            return hexValue;
        }
        /// <summary>
        /// Из Dec в Hex, возвращает строку
        /// </summary>
        /// <param name="Dec"></param>
        /// <returns></returns>
        public static string ConvertFromDecToHexStr(byte Dec)
        {
            string hexValueStr = Dec.ToString("X");
            return hexValueStr;
        }
    }
}
