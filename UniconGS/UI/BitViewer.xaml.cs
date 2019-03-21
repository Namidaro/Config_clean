using System;
using System.Windows;
using System.Windows.Controls;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for BitViewer.xaml
    /// </summary>
    public partial class BitViewer : UserControl
    {

        private bool? _value = null;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BitViewer), new UIPropertyMetadata("Text"));
        public BitViewer()
        {
            InitializeComponent();
        }
        private void SetTrue()
        {
            this.uiFalse.Visibility = Visibility.Hidden;
            this.uiNull.Visibility = Visibility.Hidden;
            this.uiTrue.Visibility = Visibility.Visible;
        }

        private void SetFalse()
        {
            this.uiTrue.Visibility = Visibility.Hidden;
            this.uiNull.Visibility = Visibility.Hidden;
            this.uiFalse.Visibility = Visibility.Visible;
        }

        private void SetNull()
        {
            this.uiFalse.Visibility = Visibility.Hidden;
            this.uiNull.Visibility = Visibility.Visible;
            this.uiTrue.Visibility = Visibility.Hidden;
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
                    this.SetNull();
                    this._value = null;
                }
                else
                {
                    if (Convert.ToBoolean(value))
                        this.SetTrue();
                    else
                        this.SetFalse();
                    this._value = Convert.ToBoolean(value);
                }
            }
        }
    }
}
