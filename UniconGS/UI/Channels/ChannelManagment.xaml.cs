using System;
using System.Collections.Generic;
using System.Windows.Controls;
using UniconGS.Source;

namespace UniconGS.UI.Channels
{
    /// <summary>
    /// Interaction logic for ChannelManagment.xaml
    /// </summary>
    public partial class ChannelManagment : UserControl
    {
        public void SetAutonomous()
        {
            this.uiManualControl.Value = null;
            this.uiCommand.Value = null;
            this.uiFixControl.Value = null;
            this.uiChecker.Value = null;
        }
        public void DisableAutonomus()
        {
            this.uiManualControl.Value = new bool?();
            this.uiCommand.Value = new bool?();
            this.uiFixControl.Value = new bool?();
            this.uiChecker.Value = new bool?();
        }
        #region Globals
        /* убрано нажатие на кнопки, потому что КН не дает записывать команды */
        public delegate void WriteValueEventHandler(object sender, List<bool> value);
        public event WriteValueEventHandler WriteValue;
       //private List<bool> _value;
        private List<bool> _value; 
        #endregion
        public List<bool> Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (value == null)
                {
                    this.DisableAll();
                }
                else
                {
                    this._value = value;
                    this.SelectAll(value);
                }
            }
        }
        
        public ChannelManagment()
        {
            InitializeComponent();
            _value = new List<bool>();
            /* убрано нажатие на кнопки, потому что КН не дает записывать команды */
            this.uiManualControl.ValueChanged += new Rele.ValueChangedEventHandler(this.OnValueChanged);
            this.uiCommand.ValueChanged += new Rele.ValueChangedEventHandler(this.OnValueChanged);
            this.uiFixControl.ValueChanged += new Rele.ValueChangedEventHandler(this.OnValueChanged);
            //this.uiManualControl.ValueChanged += this.OnValueChanged;
            //this.uiCommand.ValueChanged += this.OnValueChanged;
            //this.uiFixControl.ValueChanged += this.OnValueChanged;
       
    }

        #region Убрано нажатие на кнопки, потому что КН не дает записывать команды
        private void OnValueChanged(object sender, bool value)
        {
            if (this.IsEnabled && this.WriteValue != null)
            {
                List<bool> val = null;
                if (sender.Equals(this.uiCommand))
                {
                    val = new List<bool>
                    {
                        this._value[0],
                        this._value[1],
                        this._value[2],
                        value,
                        this._value[4],
                        this._value[5],
                        this._value[6]
                    };
                }
                else if (sender.Equals(this.uiFixControl))
                {
                    val = new List<bool>
                    {
                        this._value[0],
                        this._value[1],
                        this._value[2],
                        this._value[3],
                        value,
                        this._value[5],
                        this._value[6]
                    };
                }
                else if (sender.Equals(this.uiManualControl))
                {
                    val = new List<bool>
                    {
                        this._value[0],
                        this._value[1],
                        this._value[2],
                        this._value[3],
                        this._value[4],
                        value,
                        this._value[6]
                    };
                }
                this.WriteValue(this, val);
            }
        }
        #endregion

        #region Privates

        private void DisableAll()
        {
            this.uiChecker.Value = null;
            this.uiCommandChecker.Value = null;
            this.uiFixStateChecker.Value = null;
            this.uiManualStateChecker.Value = null;
            this.uiCommand.Value = null;
            this.uiFixControl.Value = null;
            this.uiManualControl.Value = null;
        }

        private void SelectAll(List<bool> value)
        {
            this.uiChecker.Value = value[6];
            this.uiCommandChecker.Value = value[0];
            this.uiFixStateChecker.Value = value[1];
            this.uiManualStateChecker.Value = value[2];
            this.uiCommand.Value = value[3];
            this.uiFixControl.Value = value[4];
            this.uiManualControl.Value = value[5];
        }
        #endregion

        //public Slot Querer
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //ushort[] IQuery.Value
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

    //    public void Update()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool WriteContext()
    //    {
    //        throw new NotImplementedException();
    //    }
    }
}
