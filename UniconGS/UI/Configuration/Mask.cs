using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using UniconGS.Source;

namespace UniconGS.UI.Configuration
{
    [Serializable]
    public class Mask
    {
        private ObservableCollection<bool> _value;

        [XmlElement]
        public ObservableCollection<bool> Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public Mask()
        {

        }

        public void InitializeByDefault()
        {
            this.Value = new ObservableCollection<bool>();

            for (int i = 0; i < 44; i++)
            {
                this.Value.Add(false);
            }


        }

        public Mask(ObservableCollection<bool> value)
        {
            this.Value = value;
        }

        public Mask(ushort[] value)
        {
            List<bool> tmp = new List<bool>();
            foreach (var item in value)
            {
                BitArray bt = new BitArray(BitConverter.GetBytes(item));
                tmp.AddRange(bt.OfType<bool>().ToList().GetRange(0, 11));
            }
            this.Value = new ObservableCollection<bool>();
            foreach (var item in tmp)
            {
                this.Value.Add(item);
            }
        }

        public ushort[] GetWordsValue()
        {

            ushort[] tm = new ushort[4];
            for (int i = 0; i < 4; i++)
            {
                List<bool> l = this.Value.ToList().GetRange(i * 11, 11);
                l.AddRange(new List<bool>() { false, false, false, false, false });
                tm[i] = Converter.GetWordFromBits(new BitArray(l.ToArray()));
            }
            return tm;
        }


    }
}
