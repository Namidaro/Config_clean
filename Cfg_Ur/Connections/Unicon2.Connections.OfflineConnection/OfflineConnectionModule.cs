using Microsoft.Practices.Unity;
using Prism.Modularity;
using Unicon2.Infrastructure;
using Unicon2.Infrastructure.Interfaces;
using Unicon2.Infrastructure.Services;

namespace Unicon2.Connections.OfflineConnection
{
    /// <summary>
    /// класс модульной интеграции в приложение
    /// </summary>
    public class OfflineConnectionModule : IModule
    {
        IUnityContainer _container;
        private IXamlResourcesService _xamlResourcesService;

        public OfflineConnectionModule(IUnityContainer container,IXamlResourcesService xamlResourcesService)
        {
            _container = container;
            _xamlResourcesService = xamlResourcesService;
        }
        /// <summary>
        /// метод, который вызавается при инициализации модуля
        /// </summary>
        public void Initialize()
        {
            //регистрация фабрики 
            _container.RegisterType<IDeviceConnectionFactory, OfflineConnectionFactory>(ApplicationGlobalNames
                .OFFLINE_CONNECTION_FACTORY_NAME,new ContainerControlledLifetimeManager());



            //регистрация ресурсов
            _xamlResourcesService.AddResourceAsGlobal("Resources/OfflineConnectionResources.xaml",this.GetType().Assembly);
        }
    }
}
