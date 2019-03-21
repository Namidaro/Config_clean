using System;
using System.Windows.Controls;
using UniconGS.Source;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for Rele.xaml
    /// </summary>
    public partial class Rele : UserControl
    {
        //public delegate void ValueChangedEventHandler(object sender,bool value);
        //public event ValueChangedEventHandler ValueChanged;

        public Rele()
        {
            InitializeComponent();
        }

        public bool? Value
        {
            get
            {
                return Convert.ToBoolean(this.uiReleButton.IsChecked);
            }
            set
            {
                if (value != null)
                {
                    this.uiReleButton.IsEnabled = true;
                    this.uiReleButton.IsChecked = Convert.ToBoolean(value);
                }
                else
                {
                    this.uiReleButton.IsEnabled = false;
                }
            }
        }
    }
}
