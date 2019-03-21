using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;

namespace UniconGS
{
  public  class UniconGsBootstrapper: UnityBootstrapper
    {

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<DeviceSelection>();
        }
        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow?.Show();

        }


        /// <summary>
        /// регистрация зависимостей
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            InitializeModules();
        }

        protected override void ConfigureModuleCatalog()
        {
            var catalog = (ModuleCatalog)ModuleCatalog;

            base.ConfigureModuleCatalog();
        }


    }
}
