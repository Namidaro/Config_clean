using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UniconGS.Interfaces;
using UniconGS.UI.MRNetworking.Model;
using UniconGS.UI.MRNetworking.ViewModel;

namespace UniconGS.UI.MRNetworking
{
    /// <summary>
    /// Логика взаимодействия для MRNetwork.xaml
    /// </summary>
    public partial class MRNetwork : UserControl
    {
        public MRNetwork()
        {
            InitializeComponent();
            this.DataContext=new ModbusMemoryViewModel();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

      
    }
}
