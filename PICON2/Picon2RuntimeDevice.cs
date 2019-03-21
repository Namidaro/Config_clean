using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ULA.AddinsHost.Business.Device.DataTables;
using ULA.AddinsHost.Business.Driver.DataTables;
using ULA.AddinsHost.Business.Strings;
using ULA.Business;
using ULA.Business.AddressesContainer;
using ULA.Business.AddressesContainer.Entities;
using ULA.Business.Infrastructure.ApplicationModes;
using ULA.Business.Infrastructure.DataServices;
using ULA.Business.Infrastructure.DeviceDomain;
using ULA.Business.Infrastructure.DeviceStringKeys;
using ULA.Business.Infrastructure.Factories;
using ULA.Business.Infrastructure.PersistanceServices;
using ULA.Business.Infrastructure.TimerInterrogation;
using ULA.Business.LoggerServices;
using ULA.Common.AOM;
using ULA.Common.Formatters;
using ULA.Devices.PICON2.Business.Business;
using ULA.Devices.PICON2.Business.Factories;
using ULA.Interceptors.Application;

namespace ULA.Devices.PICON2.Business
{
    public class Picon2RuntimeDevice : RuntimeDeviceBase
    {
        private readonly Picon2StarterFactory _picon2StarterFactory;

        #region [Const]
        private const string NAME_PROPERTY = "Name";
        private const string ADDRESS_PROPERTY = "Address";
        private const string LENGTH_PROPERTY = "Length";

        protected  async void InitFormatters()
        {

            await Task.Run((() =>
             {

                DriverMomento.State.DataContainer.AddValue("DataTable", new DriverDataTable(this.CreateDataTableRowType()));
                var dataTable = DriverMomento.State.DataContainer.GetValue<IDriverDataTable>("DataTable");
                var commandManagementId = this.AddDriverData(DeviceStringKeys.DeviceTableTagKeys.COMMAND_MANAGENT_ID_NAME, 0x0000, 14);
                commandManagementId = dataTable.GetRowByName(DeviceStringKeys.DeviceTableTagKeys.COMMAND_MANAGENT_ID_NAME).Id;
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_SWITCHING_STARTER + 1, commandManagementId, new BytesToBooleanFormatter(1, 0));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_MANUALMODE_STARTER + 1, commandManagementId, new BytesToBooleanFormatter(1, 2));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_REPAIR_STARTER + 1, commandManagementId, new BytesToBooleanFormatter(1, 3));

                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_SWITCHING_STARTER + 2, commandManagementId, new BytesToBooleanFormatter(1, 4));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_MANUALMODE_STARTER + 2, commandManagementId, new BytesToBooleanFormatter(1, 6));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_REPAIR_STARTER + 2, commandManagementId, new BytesToBooleanFormatter(1, 7));

                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_SWITCHING_STARTER + 3, commandManagementId, new BytesToBooleanFormatter(0, 0));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_MANUALMODE_STARTER + 3, commandManagementId, new BytesToBooleanFormatter(0, 2));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_REPAIR_STARTER + 3, commandManagementId, new BytesToBooleanFormatter(0, 3));

                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_SWITCHING_STARTER + 4, commandManagementId, new BytesToBooleanFormatter(0, 4));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_MANUALMODE_STARTER + 4, commandManagementId, new BytesToBooleanFormatter(0, 6));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_REPAIR_STARTER + 4, commandManagementId, new BytesToBooleanFormatter(0, 7));

                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_SWITCHING_STARTER + 5, commandManagementId, new BytesToBooleanFormatter(3, 0));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_MANUALMODE_STARTER + 5, commandManagementId, new BytesToBooleanFormatter(3, 2));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_REPAIR_STARTER + 5, commandManagementId, new BytesToBooleanFormatter(3, 3));

                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_SWITCHING_STARTER + 6, commandManagementId, new BytesToBooleanFormatter(3, 4));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_MANUALMODE_STARTER + 6, commandManagementId, new BytesToBooleanFormatter(3, 6));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_REPAIR_STARTER + 6, commandManagementId, new BytesToBooleanFormatter(3, 7));

                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_SWITCHING_STARTER + 7, commandManagementId, new BytesToBooleanFormatter(2, 0));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_MANUALMODE_STARTER + 7, commandManagementId, new BytesToBooleanFormatter(2, 2));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_REPAIR_STARTER + 7, commandManagementId, new BytesToBooleanFormatter(2, 3));

                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_SWITCHING_STARTER + 8, commandManagementId, new BytesToBooleanFormatter(2, 4));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_MANUALMODE_STARTER + 8, commandManagementId, new BytesToBooleanFormatter(2, 6));
                this.AddDataRow(DeviceStringKeys.DeviceCommandsTagKeys.COMMAND_REPAIR_STARTER + 8, commandManagementId, new BytesToBooleanFormatter(2, 7));




                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_STATE_PATTERN + 1, commandManagementId, new BytesToBooleanFormatter(5, 0));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_MANUAL_MODE_PATTERN + 1, commandManagementId, new BytesToBooleanFormatter(5, 2));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_REPAIR + 1, commandManagementId, new BytesToBooleanFormatter(5, 3));

                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_STATE_PATTERN + 2, commandManagementId, new BytesToBooleanFormatter(5, 4));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_MANUAL_MODE_PATTERN + 2, commandManagementId, new BytesToBooleanFormatter(5, 6));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_REPAIR + 2, commandManagementId, new BytesToBooleanFormatter(5, 7));

                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_STATE_PATTERN + 3, commandManagementId, new BytesToBooleanFormatter(4, 0));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_MANUAL_MODE_PATTERN + 3, commandManagementId, new BytesToBooleanFormatter(4, 2));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_REPAIR + 3, commandManagementId, new BytesToBooleanFormatter(4, 3));

                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_STATE_PATTERN + 4, commandManagementId, new BytesToBooleanFormatter(4, 4));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_MANUAL_MODE_PATTERN + 4, commandManagementId, new BytesToBooleanFormatter(4, 6));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_REPAIR + 4, commandManagementId, new BytesToBooleanFormatter(4, 7));




                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_STATE_PATTERN + 5, commandManagementId, new BytesToBooleanFormatter(7, 0));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_MANUAL_MODE_PATTERN + 5, commandManagementId, new BytesToBooleanFormatter(7, 2));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_REPAIR + 5, commandManagementId, new BytesToBooleanFormatter(7, 3));

                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_STATE_PATTERN + 6, commandManagementId, new BytesToBooleanFormatter(7, 4));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_MANUAL_MODE_PATTERN + 6, commandManagementId, new BytesToBooleanFormatter(7, 6));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_REPAIR + 6, commandManagementId, new BytesToBooleanFormatter(7, 7));

                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_STATE_PATTERN + 7, commandManagementId, new BytesToBooleanFormatter(6, 0));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_MANUAL_MODE_PATTERN + 7, commandManagementId, new BytesToBooleanFormatter(6, 2));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_REPAIR + 7, commandManagementId, new BytesToBooleanFormatter(6, 3));

                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_STATE_PATTERN + 8, commandManagementId, new BytesToBooleanFormatter(6, 4));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_MANUAL_MODE_PATTERN + 8, commandManagementId, new BytesToBooleanFormatter(6, 6));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_IS_REPAIR + 8, commandManagementId, new BytesToBooleanFormatter(6, 7));





                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.POWER_DEFECT, commandManagementId, new BytesToBooleanFormatter(9, 0));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.MANAGEMENT_CHAINS_DEFECT, commandManagementId, new BytesToBooleanFormatter(9, 3));//для пикона2 перепутаны адреса неисправностей "управления" и "цепей управления" ->
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.PROTECTION_DEFECT, commandManagementId, new BytesToBooleanFormatter(9, 2));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.MANAGEMENT_DEFECT, commandManagementId, new BytesToBooleanFormatter(9, 1));// в самом устройстве, и никто это менять не будет... так и живем
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.FUSES_DEFECT, commandManagementId, new BytesToBooleanFormatter(9, 4));
                // this.AddDataRow("Defect.Controls", result.CommandManagementId, new BytesToBooleanFormatter(9, 5));
                //  this.AddDataRow("Defect.Controller", result.CommandManagementId, new BytesToBooleanFormatter(9, 7));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_MANAGEMENT_CONTROL + 1, commandManagementId, new BytesToBooleanFormatter(8, 0));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_MANAGEMENT_CONTROL + 2, commandManagementId, new BytesToBooleanFormatter(8, 1));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_MANAGEMENT_CONTROL + 3, commandManagementId, new BytesToBooleanFormatter(8, 2));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_MANAGEMENT_CONTROL + 4, commandManagementId, new BytesToBooleanFormatter(8, 3));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_MANAGEMENT_CONTROL + 5, commandManagementId, new BytesToBooleanFormatter(8, 4));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_MANAGEMENT_CONTROL + 6, commandManagementId, new BytesToBooleanFormatter(8, 5));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_MANAGEMENT_CONTROL + 7, commandManagementId, new BytesToBooleanFormatter(8, 6));
                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.STARTER_MANAGEMENT_CONTROL + 8, commandManagementId, new BytesToBooleanFormatter(8, 7));




                this.AddDataRow(DeviceStringKeys.DeviceTableTagKeys.ACKNOWLEDGE_VALUE, commandManagementId, new BytesToFailAckFormatter(27));
                AddDataRow("KEY_MAIN_MANAGEMENT_FAIL", commandManagementId, new BytesToBooleanFormatter(13, 3));

                AddressesContainer addressesContainer =
                    AddressesContainerManager.Load(this.DeviceMomento.State.DeviceType);
                foreach (AddressData addressData in addressesContainer.GetAllAddressData())
                //достает данные по адресам из контейнеров, которые подтягиваются из конфиг-файла
                {
                    this.AddDataRow(addressData.Name, commandManagementId,
                        new BytesToBooleanFormatter(addressData.ByteIndex, addressData.BitIndex));
                }
                var dateTimeId = this.AddDriverData(DeviceStringKeys.DeviceTableTagKeys.DATETIME_ID_NAME, ushort.Parse(addressesContainer.DateTimeContainer.Value, NumberStyles.HexNumber), 8);
                dateTimeId = dataTable.GetRowByName(DeviceStringKeys.DeviceTableTagKeys.DATETIME_ID_NAME).Id;

                this.AddDataRow(DeviceStringKeys.DeviceDateTimeTagKeys.DATETIME_YEAR, dateTimeId, new BytesToInt16FormatterForPicon2(3));
                this.AddDataRow(DeviceStringKeys.DeviceDateTimeTagKeys.DATETIME_MONTH, dateTimeId, new BytesToByteFormatter(5));
                this.AddDataRow(DeviceStringKeys.DeviceDateTimeTagKeys.DATETIME_DAY, dateTimeId, new BytesToByteFormatter(7));
                this.AddDataRow(DeviceStringKeys.DeviceDateTimeTagKeys.DATETIME_DAYINWEEK, dateTimeId, new BytesToByteFormatter(8));
                this.AddDataRow(DeviceStringKeys.DeviceDateTimeTagKeys.DATETIME_HOUR, dateTimeId, new BytesToByteFormatter(9));
                this.AddDataRow(DeviceStringKeys.DeviceDateTimeTagKeys.DATETIME_MINUTE, dateTimeId, new BytesToByteFormatter(11));
                this.AddDataRow(DeviceStringKeys.DeviceDateTimeTagKeys.DATETIME_SECOND, dateTimeId, new BytesToByteFormatter(13));
                this.AddDataRow(DeviceStringKeys.DeviceDateTimeTagKeys.DATETIME_MILLISECOND, dateTimeId, new BytesToByteFormatter(14));



                var signalLevelId = this.AddDriverData(DeviceStringKeys.DeviceTableTagKeys.SIGNAL_LEVEL_DATA_ID_NAME, 0x001F, 1);
                this.AddDataRow("SignalLevel", signalLevelId, new BytesToByteFormatter(1));


                var analogId = this.AddDriverData(DeviceStringKeys.DeviceAnalogMetersTagKeys.ANALOG_ID, 0x000E, 18);

                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.VOLTAGE_A, analogId, new BytesToIntVoltageFormatter(1));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.VOLTAGE_B, analogId, new BytesToIntVoltageFormatter(3));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.VOLTAGE_C, analogId, new BytesToIntVoltageFormatter(5));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.CURRENT_A, analogId, new BytesToIntCurrentFormatter(7));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.CURRENT_B, analogId, new BytesToIntCurrentFormatter(9));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.CURRENT_С, analogId, new BytesToIntCurrentFormatter(11));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.POWER_A, analogId, new BytesToIntPowerFormatter(13));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.POWER_B, analogId, new BytesToIntPowerFormatter(15));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.POWER_C, analogId, new BytesToIntPowerFormatter(17));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.STORED_ENERGY, analogId, new BytesToIntEnergyFormatter(23));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.STORED_ENERGY_1, analogId, new BytesToIntEnergyFormatter(25));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.STORED_ENERGY_FOR_MOUNTH, analogId, new BytesToIntEnergyFormatter(27));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.STORED_ENERGY_FOR_MOUNTH_1, analogId, new BytesToIntEnergyFormatter(29));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.STORED_ENERGY_FOR_DAY, analogId, new BytesToIntEnergyFormatter(31));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.STORED_ENERGY_FOR_DAY_1, analogId, new BytesToIntEnergyFormatter(33));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.VOLTAGE_A_MSA961, analogId, new BytesToIntVoltageFormatter(7));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.VOLTAGE_B_MSA961, analogId, new BytesToIntVoltageFormatter(9));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.VOLTAGE_C_MSA961, analogId, new BytesToIntVoltageFormatter(11));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.CURRENT_A_MSA961, analogId, new BytesToIntCurrentFormatter(1));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.CURRENT_B_MSA961, analogId, new BytesToIntCurrentFormatter(3));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.CURRENT_С_MSA961, analogId, new BytesToIntCurrentFormatter(5));

                var meterDateTime = this.AddDriverData(DeviceStringKeys.DeviceAnalogMetersTagKeys.METER_DATE_TIME, 0x210, 16);


                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.ANALOG_METER_TIME, meterDateTime, new StringFormatter(16));
                this.AddDataRow(DeviceStringKeys.DeviceAnalogMetersTagKeys.ANALOG_METER_DATE, meterDateTime, new StringFormatter(0));






            }));


            base.InitFormatters();



        }

        #endregion [Const]

        #region [Private fields]

        private IPersistanceService _persistance;
        private Timer _reconectTimer;

        private int _failedReconectAttemp;





        #endregion [Private fields]


        public Picon2RuntimeDevice(IApplicationSettingsService applicationSettingsService,
            IRuntimeModeDriversService runtimeModeDriversService, Picon2StarterFactory picon2StarterFactory,
            ApplicationConnectionService.ApplicationConnectionService applicationConnectionService,
            IDeviceTimerInterrogationService timerInterrigationService, IDefectState defectState, IDeviceDataCache deviceDataCache, IAnalogData analogData, IResistorFactory resistorFactory, ICustomItemsFactory customItemsFactory, ConnectionLogger connectionLogger) : base(
            applicationSettingsService, runtimeModeDriversService,
            applicationConnectionService, timerInterrigationService, defectState, deviceDataCache, analogData, resistorFactory, customItemsFactory, connectionLogger)
        {
            _picon2StarterFactory = picon2StarterFactory;
            Picon2ModuleInfo = new Picon2ModuleInfo();
        }

        public Picon2ModuleInfo Picon2ModuleInfo { get; set; }

        #region Overrides of RuntimeDeviceBase

        #region Overrides of RuntimeDeviceBase

        public override async Task UpdateSignature()
        {
            if (!TcpDeviceConnection.LastTransactionSucceed) return;
            if (ConnectionModuleId == 0) return;
            var signBytes = await TcpDeviceConnection.ExecuteFunction12Async((byte)ConnectionModuleId, "Get Picon Signature", 0xF0);
            if (signBytes == null)
            {
                signBytes = await TcpDeviceConnection.ExecuteFunction12Async((byte)ConnectionModuleId, "Get Picon Signature", 0xF0);
            }
            string signature = Encoding.UTF8.GetString(signBytes);
            DeviceSignature = signature;
            DeviceValuesUpdated?.Invoke();
        }

        public override async Task UpdateSignal()
        {
            if (!TcpDeviceConnection.LastTransactionSucceed) return;
            if (ConnectionModuleId == 0) return;
            var signalLevelBytes = await TcpDeviceConnection.ExecuteFunction12Async((byte)ConnectionModuleId, "Get Picon SignalLevel", 0x60);
            if (signalLevelBytes == null)
            {
                signalLevelBytes = await TcpDeviceConnection.ExecuteFunction12Async((byte)ConnectionModuleId, "Get Picon SignalLevel", 0x60);
            }
            if (signalLevelBytes != null)
            {
                AnalogData.SignalLevel = signalLevelBytes[0];
                AnalogData.AnalogDataUpdated?.Invoke();
                DeviceValuesUpdated?.Invoke();
            }

        }

        public override async Task InitializeAsync()
        {
            if (await InitializeConfigData())
            {
                await base.InitializeAsync();
            }
        }

        #endregion


        private async Task<bool> InitializeConfigData()
        {
            ushort address = 0x3004;
            ushort lenghtData = 0x007B;
            int chanelCount = 8;
            List<byte> bytes = new List<byte>();
            try
            {
                bytes.AddRange(await _tcpDeviceConnection.GetDataByAddressAsync(address, 70, RequestStrings.GET_STARTER_DATA));
                bytes.AddRange(await _tcpDeviceConnection.GetDataByAddressAsync((ushort)(address + 70), 70, RequestStrings.GET_STARTER_DATA));
            }
            catch (Exception e)
            {
                return false;
            }
         

            //var data3004 = await _tcpDeviceConnection.GetDataByAddressAsync(0x3004, 1, RequestStrings.GET_STARTER_DATA);
            if (bytes.Count == 0)
            {
                return false;
            }
            ConnectionModuleId = bytes[0x3004 + 1 - address];
            StartersOnDevice.Clear();
            StartersOnDevice.AddRange(_picon2StarterFactory.CreateStarters(DeviceMomento.State, bytes.ToArray()));
            return true;

        }

        #endregion


        /// <summary>
        ///     Добавляет с таблицу данных устройства новую строку.
        /// </summary>
        /// <param name="tag">Тэг строки данных</param>
        /// <param name="driverDataId">Ссылка на Id блока данных в таблице драйвера</param>
        /// <param name="formatter">Объект конвертирующий данные между байтовым(как хранится в устройстве) и исходным
        /// (представленным в программе) типом данным</param>
        protected void AddDataRow(string tag, Guid driverDataId, BinaryFormatterBase formatter)
        {
            var row = new DefaultDataRow
            {
                DriverDataId = driverDataId,
                Formatter = formatter,
                Tag = tag
            };
            DeviceMomento.State.DataTable.AddObject(row);
        }

        public int ConnectionModuleId { get; set; }

        /// <summary>
        ///     Добавляет в таблицу данных драйвера новую строку с техническимим данными переданными в параметрах
        /// </summary>
        /// <param name="name">Имя строки данных драйвера</param>
        /// <param name="address">Адррес начала блока данных в устройстве</param>
        /// <param name="length">Длина блока данных</param>
        /// <param name="isUpdatable">Являются ли эти данные обновляемыми(могу ли обновляться на стороне устройства)</param>
        /// <returns>Id строки данного в таблице данных драйвера</returns>
        private Guid AddDriverData(string name, ushort address, ushort length)
        {

            var row = this.DriverMomento.State.DataContainer.GetValue<IDriverDataTable>("DataTable").CreateRow();
            row.Properties[NAME_PROPERTY].Value = name;
            row.Properties[ADDRESS_PROPERTY].Value = address;
            row.Properties[LENGTH_PROPERTY].Value = length;

            this.DriverMomento.State.DataContainer.GetValue<IDriverDataTable>("DataTable").PushRow(row);
            return row.Id;
        }
        private AomEntityType CreateDataTableRowType()
        {
            return new AomEntityType(typeof(AomDataTableRowEntity),
                new AomPropertyTypeCollection(new List<AomPropertyType>
                {
                    new AomPropertyType(NAME_PROPERTY, typeof (string)),
                    new AomPropertyType(ADDRESS_PROPERTY, typeof (ushort)),
                    new AomPropertyType(LENGTH_PROPERTY, typeof (ushort)),
                    new AomPropertyType("Value", typeof(byte[]))
                }));
        }
    }
}
