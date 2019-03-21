using System;

namespace UniconGS.Source
{
    /// <summary>
    /// Класс, содержащий в себе массив данных формата UInt16,
    /// начальный и конечный адреса памяти в устройстве, количество слов в памяти
    /// </summary>
    public class Slot : ICloneable
    {
        #region Functions

        /// <summary>
        /// Создает слот, выделяет память.
        /// </summary>
        /// <param name="start"> Начальный адрес памяти </param>
        /// <param name="count"> Количество слов в слоте </param>
        public Slot(ushort start, ushort count, string name)
        {
            this._start = start;
            this._end = (ushort)(start + count);
            this._size = count;
            this._name = name;
            this._value = new ushort[_size];
        }

        /// <summary>
        /// Дублирует слот
        /// </summary>
        /// <param name="newSlot">Слот - копия </param>
        public Slot Copy(Slot slot)
        {
            return this.MemberwiseClone() as Slot;
        }

        /// <summary>
        /// Создает массив ячеек.
        /// </summary>
        /// <param name="qslots"> Непроинициированные слоты</param>
        /// <param name="start">Начальный адрес памяти</param>
        /// <param name="size">Величина памяти, выделяемая на 1 слот</param>
        /// <param name="count">Кол-во слотов</param>
        public static void InitSlots(Slot[] qslots, ushort start, ushort blockSize, int count, string name)
        {
            for (int i = 0; i < count; i++)
            {
                qslots[i] = new Slot(start, start += blockSize, name);
            }

        }
        #endregion

        #region Fields
        private ushort _start;
        private ushort _end;
        private ushort _size;
        private string _name;
        private ushort[] _value;
        private bool loaded = false;

        #endregion

        #region Properties

        /// <summary>
        /// Имя слота
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Начальный адрес слота
        /// </summary>
        public ushort Start
        {
            get { return _start; }
            set { _start = value; }
        }
        /// <summary>
        /// Конечный адрес слота
        /// </summary>
        public ushort End
        {
            get { return _end; }
        }
        /// <summary>
        /// Размер памяти слота
        /// </summary>
        public ushort Size
        {
            get { return _size; }
            set { _size = value; }
        }
        /// <summary>
        /// Значение хранимое в слоте
        /// </summary>
        public ushort[] Value
        {
            get
            {
                return _value;
            }
            set
            {
                this._value = value;
            }
        }
        /// <summary>
        /// Определяет, загружалась или нет ,инф. из устройства в слот
        /// </summary>
        public bool Loaded
        {
            get
            {
                return loaded;
            }
            set
            {
                loaded = value;
            }
        }

        #endregion Properties

        public object Clone()
        {
            var slot = new Slot(this.Start, (ushort)(this.End - this.Start), this.Name);
            slot.Value = this.Value;
            return slot;
        }

        public static explicit operator ushort(Slot v)
        {
            throw new NotImplementedException();
        }
    }
}
