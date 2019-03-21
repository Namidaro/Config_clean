using System;
using System.Windows.Controls;

namespace UniconGS.UI.Channels
{
    /// <summary>
    /// Interaction logic for Rele.xaml
    /// </summary>
    public partial class Rele : UserControl
    {
        public delegate void ValueChangedEventHandler(object sender,bool value);
        public event ValueChangedEventHandler ValueChanged;

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

        private void uiReleButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.IsEnabled && this.ValueChanged != null)
                this.ValueChanged(this, Convert.ToBoolean(this.uiReleButton.IsChecked));

        }
    }
}
