using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace UniconGS.UI.Configuration
{
    [XmlRoot("ChannelManagment", Namespace = "",
     IsNullable = false), Serializable]
    public class ChannelManagment : INotifyPropertyChanged
    {
        private Mask _securityMask;
        private Mask _errorMask;
        private ushort _automationTime = 100;

        [XmlElement]
        public ushort AutomationTime
        {
            get
            {
                return this._automationTime;
            }
            set
            {
                if (this._automationTime != value)
                {
                    this._automationTime = value;
                    this.onPropertyChanged("AutomationTime");

                }
            }
        }
        [XmlElement]
        public ObservableCollection<Channel> Channels { get; set; }
        [XmlElement]
        public ObservableCollection<Mask> ChannelMasks { get; set; }
        [XmlElement]
        public Mask SecurityMask
        {
            get
            {
                return this._securityMask;
            }
            set
            {
                this._securityMask = value;
            }
        }
        [XmlElement]
        public Mask ManagmentMask { get; set; }
        [XmlElement]
        public Mask PowerMask
        {
            get
            {
                return this._errorMask;
            }
            set
            {
                this._errorMask = value;
                onPropertyChanged("PowerMask");
            }
        }
        [XmlElement]
        public Mask ErrorMask { get; set; }

        public ChannelManagment()
        {

        }

        public void InitializeChannelByDefault()
        {
            this.AutomationTime = 100;
            this.ChannelMasks = new ObservableCollection<Mask>();
            this.Channels = new ObservableCollection<Channel>();
            for (int i = 0; i < 8; i++)
            {
                var tmpMask = new Mask();
                tmpMask.InitializeByDefault();
                this.ChannelMasks.Add(tmpMask);
                this.Channels.Add(new Channel());
            }


            this.ManagmentMask = new Mask();
            this.PowerMask = new Mask();
            this.SecurityMask = new Mask();
            this.ErrorMask = new Mask();

            this.ManagmentMask.InitializeByDefault();
            this.PowerMask.InitializeByDefault();
            this.SecurityMask.InitializeByDefault();
            this.ErrorMask.InitializeByDefault();
        }

        private void SetErrorMask()
        {
            for (int i = 0; i < this.ErrorMask.Value.Count; i++)
            {
                bool tmp = false;
                foreach (var channelMask in this.ChannelMasks)
                {
                    tmp |= channelMask.Value[i];
                }
                tmp |= this.ManagmentMask.Value[i] | this.PowerMask.Value[i] | this.SecurityMask.Value[i];
                this.ErrorMask.Value[i] = tmp;
            }

        }

        public void SetData(object value)
        {
            var tmp = (value as Array).OfType<ushort>().ToList();
            int counter = 0;
            for (int i = 0; i < this.Channels.Count; i++)
            {
                this.Channels[i] = new Channel(tmp.GetRange(i * 6, 2).ToArray());
                this.ChannelMasks[i] = new Mask(tmp.GetRange(i * 6 + 2, 4).ToArray());
                counter = i * 6 + 2 + 4;
            }
            this.SecurityMask = new Mask(tmp.GetRange(counter, 4).ToArray());
            this.ManagmentMask = new Mask(tmp.GetRange(counter + 4, 4).ToArray());
            this.PowerMask = new Mask(tmp.GetRange(counter + 8, 4).ToArray());
            this.AutomationTime = tmp[tmp.Count - 1];
            this.SetErrorMask();
        }


        public ushort[] GetValue()
        {
            List<ushort> tmp = new List<ushort>();
            for (int i = 0; i < this.Channels.Count; i++)
            {
                tmp.AddRange(this.Channels[i].GetValue());
                tmp.AddRange(this.ChannelMasks[i].GetWordsValue());
            }
            tmp.AddRange(this.SecurityMask.GetWordsValue());
            tmp.AddRange(this.ManagmentMask.GetWordsValue());
            tmp.AddRange(this.PowerMask.GetWordsValue());
            tmp.Add(AutomationTime);
            return tmp.ToArray();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void onPropertyChanged(string fieldName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(fieldName));
            }
        }

        #endregion
    }
}
