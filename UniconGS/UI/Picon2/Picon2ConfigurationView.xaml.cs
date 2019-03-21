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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniconGS.UI.Picon2.ViewModel;

namespace UniconGS.UI.Picon2
{
    /// <summary>
    /// Логика взаимодействия для Picon2ConfigurationView.xaml
    /// </summary>
    public partial class Picon2ConfigurationView
    {
        /// <summary>
        ///     Конструктор вьюшеи конфигурации пикона
        /// </summary>
        /// <param name="viewModel">вью-модель для вьюшки</param>
        /// <summary>
        ///     Конструктор вьюшеи конфигурации руно
        /// </summary>
        /// <param name="container">контейнер для поиска вью-модели</param>
        public Picon2ConfigurationView( )
        {
            InitializeComponent();
        }

        public byte[] GetConfig()
        {
            var vm = this.DataContext as PICON2ConfigurationModeViewModel;
            return vm.CreateDataPackage();
        }

        public void SetConfig(byte[] _config)
        {
            var vm = this.DataContext as PICON2ConfigurationModeViewModel;
            vm.InitializeFromSettings(_config);
        }
        public async Task GetAllConfig()
        {
            var vm = this.DataContext as PICON2ConfigurationModeViewModel;
            vm.GetConfiguration.Execute(null);
        }

        //TODO: make updateState for picon2
        public async Task UpdateState()
        {

            ushort[] value;
            {
                value = await RTUConnectionGlobal.GetDataByAddress(1, 0x8200, 61);
            }

            //ImportComplete(value);

        }

        private void TimeToStartTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox)
            {
                if (((sender as TextBox).Text + e.Text).Length == 0)
                {
                    (sender as TextBox).Text = "0";
                }
                e.Handled = IsCorrectTimeToStart((sender as TextBox).Text + e.Text);
            }
            else
            {
                e.Handled = true;
            }
        }

        private bool IsCorrectTimeToStart(string text)
        {
            bool result = true;
            int intResult;
            if (Int32.TryParse(text, out intResult))
            {
                if (intResult >= 0 && intResult <= 65535)
                {
                    result = false;
                }
            }
            return result;
        }

        private void TimeToStartTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                if ((sender as TextBox).Text.Length == 0)
                {
                    (sender as TextBox).Text = "0";
                }
            }
            else
            {
                e.Handled = true;
            }
        }
        private void _cbManagemnt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = this.DataContext as PICON2ConfigurationModeViewModel;
            if (!vm.IsInitializeNow)
            {
                for (int i = 0; i < vm.ManagementKuSelected.Count; i++)
                {
                    if (vm.TempManagmentCollection[i] != vm.ManagementKuSelected[i])
                    {
                        //vm.ManagementKuSelected[i] = vm.TempManagmentCollection[i];
                        vm.TempManagmentCollection[i] = vm.ManagementKuSelected[i];
                        return;
                    }
                }
            }
        }
        private void FaultMask_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = this.DataContext as PICON2ConfigurationModeViewModel;
            if (!vm.IsInitializeNow)
            {
                for (int i = 0; i < vm.OutputsKuSelected.Count; i++)
                {
                    if (vm.TempOutputKuSelected[i] != vm.OutputsKuSelected[i])
                    {
                        //                        vm.OutputsKuSelected[i] = vm.TempOutputKuSelected[i];

                        vm.TempOutputKuSelected[i] = vm.OutputsKuSelected[i];
                        return;
                    }

                }
            }
        }

        private void Selector_OnSelectionChangedInv(object sender, SelectionChangedEventArgs e)
        {
            var vm = this.DataContext as PICON2ConfigurationModeViewModel;
            if (!vm.IsInitializeNow)
            {
                for (int i = 0; i < vm.OutputsKuSelected.Count; i++)
                {
                    if (vm.TempOutputKuSelectedInv[i] != vm.OutputsKuSelectedInv[i])
                    {
                        //vm.OutputsKuSelectedInv[i] = vm.TempOutputKuSelectedInv[i];
                        vm.TempOutputKuSelectedInv[i] = vm.OutputsKuSelectedInv[i];

                        return;
                    }
                }
            }
        }

        public void SetAutonomus()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var vm = this.DataContext as PICON2ConfigurationModeViewModel;
                    vm.IsAutonomus = true;
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
                    var vm = this.DataContext as PICON2ConfigurationModeViewModel;
                    vm.IsAutonomus = false;
                });

            }
            catch (Exception ex)
            {

            }
        }
    }
}

