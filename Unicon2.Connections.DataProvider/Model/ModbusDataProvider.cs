using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NModbus4.Device;
using Unicon2.Infrastructure.BaseItems;
using Unicon2.Infrastructure.Connection;
using Unicon2.Infrastructure.DeviceInterfaces;
using Unicon2.Infrastructure.Interfaces;

namespace Unicon2.Connections.DataProvider.Model
{
    [DataContract()]
    public abstract class ModbusDataProvider:Disposable,IDataProvider,IInitializableFromContainer
    {
        private IQueryResultFactory _queryResultFactory;
        protected bool _isInitialized;
        protected IModbusMaster _currentModbusMaster;
        protected byte _slaveId=0;
        protected bool _lastQuerySucceed;




        public ModbusDataProvider()
        {
          
        }


        #region Implementation of IDataProvider

        public async Task<IQueryResult<ushort[]>> ReadHoldingResgistersAsync(ushort startAddress, ushort numberOfPoints, string dataTitle)
        {
            IQueryResult<ushort[]> queryResult = _queryResultFactory.CreateDefaultQueryResult<ushort[]>();
            if (!CheckConnection(queryResult)) return queryResult;
            try
            {
                queryResult.Result = await _currentModbusMaster.ReadHoldingRegistersAsync(_slaveId, startAddress, numberOfPoints);
                List<string> results = queryResult.Result.Select((arg => arg.ToString())).ToList();
                string resStr = "";
                foreach (var res in results)
                {
                    resStr += res;
                    resStr += " ";
                }
                queryResult.IsSuccessful = true;

            }
            catch (Exception e)
            {
                queryResult.IsSuccessful = false;
            }
            return queryResult;
        }

        public async Task<IQueryResult<bool>> ReadCoilStatusAsync(ushort coilAddress, string dataTitle)
        {
            IQueryResult<bool> queryResult = _queryResultFactory.CreateDefaultQueryResult<bool>();
            if (!CheckConnection(queryResult)) return queryResult;

            try
            {
                queryResult.Result = (await _currentModbusMaster.ReadCoilsAsync(_slaveId, coilAddress, 1))[0];
                queryResult.IsSuccessful = true;
            }
            catch (Exception e)
            {
                queryResult.IsSuccessful = false;
            }
            return queryResult;
        }

        public async Task<IQueryResult<bool[]>> ReadCoilStatusAsync(ushort coilAddress, string dataTitle, ushort numberOfPoints)
        {
            IQueryResult<bool[]> queryResult = _queryResultFactory.CreateDefaultQueryResult<bool[]>();
            if (!CheckConnection(queryResult)) return queryResult;

            try
            {
                queryResult.Result = await _currentModbusMaster.ReadCoilsAsync(_slaveId, coilAddress, numberOfPoints);
                string resStr = "";
                foreach (var res in queryResult.Result)
                {
                    resStr += res;
                    resStr += " ";
                }
                queryResult.IsSuccessful = true;
            }
            catch (Exception e)
            {
                queryResult.IsSuccessful = false;
            }
            return queryResult;
        }

        public async Task<IQueryResult> WriteMultipleRegistersAsync(ushort startAddress, ushort[] dataToWrite, string dataTitle)
        {
            IQueryResult queryResult = _queryResultFactory.CreateDefaultQueryResult();
            if (!CheckConnection(queryResult)) return queryResult;

            string dataStr = "";
            foreach (var res in dataToWrite)
            {
                dataStr += res;
                dataStr += " ";
            }
            try
            {
                await _currentModbusMaster.WriteMultipleRegistersAsync(_slaveId, startAddress, dataToWrite);

                queryResult.IsSuccessful = true;
            }
            catch (Exception e)
            {
                queryResult.IsSuccessful = false;

            }
            return queryResult;
        }



        public async Task<IQueryResult> WriteSingleCoilAsync(ushort coilAddress, bool valueToWrite, string dataTitle)
        {
            IQueryResult queryResult = _queryResultFactory.CreateDefaultQueryResult();
            if (!CheckConnection(queryResult)) return queryResult;

            try
            {
                await _currentModbusMaster.WriteSingleCoilAsync(_slaveId, coilAddress, valueToWrite);
                queryResult.IsSuccessful = true;
            }
            catch (Exception e)
            {
                queryResult.IsSuccessful = false;
            }
            return queryResult;
        }

        public async Task<IQueryResult> WriteSingleRegisterAsync(ushort registerAddress, ushort valueToWrite, string dataTitle)
        {
            IQueryResult queryResult = _queryResultFactory.CreateDefaultQueryResult();
            if (!CheckConnection(queryResult)) return queryResult;

            try
            {
                await _currentModbusMaster.WriteSingleRegisterAsync(_slaveId, registerAddress, valueToWrite);
                queryResult.IsSuccessful = true;
            }
            catch (Exception e)
            {
                queryResult.IsSuccessful = false;
            }
            return queryResult;
        }

        public bool LastQuerySucceed
        {
            get
            {
                return _lastQuerySucceed;
                
            }
        }


        

        #endregion

        #region Implementation of IInitializableFromContainer

        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        public virtual void InitializeFromContainer(IUnityContainer container)
        {
            _queryResultFactory = container.Resolve<IQueryResultFactory>();
            _isInitialized = true;

        }

        #endregion
    }
}
