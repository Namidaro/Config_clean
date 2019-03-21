using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NModbus4;
using NModbus4.Device;
using NModbus4.IO;
using Unicon2.Connections.ModBusRtuConnection.Interfaces;
using Unicon2.Connections.ModBusRtuConnection.Interfaces.Factories;
using Unicon2.Infrastructure.BaseItems;
using Unicon2.Infrastructure.Connection;
using Unicon2.Infrastructure.DeviceInterfaces;
using Unicon2.Infrastructure.Services;
using Unicon2.Infrastructure.Services.LogService;

namespace Unicon2.Connections.ModBusRtuConnection.Model
{

    /// <summary>
    /// класс подключения по ModBusRtu
    /// </summary>
    [DataContract(Namespace = "ModBusRtuConnectionNS",IsReference = true)]
    public class ModBusRtuConnection :Disposable, IModbusRtuConnection
    {
        private IComConnectionManager _connectionManager;
        private IUnityContainer _container;
        private string _openedPort;
        private ILocalizerService _localizerService;
        private IComPortConfigurationFactory _comPortConfigurationFactory;
        private  IDevicesContainerService _devicesContainerService;
        private byte _slaveId;
        private bool _isConnectionLost;
        private IModbusMaster _currentModbusMaster;
        private IDeviceLogger _deviceLogger;

        #region Overrides of ModbusDataProvider

   

        #endregion


        public ModBusRtuConnection(IComConnectionManager connectionManager,
            IUnityContainer container) : base()
        {
            _connectionManager = connectionManager;
            _container = container;

            ComPortConfiguration = _comPortConfigurationFactory.CreateComPortConfiguration();
        }

        [DataMember]
        public string PortName { get; set; }

        [DataMember]
        public IComPortConfiguration ComPortConfiguration { get; set; }

     

        /// <summary>
        /// название подключения
        /// </summary>
        public string ConnectionName => "ModBus RTU";

        public async Task<bool> TryOpenConnectionAsync(bool isThrowingException, IComPortConfiguration comPortConfiguration)
        {
            //if ((_openedPort != null) && (_openedPort == PortName)) return true;
            try
            {
              
                IModbusSerialMaster modbusSerialMaster=null;
                await Task.Run((() =>
                {
                    IStreamResource streamResource = _connectionManager.GetSerialPortAdapter(PortName,comPortConfiguration);
                    modbusSerialMaster = ModbusSerialMaster.CreateRtu(streamResource);
                }));
              
                if (modbusSerialMaster != null)
                {
                    _currentModbusMaster?.Dispose();
                    _currentModbusMaster = modbusSerialMaster;
                }
                else
                {
                    throw new Exception();
                }
                _slaveId = SlaveId;
                //  QueryQueue.Initialize(_currentModbusSerialMaster,deviceLogger,(() => LastQueryStatusChangedAction?.Invoke()));
                _openedPort = PortName;
                
            }
            catch (Exception e)
            {
                if (isThrowingException) throw e;
                return false;
            }
            return true;
        }

        public Action<bool> LastQueryStatusChangedAction { get; set; }


        public void CloseConnection()
        {
            _currentModbusMaster.Dispose();
            
        }


        [DataMember]

        public byte SlaveId
        {
            get { return _slaveId; }
            set { _slaveId = value; }
        }
        
        protected override void OnDisposing()
        {
            _currentModbusMaster?.Dispose();
            base.OnDisposing();
        }


    }

}
