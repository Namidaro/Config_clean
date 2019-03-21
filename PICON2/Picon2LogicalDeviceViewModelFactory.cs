using System;
using ULA.AddinsHost.Business.Device;
using ULA.AddinsHost.ViewModel.Device;
using ULA.AddinsHost.ViewModel.Factories;
using ULA.Devices.PICON2.Business.Factories;
using ULA.Devices.Presentation.Runtime;
using ULA.Interceptors.IoC;

namespace ULA.Devices.PICON2.Business
{
    /// <summary>
    ///     Repressents PICONGS logical device factory functionality
    ///     Представляет фабрику для PICONGS устройств в разных режимах
    /// </summary>
    public class Picon2LogicalDeviceViewModelFactory : ILogicalDeviceViewModelFactory
    {
        #region [Private fields]

        private readonly IIoCContainer _container;
        private readonly Picon2DeviceFactory _picon2DeviceFactory;

        #endregion [Private fields]

        #region [Ctor's]

        /// <summary>
        ///     Creates an instance of <see cref="PicongsLogicalDeviceViewModelFactory" />
        /// </summary>
        /// <param name="container">
        ///     An instance of <see cref="IIoCContainer" /> to use
        /// </param>
        public Picon2LogicalDeviceViewModelFactory(IIoCContainer container, Picon2DeviceFactory picon2DeviceFactory)
        {
            this._container = container;
            _picon2DeviceFactory = picon2DeviceFactory;
        }

        #endregion [Ctor's]

        #region [ILogicalDeviceViewModelFactory members]

        /// <summary>
        ///     Creates an instance of <see cref="IRuntimeDeviceViewModel" />
        /// </summary>
        /// <param name="deviceMomento"></param>
        /// <returns>
        ///     Created instance of <see cref="IRuntimeDeviceViewModel" />
        /// </returns>
        public IRuntimeDeviceViewModel CreateRuntimeLogicalDeviceViewModel(IDeviceMomento deviceMomento, Guid id)
        {
            RuntimeDeviceViewModel runtimeDeviceViewModel = this._container.Resolve<RuntimeDeviceViewModel>();
            runtimeDeviceViewModel.Model = _picon2DeviceFactory.CreateLogicalDevice(deviceMomento);
            runtimeDeviceViewModel.Model.DeviceId = id;
            return runtimeDeviceViewModel;
        }

        /// <summary>
        ///     Creates an instance of <see cref="IConfigLogicalDevice" />
        /// </summary>
        /// <returns>
        ///     Created instance of <see cref="IConfigLogicalDevice" />
        /// </returns>
        public IConfigLogicalDevice CreateConfigLogicalDevice()
        {
            return this._container.Resolve<Picon2ConfigLogicalDevice>();
        }

        #endregion [ILogicalDeviceViewModelFactory members]
    }
}