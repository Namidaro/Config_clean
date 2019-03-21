using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UniconGS.UI.Journal
{
    /// <summary>
    /// Логика взаимодействия для JournalPassword.xaml
    /// </summary>
    public partial class JournalPassword : Window
    {
        public JournalPassword()
        {
            InitializeComponent();
        }

        private void uiOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.uiPSW.Password == "bemn")
                DialogResult = true;
            else
                DialogResult = false;

            this.Close();
        }
    }
}
