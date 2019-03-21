using System;
using System.Windows;
using System.Windows.Controls;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for Lightning.xaml
    /// </summary>
    public partial class Lightning : UserControl
    {

        private bool? _value = null;

        public Lightning()
        {
            InitializeComponent();
        }

        #region IQueryMember
        public bool? ValueRuno
        {
            get
            {
                return this._value;
            }
            set
            {

                if (value == null)
                {
                    this.SetDefault();
                    this._value = null;
                }
                else
                {

                    var val = Convert.ToBoolean(value);
                    if (val)
                        SetOn();
                    else
                        SetDefault();
                    this._value = Convert.ToBoolean(value);

                }
            }
        }
        public bool? Value
        {
            get
            {
                return this._value;
            }
            set
            {

                if (value == null)
                {
                    this.SetDefault();
                    this._value = null;
                }
                else
                {
                    var val = Convert.ToBoolean(value);
                    if (val)
                        SetOn();
                    else
                        SetOff();
                    this._value = Convert.ToBoolean(value);
                    
                }
            }
        }
        #endregion

        public void SetOff()
        {
            this.uiTurnOn.Visibility = Visibility.Hidden;
            this.uiTurnOff.Visibility = Visibility.Visible;
            this.uiDefault.Visibility = Visibility.Hidden;
            this.uiСhRuno.Visibility = Visibility.Hidden;
        }

        public void SetOn()
        {
            this.uiDefault.Visibility = Visibility.Hidden;
            this.uiTurnOff.Visibility = Visibility.Hidden;
            this.uiTurnOn.Visibility = Visibility.Visible;
            this.uiСhRuno.Visibility = Visibility.Hidden;
        }

        private void SetDefault()
        {
            this.uiTurnOn.Visibility = Visibility.Hidden;
            this.uiTurnOff.Visibility = Visibility.Hidden;
            this.uiDefault.Visibility = Visibility.Visible;
            this.uiСhRuno.Visibility = Visibility.Hidden;
        }

        public void SetCh()
        {
            this.uiTurnOn.Visibility = Visibility.Hidden;
            this.uiTurnOff.Visibility = Visibility.Hidden;
            this.uiDefault.Visibility = Visibility.Hidden;
            this.uiСhRuno.Visibility = Visibility.Visible;
        }
    }
}
