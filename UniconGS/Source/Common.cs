using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniconGS.Source
{
    public class Common
    {
        /// <summary>
        /// Конвертирует 2 байта в слово
        /// </summary>
        /// <param name="high">Ст.байт</param>
        /// <param name="low">Мл.байт</param>
        /// <returns>Слово.</returns>
        public static ushort TOWORD(byte high, byte low)
        {
            UInt16 ret = (UInt16)high;
            return (ushort)((ushort)(ret << 8) + (ushort)low);
        }

        /// <summary>?
        /// Конвертирует массив байт в массив слов. 
        /// </summary>
        /// <param name="bytes">Массив байт</param>
        /// <param name="bDirect"> Порядок байт.false - реверс,true - обычный</param>
        /// <returns></returns>
        public static ushort[] TOWORDS(byte[] bytes, bool bDirect)
        {

            if (0 != (bytes.Length % 2))
            {
                bytes = new byte[bytes.Length + 1];
            }
            ushort[] ret = new ushort[bytes.Length / 2];
            int j = 0;
            for (int i = 0; i < bytes.Length; i += 2)
            {
                if (bDirect)
                {
                    ret[j++] = TOWORD(bytes[i], bytes[i + 1]);
                }
                else
                {
                    ret[j++] = TOWORD(bytes[i + 1], bytes[i]);
                }

            }
            return ret;
        }


        /// <summary>
        /// Возвращает младший байт слова
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Мл.байт</returns>
        public static byte LOBYTE(long v)
        {
            return (byte)(v & 0xff);
        }
        /// <summary>
        /// Возвращает старший байт слова.
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Ст.байт</returns>
        public static byte HIBYTE(long v)
        {
            return (byte)(v >> 8);
        }

        /// <summary>
        /// Конвертирует 2 ushort в long
        /// </summary>
        /// <param name="Word1">1й ushort</param>
        /// <param name="Word2">2й ushort</param>
        /// <returns>Int</returns>
        public static int HIINT(long param)
        {
            string d = param.ToString("X2");
            if (d.Length < 16)
                do
                {
                    d = "0" + d;
                } while (d.Length < 16);

            string d1 = string.Empty;
            for (int i = 0; i < 8; i++)
            {
                d1 += d[i];
            }

            return System.Int32.Parse(d1, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Конвертирует 2 ushort в long
        /// </summary>
        /// <param name="Word1">1й ushort</param>
        /// <param name="Word2">2й ushort</param>
        /// <returns>Int</returns>
        public static int LOINT(long param)
        {
            string d = param.ToString("X2");
            if (d.Length < 16)
                do
                {
                    d = "0" + d;
                } while (d.Length < 16);

            string d1 = string.Empty;
            for (int i = 8; i < 16; i++)
            {
                d1 += d[i];
            }

            return System.Int32.Parse(d1, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Конвертирует 2 ushort в long
        /// </summary>
        /// <param name="Word1">1й ushort</param>
        /// <param name="Word2">2й ushort</param>
        /// <returns>Int</returns>
        public static int ToLONG(ushort param1, ushort param2)
        {
            string d1 = param1.ToString("X2");
            string d2 = param2.ToString("X2");
            if (d2.Length < 4)
                do
                {
                    d2 = "0" + d2;
                } while (d2.Length < 4);

            return System.Int32.Parse(d1 + d2, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
