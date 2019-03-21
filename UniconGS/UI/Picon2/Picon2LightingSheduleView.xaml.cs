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
    /// Логика взаимодействия для Picon2LightingSheduleView.xaml
    /// </summary>
    public partial class Picon2LightingSheduleView : UserControl
    {
        public Picon2LightingSheduleView()
        {
            InitializeComponent();
        }

        public byte[] GetSchedule(string _name)
        {
            var vm = this.DataContext as Picon2LightingSheduleViewModel;
            return vm.GetCachedSchedule(_name);
        }

        public void SetScheduleToCache(string _name,byte[] _schedule)
        {
            var vm = this.DataContext as Picon2LightingSheduleViewModel;
            vm.SetCachedSchedule(_name, _schedule);
        }

        public async Task ReadAllSchedules()
        {
            var vm = this.DataContext as Picon2LightingSheduleViewModel;
            vm.GetLightingShedule.Execute(null);
            //await vm.ReadAllSchedules();

        }
        public void SetAutonomus()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var vm = this.DataContext as Picon2LightingSheduleViewModel;
                    vm.IsAutonomus=true;
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
                    var vm = this.DataContext as Picon2LightingSheduleViewModel;
                    vm.IsAutonomus = false;
                });

            }
            catch (Exception ex)
            {

            }
        }
    }
}
