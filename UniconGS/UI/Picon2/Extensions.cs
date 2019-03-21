using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Picon2
{
    /// <summary>
    /// 	Extension methods for the array data type
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// Конвертирует массив слов в массив байт
        /// </summary>
        /// <param name="words"> Массив слов.</param>
        /// <param name="bDirect">Порядок байт. true - обычный, false - ст.байт меняем местом с мл.байтом.</param>
        /// <returns>Массив байт.</returns>
        public static byte[] UshortArrayToByteArray(ushort[] words)
        {
            byte[] buffer = new byte[words.Length * 2];
            for (int i = 0, j = 0; i < words.Length; i++)
            {
                buffer[j++] = HIBYTE(words[i]);
                buffer[j++] = LOBYTE(words[i]);
            }
            return buffer;
        }

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

        public static ushort[] ByteArrayToUshortArray(byte[] byteArray)
        {
            if (byteArray.Length % 2 != 0)
            {
                byte[] buffer = byteArray;
                byteArray = new byte[byteArray.Length + 1];
                Array.ConstrainedCopy(buffer, 0, byteArray, 0, buffer.Length);
            }
            ushort[] ushorts = new ushort[byteArray.Length / 2];
            int ind = 0;
            for (int i = 0; i < ushorts.Length; i++)
            {
                ushorts[i] = TwoBytesToUshort(byteArray[ind], byteArray[ind + 1]);
                ind += sizeof(ushort);
            }
            return ushorts;
        }


        /// <summary>
        /// Конвертирует 2 байта в слово
        /// </summary>
        /// <param name="highByte">Ст.байт</param>
        /// <param name="lowByte">Мл.байт</param>
        /// <returns>Слово.</returns>
        public static ushort TwoBytesToUshort(byte highByte, byte lowByte)
        {
            UInt16 ret = (UInt16)highByte;
            return (ushort)((ushort)(ret << 8) + (ushort)lowByte);
        }



        ///<summary>
        ///	Check if the array is null or empty
        ///</summary>
        ///<param name = "source"></param>
        ///<returns></returns>
        public static bool IsNullOrEmpty(this Array source)
        {
            return source != null && source.Length <= 0;
        }
        ///<summary>
        ///	Check if the index is within the array
        ///</summary>
        ///<param name = "source"></param>
        ///<param name = "index"></param>
        ///<returns></returns>
        public static bool WithinIndex(this Array source, int index)
        {
            return source != null && index >= 0 && index < source.Length;
        }
        ///<summary>
        ///	Check if the index is within the array
        ///</summary>
        ///<param name = "source"></param>
        ///<param name = "index"></param>
        ///<param name="dimension"></param>
        ///<returns></returns>
        public static bool WithinIndex(this Array source, int index, int dimension = 0)
        {
            return source != null && index >= source.GetLowerBound(dimension) && index <= source.GetUpperBound(dimension);
        }
        /// <summary>
        /// Combine two arrays into one.
        /// </summary>
        /// <typeparam name="T">Type of Array</typeparam>
        /// <param name="combineWith">Base array in which arrayToCombine will add.</param>
        /// <param name="arrayToCombine">Array to combine with Base array.</param>
        /// <returns></returns>
        public static T[] CombineArray<T>(this T[] combineWith, T[] arrayToCombine)
        {
            if (combineWith != default(T[]) && arrayToCombine != default(T[]))
            {
                var initialSize = combineWith.Length;
                Array.Resize(ref combineWith, initialSize + arrayToCombine.Length);
                Array.Copy(arrayToCombine, arrayToCombine.GetLowerBound(0), combineWith, initialSize, arrayToCombine.Length);
            }
            return combineWith;
        }
        /// <summary>
        /// To clear the contents of the array.
        /// </summary>
        /// <param name="clear"> The array to clear</param>
        /// <returns>Cleared array</returns>
        public static Array ClearAll(this Array clear)
        {
            if (clear != null)
                Array.Clear(clear, 0, clear.Length);
            return clear;
        }
        /// <summary>
        /// To clear the contents of the array.
        /// </summary>
        /// <typeparam name="T">The type of array</typeparam>
        /// <param name="arrayToClear"> The array to clear</param>
        /// <returns>Cleared array</returns>
        public static T[] ClearAll<T>(this T[] arrayToClear)
        {
            if (arrayToClear != null)
                for (var i = arrayToClear.GetLowerBound(0); i <= arrayToClear.GetUpperBound(0); ++i)
                    arrayToClear[i] = default(T);
            return arrayToClear;
        }
        /// <summary>
        /// Попарно меняет местами элементы массива 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void SwapArrayItems<T>(ref T[] array)
        {
            T temp;
            try
            {
                for (int i = 0; i < array.Length; i += 2)
                {
                    temp = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = temp;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }


        }


        #region BlockCopy
        /// <summary>
        /// Returns a block of items from an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] BlockCopy<T>(this T[] array, int index, int length)
        {
            return BlockCopy(array, index, length, false);
        }
        /// <summary>
        /// Returns a block of items from an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="padToLength"></param>
        /// <returns></returns>
        public static T[] BlockCopy<T>(this T[] array, int index, int length, bool padToLength)
        {
            if (array == null) throw new NullReferenceException();

            var n = length;
            T[] b = null;

            if (array.Length < index + length)
            {
                n = array.Length - index;
                if (padToLength)
                {
                    b = new T[length];
                }
            }

            if (b == null) b = new T[n];
            Array.Copy(array, index, b, 0, n);
            return b;
        }

        /// <summary>
        /// Allows enumeration over an Array in blocks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> BlockCopy<T>(this T[] array, int count)
        {
            return BlockCopy(array, count, false);
        }
        /// <summary>
        /// Allows enumeration over an Array in blocks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="count"></param>
        /// <param name="padToLength"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> BlockCopy<T>(this T[] array, int count, bool padToLength)
        {
            for (var i = 0; i < array.Length; i += count)
                yield return array.BlockCopy(i, count, padToLength);
        }
        #endregion
    }
}
