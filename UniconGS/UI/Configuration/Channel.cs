using System;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;

namespace UniconGS.UI.Configuration
{
    [Serializable]
    public class Channel : INotifyPropertyChanged
    {
        private ushort _graphicValue = 0;
        private byte _discretValue = 0;
        private byte _releValue = 0;

        [XmlElement]
        public bool IsTurnOn { get; set; }
        [XmlElement]
        public ushort GraphicValue
        {
            get
            {
                return this._graphicValue;
            }
            set
            {
                if (this._graphicValue != value)
                {
                    this._graphicValue = value;
                    this.OnPropertyChanged("GraphicValue");
                }
            }
        }
        [XmlElement]
        public byte DiscretValue
        {
            get
            {
                return this._discretValue;
            }
            set
            {
                if (this._discretValue != value)
                {
                    this._discretValue = value;
                    this.OnPropertyChanged("DiscretValue");
                }
            }
        }
        [XmlElement]
        public byte ReleValue
        {
            get
            {
                return this._releValue;
            }
            set
            {
                if (this._releValue != value)
                {
                    this._releValue = value;
                    this.OnPropertyChanged("ReleValue");
                }
            }
        }

        public Channel()
        {

        }

        public Channel(ushort[] value)
        {
            try
            {
                if (value.Length == 2)
                {
                    //не правильная конвертация слова к байту. 
                    //Если в слове записано число размером больше байта 
                    //возникнет ошибка.
                    //Из слова сначала надо выделить байт.
                    //Либо сделать GraphicNumber типа слова и работать с ним
                    //как со словом.(рекомендуется)
                    this.GraphicValue = value[0];
                    var tmp = BitConverter.GetBytes(value[1]);
                    this.ReleValue = tmp[0];
                    this.DiscretValue = tmp[1];
                }
            }
            catch (Exception f)
            {
                MessageBox.Show("Память заполнена FFFF.", "Внимание!");
            }
        }

        public Channel(bool isTurnOn, ushort graphicNumber, byte releNumber, byte discretNumber)
        {
            this.IsTurnOn = isTurnOn;
            this.GraphicValue = graphicNumber;
            this.ReleValue = releNumber;
            this.DiscretValue = discretNumber;
        }

        public ushort[] GetValue()
        {
            return new ushort[2]{this.GraphicValue,
            BitConverter.ToUInt16(new byte[2]{this.ReleValue, this.DiscretValue},0)};
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string fieldName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(fieldName));
            }
        }
        #endregion
    }
}
