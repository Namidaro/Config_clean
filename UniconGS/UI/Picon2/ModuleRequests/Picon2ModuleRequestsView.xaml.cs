using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniconGS.Enums;
using UniconGS.UI.Picon2.ModuleRequests;

namespace UniconGS.UI.Picon2.ModuleRequests
{
    /// <summary>
    /// Логика взаимодействия для Picon2ModuleRequestsView.xaml
    /// </summary>
    public partial class Picon2ModuleRequestsView : UserControl
    {
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        public Picon2ModuleRequestsView()
        {
            InitializeComponent();
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = e.Source as ComboBox;
            var test = cb.Name;
            string hexAsStr = test.Split('x').Last();
            int hexAsInt = Int32.Parse(hexAsStr, System.Globalization.NumberStyles.HexNumber);
            int iterator = hexAsInt;
            var vm = this.DataContext as Picon2ModuleRequestsViewModel;
            if (!vm.IsToggleCrate918Checked)
                iterator++;
            vm.ImageSRCList[hexAsInt] = vm.GetImageSRC(vm.GetModuleType(vm.ModuleListForUI[hexAsInt]));
            switch (vm.GetModuleType(vm.ModuleListForUI[hexAsInt]))
            {
                case (byte)ModuleSelectionEnum.MODULE_MS911:
                    {
                        var win = new Picon2CommunicationModule910SeriesView();
                        win.DataContext = new Picon2CommunicationModule910SeriesViewModel(0x0F, (byte)iterator, vm.ModuleListForUI[hexAsInt]);
                        win.ShowDialog();
                        break;
                    }
                case (byte)ModuleSelectionEnum.MODULE_MS910R:
                    {
                        var win = new Picon2CommunicationModule910SeriesView();
                        win.DataContext = new Picon2CommunicationModule910SeriesViewModel(0x0F, (byte)iterator, vm.ModuleListForUI[hexAsInt]);
                        win.ShowDialog();
                        break;
                    }
                case (byte)ModuleSelectionEnum.MODULE_MS911R:
                    {
                        var win = new Picon2CommunicationModule910SeriesView();
                        win.DataContext = new Picon2CommunicationModule910SeriesViewModel(0x0F, (byte)iterator, vm.ModuleListForUI[hexAsInt]);
                        win.ShowDialog();
                        break;
                    }
                case (byte)ModuleSelectionEnum.MODULE_MS915:
                    {
                        var win = new Picon2CommunicationModule915SeriesView();
                        win.DataContext = new Picon2CommunicationModule915SeriesViewModel(0x0E, (byte)iterator, vm.ModuleListForUI[hexAsInt]);
                        win.ShowDialog();
                        break;
                    }
                case (byte)ModuleSelectionEnum.MODULE_MS917:
                    {
                        var win = new Picon2CommunicationModule915SeriesView();
                        win.DataContext = new Picon2CommunicationModule915SeriesViewModel(0x0E, (byte)iterator, vm.ModuleListForUI[hexAsInt]);
                        win.ShowDialog();
                        break;
                    }
                case (byte)ModuleSelectionEnum.MODULE_MS915L:
                    {
                        //люксметр
                        vm.WriteLuxmetrRequest(iterator);
                        break;
                    }
            }
            vm.OnGenerateRequestsCommand();
        }
        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var vm = this.DataContext as Picon2ModuleRequestsViewModel;
                    vm.SetModuleErrors();
                });
                
            }
            catch (Exception ex)
            {

            }
            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}
        }
        public void SetAutonomus()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var vm = this.DataContext as Picon2ModuleRequestsViewModel;
                    vm.SetAutonomus();
                });

            }
            catch (Exception ex)
            {

            }
        }

        public void DisableAutonomus()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var vm = this.DataContext as Picon2ModuleRequestsViewModel;
                    vm.IsAutonomus = false;
                });

            }
            catch (Exception ex)
            {

            }
        }
    }
}
