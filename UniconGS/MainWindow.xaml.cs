using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Text;
using System.Windows.Threading;
using UniconGS.UI;
using UniconGS.Source;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Win32;
using NModbus4.Device;
using NModbus4.IO;
using NModbus4.Serial;
using UniconGS.Annotations;
using UniconGS.UI.Configuration;
using UniconGS.UI.DiscretModules;
using UniconGS.UI.GPRS;
using UniconGS.UI.HeatingSchedule;
using UniconGS.UI.Journal;
using UniconGS.UI.MRNetworking.ViewModel;
using UniconGS.UI.Picon2.ViewModel;
using UniconGS.UI.Schedule;
using UniconGS.UI.Settings;
using UniconGS.UI.Time;
using TabControl = System.Windows.Controls.TabControl;
using static UniconGS.GSMConnection;
using UniconGS.Enums;
using UniconGS.UI.Picon2.ModuleRequests;

namespace UniconGS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Globals

        private Timer _uiUpdateTimer;
        private Slot _connectionChecker;
        private Config _config = null;
        private bool _isAutonomous = false;
        private Stack<bool> _connectionFail = new Stack<bool>(5);
        private bool _isAfterConnectionLost = false;

        #endregion

        #region Threading
        private ManualResetEvent _shutDownEvent = new ManualResetEvent(true);
        private Thread _work;
        //private int _updateRate = 1;
        public static bool isAutonomus = false;
        private static SemaphoreSlim _semaphoreSlim;
        private bool _b;
        public bool ConnectionLost = false;
        #endregion
        int AutonomusCheck;
        public Schedule LightningSchedule => this.uiLightingSchedule;
        public Timer UITimer
        {
            get
            {
                return _uiUpdateTimer;
            }
            set
            {
                this._uiUpdateTimer = value;
            }
        }

        public MainWindow()
        {

            InitializeComponent();

            _semaphoreSlim = new SemaphoreSlim(1, 1);
            isAutonomus = false;
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                uiScrollViewerPicon2.Visibility = Visibility.Visible;
                uiPicon2Diagnostics.Visibility = Visibility.Visible;
                uiLogicConfig.Visibility = Visibility.Collapsed;
                uiLogicConfigTab.Visibility = Visibility.Collapsed;


                picon2LightingSheduleView.DataContext = new Picon2LightingSheduleViewModel();
                uiPicon2ConfigurationView.DataContext = new PICON2ConfigurationModeViewModel();
                uiSheduleLightining.Visibility = Visibility.Collapsed;
                uiSheduleBackLight.Visibility = Visibility.Collapsed;
                uiSheduleIllumination.Visibility = Visibility.Collapsed;
                uiSheduleEconomy.Visibility = Visibility.Collapsed;
                uiSheduleHeating.Visibility = Visibility.Collapsed;
                LogicTab.Visibility = Visibility.Visible;


                uiGPRSConfig.Visibility = Visibility.Collapsed;
                uiGSMConnection.IsEnabled = true;
                uiGPRSTab.Visibility = Visibility.Collapsed;




                uiPicon2ModuleRequests.Visibility = Visibility.Visible;
                Picon2ModuleRequest.DataContext = new Picon2ModuleRequestsViewModel();

                uiFuseErrors.Visibility = Visibility.Collapsed;
                uiTurnOnError.Visibility = Visibility.Collapsed;
                uiPicon2ChannelManagement.Visibility = Visibility.Visible;
            }
            else
            {
                uiPicon2ConfigurationViewTab.Visibility = Visibility.Collapsed;
                picon2ScheduleTab.Visibility = Visibility.Collapsed;
                uiPicon2ConfigurationView.Visibility = Visibility.Collapsed;
                uiPicon2ModuleRequests.Visibility = Visibility.Collapsed;

                uiFuseErrors.Visibility = Visibility.Visible;
                uiTurnOnError.Visibility = Visibility.Visible;
                uiPicon2ChannelManagement.Visibility = Visibility.Collapsed;
            }
            InitSlots();

            this.uiTime.ShowMessage += new Time.ShowMessageEventHandler(ShowMessage);
            RTUConnectionGlobal.ConnectionLostAction += () =>
            {
                //if (isAutonomus == false)
                //{
                Application.Current.Dispatcher.Invoke(SetAllAutonomous);
                //isAutonomus = true;
                //}
                //else
                //{
                //}

            };
            this.uiSettings.ReadAll += async (sender, args) =>
            {
                this.uiSettings.uiOpenSettings.IsEnabled = false;
                this.uiSettings.uiSaveSettings.IsEnabled = false;
                //TODO: make decision on what device connected and update uiLogicConfig or picon2LogicConfig
                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                {
                    await this.uiPicon2ConfigurationView.GetAllConfig();
                    await this.picon2LightingSheduleView.ReadAllSchedules();
                }
                else
                {
                    await this.uiLightingSchedule.UpdateState();
                    await this.uiBacklightSchedule.UpdateState();
                    await this.uiIlluminationSchedule.UpdateState();
                    await this.uiEnergySchedule.UpdateState();
                    await this.uiLogicConfig.UpdateState();
                    await this.uiHeatingSchedule.UpdateState();
                    this.ShowMessage("Чтение настроек из устройства прошло успешно." + Environment.NewLine + "Чтение конфигурации прошло успешно." + Environment.NewLine +
                    "Чтение графика освещения прошло успешно." + Environment.NewLine + "Чтение графика подсветки прошло успешно." + Environment.NewLine + "Чтение графика иллюминации прошло успешно."
                    + Environment.NewLine + "Чтение графика энергосбережения прошло успешно." + Environment.NewLine + "Чтение графика обогрева прошло успешно.",

                           "Чтение настроек", MessageBoxImage.Information);

                }
                //this.ShowMessage("Чтение настроек из устройства прошло успешно." + Environment.NewLine + "Чтение конфигурации прошло успешно." + Environment.NewLine +
                //    "Чтение графика освещения прошло успешно." + Environment.NewLine + "Чтение графика подсветки прошло успешно." + Environment.NewLine + "Чтение графика иллюминации прошло успешно."
                //    + Environment.NewLine + "Чтение графика энергосбережения прошло успешно." + Environment.NewLine + "Чтение графика обогрева прошло успешно.",

                //           "Чтение настроек", MessageBoxImage.Information);
                this.uiSettings.uiOpenSettings.IsEnabled = true;
                this.uiSettings.uiSaveSettings.IsEnabled = true;
            };
            this.uiSettings.WriteAll += async (sender, args) =>
            {
                await this.uiLightingSchedule.WriteAll();
                await this.uiBacklightSchedule.WriteAll();
                await this.uiIlluminationSchedule.WriteAll();
                await this.uiEnergySchedule.WriteAll();
                await this.uiLogicConfig.WriteAll();
                await this.uiHeatingSchedule.WriteAll();
                this.ShowMessage("Запись настроек в устройство прошла успешно" + Environment.NewLine + "Запись конфигурации прошла успешно." + Environment.NewLine +
                 "Запись графика освещения прошла успешно." + Environment.NewLine + "Запись графика подстветки прошла успешно." + Environment.NewLine + "Запись графика иллюминации прошла успешно."
                 + Environment.NewLine + "Запись графика энергосбережения прошла успешно." + Environment.NewLine + "Запись графика обогрева прошла успешно.",

                        "Запись настроек", MessageBoxImage.Information);
            };
            this.uiLogicConfig.ShowMessage += new LogicConfig.ShowMessageEventHandler(ShowMessage);
            this.uiLogicConfig.SaveInFile += new LogicConfig.SaveInFileEventHandler(uiLogicConfig_SaveInFile);
            this.uiLogicConfig.OpenFromFile += new LogicConfig.OpenFromFileEventHandler(uiLogicConfig_OpenFromFile);
            this.uiLightingSchedule.OpenFromFile += new Schedule.OpenFromFileEventHandler(uiLightingSchedule_OpenFromFile);
            this.uiLightingSchedule.SaveInFile += new Schedule.SaveInFileEventHandler(uiLightingSchedule_SaveInFile);
            this.uiLightingSchedule.ShowMessage += new Schedule.ShowMessageEventHandler(ShowMessage);
            this.uiBacklightSchedule.SaveInFile += new Schedule.SaveInFileEventHandler(uiLightingSchedule_SaveInFile);
            this.uiBacklightSchedule.OpenFromFile += new Schedule.OpenFromFileEventHandler(uiLightingSchedule_OpenFromFile);
            this.uiBacklightSchedule.ShowMessage += new Schedule.ShowMessageEventHandler(ShowMessage);
            this.uiIlluminationSchedule.SaveInFile += new Schedule.SaveInFileEventHandler(uiLightingSchedule_SaveInFile);
            this.uiIlluminationSchedule.OpenFromFile += new Schedule.OpenFromFileEventHandler(uiLightingSchedule_OpenFromFile);
            this.uiIlluminationSchedule.ShowMessage += new Schedule.ShowMessageEventHandler(ShowMessage);
            this.uiEnergySchedule.OpenFromFile += new Schedule.OpenFromFileEventHandler(uiEnergySchedul_OpenFromFile);
            this.uiEnergySchedule.SaveInFile += new Schedule.SaveInFileEventHandler(uiEnergySchedul_SaveInFile);
            this.uiEnergySchedule.ShowMessage += new Schedule.ShowMessageEventHandler(ShowMessage);
            this.uiGPRSConfig.ShowMessage += new GPRSConfiguration.ShowMessageEventHandler(ShowMessage);
            this.uiHeatingSchedule.ShowMessage += new HeatingSchedule.ShowMessageEventHandler(ShowMessage);
            this.uiSystemJournal.ShowMessage += new SystemJournal.ShowMessageEventHandler(ShowMessage);
            this.uiLightingSchedule.StartWork += new Schedule.StartWorkEventHandler(ControlStartWork);
            this.uiBacklightSchedule.StartWork += new Schedule.StartWorkEventHandler(ControlStartWork);
            this.uiIlluminationSchedule.StartWork += new Schedule.StartWorkEventHandler(ControlStartWork);
            this.uiHeatingSchedule.StartWork += new HeatingSchedule.StartWorkEventHandler(ControlStartWork);
            this.uiGPRSConfig.StartWork += new GPRSConfiguration.StartWorkEventHandler(ControlStartWork);
            this.uiLogicConfig.StartWork += new LogicConfig.StartWorkEventHandler(ControlStartWork);
            this.uiLightingSchedule.StopWork += new Schedule.StopWorkEventHandler(ControlStopWork);
            this.uiBacklightSchedule.StopWork += new Schedule.StopWorkEventHandler(ControlStopWork);
            this.uiIlluminationSchedule.StopWork += new Schedule.StopWorkEventHandler(ControlStopWork);
            this.uiHeatingSchedule.StopWork += new HeatingSchedule.StopWorkEventHandler(ControlStopWork);
            this.uiGPRSConfig.StopWork += new GPRSConfiguration.StopWorkEventHandler(ControlStopWork);
            this.uiLogicConfig.StopWork += new LogicConfig.StopWorkEventHandler(ControlStopWork);
            this.uiDisconnect.IsEnabled = false;
            this.uiAutonomous.Click += new RoutedEventHandler(uiAutonomous_Click);
            this.uiDisconnectBtn.Click += new RoutedEventHandler(uiDisconnectBtn_Click);
            this.uiConnectBtn.Click += new RoutedEventHandler(uiConnectBtn_Click);
            this.uiAutonomousBtn.Click += new RoutedEventHandler(uiAutonomousBtn_Click);
            this.uiMainControl.SelectedIndex = 0;
            this.uiReconnectBtn.Click += new RoutedEventHandler(uiReconnectBtn_Click);
            this.uiReconnect.Click += new RoutedEventHandler(uiReconnect_Click);
            this.uiUsersGyde.Click += new RoutedEventHandler(uiUsersGyde_Click);
            this.uiAbout.Click += new RoutedEventHandler(uiAbout_Click);
            this.uiSettings.GetControlsValue += new ControllerSettings.GetControlsValueDelegate(GetControlsValue);
            this.uiSettings.GetPicon2ControlsValue += new ControllerSettings.GetPicon2ControlsValueDelegate(GetControlsValuePicon2);
            this.uiSettings.SetControlsValue += new ControllerSettings.SetValueControlsDelegate(SetValueControls);
            this.uiSettings.SetPicon2ControlsValue += new ControllerSettings.SetPicon2ValueControlsDelegate(SetPicon2ValueControls);
            this.uiSettings.GetPicon2ModuleInfo += new ControllerSettings.GetPicon2ModuleInfoDelegate(GetPicon2ModuleInfo);
            this.uiSettings.ShowMessage += new ControllerSettings.ShowMessageEventHandler(ShowMessage);
            this.uiSettings.IsAutonomous = this._isAutonomous;
            this.uiSettings.Config = this._config;

            this.uiSystemJournal.ClearInProcess += new SystemJournal.ClearInProgressEventHandler(ClearInProgress);
            this.uiSystemJournal.ClearCompleted += new SystemJournal.ClearCompletedEventHandler(ClearCompleted);

            if (_isAutonomous == false)
            {
                RTUConnectionGlobal.OnWritingStartedAction += () =>
                {
                    uiStateIcon.Dispatcher.Invoke(() =>
                    {
                        uiStateIcon.Visibility = Visibility.Visible;
                        uiStatePresenter.Visibility = Visibility.Visible;
                        //uiAutonomusPresenter.Visibility = Visibility.Hidden;
                    });
                };
                RTUConnectionGlobal.OnWritingCompleteAction += () =>
                {
                    try
                    {
                        uiStateIcon.Dispatcher.Invoke(() =>
                        {
                            uiStateIcon.Visibility = Visibility.Hidden;
                            uiStatePresenter.Visibility = Visibility.Hidden;
                            //uiAutonomusPresenter.Visibility = Visibility.Hidden;
                        });
                    }
                    catch (Exception ex) { };
                };
            }
        }



        private async void UiUpdateTimerTriggered()
        {
            try
            {

                if (_semaphoreSlim.CurrentCount == 0)
                {
                    return;
                }
                //await _semaphoreSlim.WaitAsync();
                var isDiagTabSelected = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    isDiagTabSelected = DiagnosticTab.IsSelected;

                });
                if (isDiagTabSelected)
                {
                    await _semaphoreSlim.WaitAsync();
                    if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                    {
                        await uiPicon2DiagnosticsErrors.Update();
                        await uiTime.Update();
                        await uiSignalGSMLevel.Update();
                    }
                    else
                    {
                        await uiPiconDiagnostics.Update();
                        await uiTime.Update();
                        await uiSignalGSMLevel.Update();
                        await uiRuno3Diagnostics.Update();
                        await uiDiagnosticsErrors.Update();
                    }
                    _semaphoreSlim.Release();
                }
                var isLogicTabSelected = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    isLogicTabSelected = LogicTab.IsSelected;
                });
                if (isLogicTabSelected)
                {
                    await _semaphoreSlim.WaitAsync();
                    if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                    {
                        await uiChannelsManagment.Update();
                        await uiErrors.Update();
                        await uiFuseErrors.Update();
                        await uiTurnOnError.Update();
                        await uiStates.Update();
                        await uiMeter.Update();
                    }
                    if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                    {
                        await uiChannelsManagment.Update();
                        await uiErrors.Update();
                        await uiFuseErrors.Update();
                        await uiTurnOnError.Update();
                        await uiStates.Update();
                        await uiMeter.Update();
                    }
                    if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                    {
                        await uiChannelsManagment.Update();
                        await uiErrors.Update();
                        await uiStates.Update();
                        await uiMeter.Update();
                        await uiPicon2ChannelManagement.Update();
                    }
                    _semaphoreSlim.Release();

                }
                var isModuleRequestsTabSelected = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    isModuleRequestsTabSelected = uiPicon2ModuleRequests.IsSelected;

                });
                if (isModuleRequestsTabSelected)
                {
                    await _semaphoreSlim.WaitAsync();
                    await Picon2ModuleRequest.Update();
                    _semaphoreSlim.Release();

                }
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    MrNetwork.IsSelected;

                //});



            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                try
                {
                    //if (_semaphoreSlim.CurrentCount == 0)
                    //{
                    //    _semaphoreSlim.Release(1);
                    //}
                }

                catch (Exception ex)
                {
                    //throw ex;
                }
            }


        }






        private void InitSlots()
        {
            this._connectionChecker = new Slot(0x0400, 52, "Check connection");
            #region Config
            var config = Config.Open();
            if (config == null)
            {
                this._config = new Config();
            }
            else
                this._config = config;
            #endregion

        }

        #region Menu items click
        void uiAbout_Click(object sender, RoutedEventArgs e)
        {
            About ab = new About();
            ab.ShowDialog();
        }

        void uiUsersGyde_Click(object sender, RoutedEventArgs e)
        {
            RunDCOMInfo();
        }

        private void RunDCOMInfo()
        {
            this.RunProcess(new FileInfo(Directory.GetCurrentDirectory() + @"\Minsk2.chm"));
        }

        private bool RunProcess(FileInfo processFileInfo)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo(processFileInfo.FullName);
            procStartInfo.Arguments = string.Empty /*"/f1 \"" + processFileInfo.Directory.FullName + "Setup.iss\" " + "/f2 \"" + processFileInfo.Directory.FullName + "Setup.log\""*/;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            try
            {
                bool startresult = proc.Start();
            }
            catch (Exception)
            {
                //MessageBox.Show("", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            //proc.WaitForExit();
            /*Внимание: заглушка*/
            return true;
        }


        void uiReconnect_Click(object sender, RoutedEventArgs e)
        {
            this.uiReconnectBtn_Click(this, new RoutedEventArgs());

        }

        void uiDisconnect_Click(object sender, RoutedEventArgs e)
        {
            this.uiDisconnectBtn_Click(sender, e);

        }

        //void SetAutonomus()
        //{
        //    var clickMode = uiAutonomousBtn.ClickMode;

        //}
        void uiMenuAutonomus_Click(object sender, RoutedEventArgs e)
        {



            SetAllAutonomous();
            DiagnosticTab.IsSelected = true;
        }

        void uiAutonomous_Click(object sender, RoutedEventArgs e)
        {
            //if (!this._isAutonomous)
            //{
            //    this.Title = this.Title + "(Автономная работа)";
            //    this._isAutonomous = true;
            SetAllAutonomous();
            DiagnosticTab.IsSelected = true;

            //}
        }

        private void uiAutonomousBtn_Click(object sender, RoutedEventArgs e)
        {
            SetAutonomusMode();
        }
        private void uiConnect_Click(object sender, RoutedEventArgs e)
        {
            this.uiConnectBtn_Click(sender, e);
            this.DisableAutonomous();
        }



        private void uiExit_Click(object sender, RoutedEventArgs e)
        {
            _semaphoreSlim.Dispose();

            this.Close();
        }
        #endregion

        #region Buttons click
        void uiReconnectBtn_Click(object sender, RoutedEventArgs e)
        {

            this.uiDisconnect.IsEnabled = true;
            this.uiConnect.IsEnabled = false;
            this._isAutonomous = false;
            this.DisableAutonomous();
            this.Title = this.Title.Replace("(Автономная работа)", "");
            this._isAfterConnectionLost = false;
            this.Start();
        }

        private void uiGSMConnection_Click(object sender, RoutedEventArgs e)
        {
            ResultGSM resultgsm = null;
            GSMConnection ngc = new GSMConnection();
            ngc.Owner = this;

            TcpClient _tcpClient = new TcpClient();

            if (Convert.ToBoolean(ngc.ShowDialog()))
            {

                try
                {
                    resultgsm = ngc.ResulGSM;
                    if (ResultGSM.IPAdress.StartsWith("127"))
                    {
                        MessageBox.Show("Не удалось подключиться по GSM-каналу. Введен неправильный IP-адрес.", "Ошибка");
                        return;
                    }


                    TcpClient client = new TcpClient(ResultGSM.IPAdress, ResultGSM.PortNumber);
                    client.ReceiveTimeout = 10000;
                    client.SendTimeout = 10000;
                    ModbusIpMaster.CreateIp(client);
                    _uiUpdateTimer = new Timer((obj) =>
                    {
                        UiUpdateTimerTriggered();
                    }, null, 5000, 5000);
                    this.Start();
                    this.uiDisconnect.IsEnabled = true;
                    this.uiConnect.IsEnabled = false;
                    RTUConnectionGlobal.Initialize(ModbusIpMaster.CreateIp(client), ResultGSM.ModbusReadTimeout, ResultGSM.ModbusWriteTimeout, ResultGSM.ModbusRetries, ResultGSM.ModbusWaitUntilRetry);




                }

                catch (SocketException ex)

                {
                    //MessageBoxResult res = MessageBox.Show("Не удалось подключиться по GSM-каналу", "Ошибка");
                    MessageBoxResult res = MessageBox.Show(ex.Message + Environment.NewLine +
                                                            "Ошибка сокета: " + ex.SocketErrorCode.ToString() + Environment.NewLine +
                                                            "Код ошибки: " + ex.ErrorCode.ToString(), "Ошибка");

                }


            }

        }

        private void uiConnectBtn_Click(object sender, RoutedEventArgs e)
        {

            CreateConnectionWizard.Result result = null;
            CreateConnectionWizard ccw = new CreateConnectionWizard();
            ccw.Owner = this;
            if (Convert.ToBoolean(ccw.ShowDialog()))
            {
                result = ccw.ResultDialog;

                IStreamResource streamResource = Connector.GetSerialPortAdapter(result.PortName, result.DeviceNumber, result.PortSpeed, result.Timeout);

                IModbusMaster modbusSerialMaster = ModbusSerialMaster.CreateRtu(streamResource);

                RTUConnectionGlobal.Initialize(modbusSerialMaster, result.ModbusReadTimeout, result.ModbusWriteTimeout, result.ModbusRetries, result.ModbusWaitUntilRetry);

                // DataTransfer.InitConnector(new Connector(result.PortName, result.KNNumber, result.DeviceNumber,
                //    result.PortSpeed, result.Timeout));
                //todo: test timer trigger
                try
                {
                    _uiUpdateTimer = new Timer((obj) =>
                    {
                        UiUpdateTimerTriggered();
                    }, null, 500, 500);
                }
                catch (Exception ex)
                {
                    throw;
                }


                this.uiHider.Visibility = Visibility.Hidden;
                this.Start();
                this.uiDisconnect.IsEnabled = true;
                this.uiConnect.IsEnabled = false;
                this.uiAutonomous.IsEnabled = true;
                AutonomusCheck = 0;
                uiAutonomusPresenter.Visibility = Visibility.Hidden;
            }

        }
        private void uiDisconnectBtn_Click(object sender, RoutedEventArgs e)
        {

            var modbusMemoryViewModel = uiMrNetwork.DataContext as ModbusMemoryViewModel;
            if (modbusMemoryViewModel != null)
                modbusMemoryViewModel.IsQueriesStarted = false;

            this.Stop();
            if (!this._isAfterConnectionLost)
                DataTransfer.UnInit();
            this.SetAllDisable();
            this.uiConnect.IsEnabled = true;
            this.uiDisconnect.IsEnabled = false;
            //this.uiStateIcon.Visibility = Visibility.Hidden;
            //this.uiStatePresenter.Text = "";
            this.uiHider.Visibility = Visibility.Visible;
            this.uiConnectBtn.Visibility = System.Windows.Visibility.Visible;
            this.uiAutonomousBtn.Visibility = System.Windows.Visibility.Visible;
            this.uiDisconnectBtn.Visibility = System.Windows.Visibility.Collapsed;

            var tabItem = this.uiMainControl.Items[0] as TabItem;
            if (tabItem != null)
                tabItem.Visibility = Visibility.Visible;
            this.uiMainControl.SelectedIndex = 0;
            for (int i = 1; i < this.uiMainControl.Items.Count; i++)
            {
                var item = this.uiMainControl.Items[i] as TabItem;
                if (item != null)
                    item.IsEnabled = false;
            }
            this.uiConnect.IsEnabled = true;
            this.uiAutonomous.IsEnabled = true;
            this.uiDisconnect.IsEnabled = true;
        }
        #endregion

        #region Diagnostic

        private Settings GetControlsValue()
        {


            return new Settings(this.uiLogicConfig.Value, this.uiLightingSchedule.Value, this.uiBacklightSchedule.Value,
           this.uiIlluminationSchedule.Value, this.uiEnergySchedule.Value, this.uiHeatingSchedule.Value,
           this.uiGPRSConfig.Value);


        }
        private Picon2Settings GetControlsValuePicon2()
        {


            return new Picon2Settings(this.uiPicon2ConfigurationView.GetConfig(),
                this.picon2LightingSheduleView.GetSchedule("График 1"),
                this.picon2LightingSheduleView.GetSchedule("График 2"),
                this.picon2LightingSheduleView.GetSchedule("График 3"));


        }

        private void SetValueControls(Settings settings)
        {
            this.uiLogicConfig.Value = settings.LogicConfig;
            this.uiLightingSchedule.Value = settings.LightSchedule;
            this.uiBacklightSchedule.Value = settings.BacklightSchedule;
            this.uiIlluminationSchedule.Value = settings.IlluminationSchedule;
            this.uiEnergySchedule.Value = settings.ConversationEnergy;
            this.uiHeatingSchedule.Value = settings.Heating;
            this.uiGPRSConfig.Value = settings.GPRS;

        }

        private void SetPicon2ValueControls(Picon2Settings settings)
        {
            this.uiPicon2ConfigurationView.SetConfig(settings.Picon2Config);
            this.picon2LightingSheduleView.SetScheduleToCache("График 1", settings.LightningSchedule);
            this.picon2LightingSheduleView.SetScheduleToCache("График 2", settings.IlluminationSchedule);
            this.picon2LightingSheduleView.SetScheduleToCache("График 3", settings.BacklightSchedule);

        }
        private void GetPicon2ModuleInfo()
        {
            this.uiSettings.uiPicon2ModuleInfo.IsEnabled = false;
            this.uiSettings.uiPicon2ModuleInfo.Content = "Ожидайте";

            TryReadPicon2ModuleInfo();
        }
        private void ClearInProgress()
        {
            this.Cursor = Cursors.Wait;
        }
        private void ClearCompleted()
        {
            this.Cursor = Cursors.Arrow;
        }

        #endregion

        #region Config
        private object uiLogicConfig_OpenFromFile(Type type)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Файл конфигурации логики|*.lc";
            ofd.Title = "Открытие файла конфигурации логики";
            ofd.InitialDirectory = this._config.LogicConfigInitialPath;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextReader reader = null;
                try
                {
                    object result = null;
                    XmlSerializer s = new XmlSerializer(type);
                    reader = new StreamReader(ofd.FileName);
                    result = s.Deserialize(reader);
                    reader.Close();
                    System.Windows.MessageBox.Show("Открытие конфигурации логики прошло успешно.",
                        "Открытие конфигураци логики", MessageBoxButton.OK, MessageBoxImage.Information);
                    this._config.LogicConfigInitialPath = ofd.FileName;
                    return result;
                }
                catch (Exception)
                {
                    if (reader != null)
                        reader.Close();
                    System.Windows.MessageBox.Show("Во время открытия конфигурации логиги произошла ошибка.",
                        "Открытие конфигурации логики", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void uiLogicConfig_SaveInFile(object value, Type type)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Файл конфигурации логики|*.lc";
            sfd.Title = "Сохранение конфигурации логики в файл";
            sfd.FileName = "Кофигурация_логики1";
            sfd.InitialDirectory = this._config.LogicConfigInitialPath;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextWriter writer = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    writer = new StreamWriter(sfd.FileName);
                    serializer.Serialize(writer, value);
                    writer.Close();
                    System.Windows.MessageBox.Show("Сохранение конфигурации логики прошло успешно.",
                        "Сохранение конфигурации логики", MessageBoxButton.OK, MessageBoxImage.Information);
                    this._config.LogicConfigInitialPath = sfd.FileName;
                }
                catch (Exception e)
                {
                    if (writer != null)
                        writer.Close();
                    System.Windows.MessageBox.Show("Во время сохранения кофигурации логики произошла ошибка.",
                        "Сохранение конфигурации логики", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion Config

        #region Shedules

        #region Lighting shedule
        private void uiLightingSchedule_SaveInFile(object value, Type type)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.InitialDirectory = this._config.ScheduleInitialPath;
            sfd.Filter = "Файл графика|*.schld";
            sfd.Title = "Сохранение графика в файл";
            sfd.FileName = "График1";
            if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            TextWriter writer = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                writer = new StreamWriter(sfd.FileName);
                serializer.Serialize(writer, value);
                writer.Close();
                System.Windows.MessageBox.Show("Сохранение графика прошло успешно.",
                    "Сохранение графика", MessageBoxButton.OK, MessageBoxImage.Information);
                this._config.ScheduleInitialPath = new FileInfo(sfd.FileName).Directory.FullName;
            }
            catch (Exception e)
            {
                if (writer != null)
                    writer.Close();
                System.Windows.MessageBox.Show("Во время сохранения графика произошла ошибка.",
                    "Сохранение графика", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private object uiLightingSchedule_OpenFromFile(Type type)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Файл графика|*.schld";
            ofd.Title = "Открытие файла графика";
            ofd.InitialDirectory = this._config.ScheduleInitialPath;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextReader reader = null;
                try
                {
                    object result = null;
                    XmlSerializer s = new XmlSerializer(type);
                    reader = new StreamReader(ofd.FileName);
                    result = s.Deserialize(reader);
                    reader.Close();
                    System.Windows.MessageBox.Show("Открытие графика прошло успешно.",
                        "Открытие графика", MessageBoxButton.OK, MessageBoxImage.Information);
                    this._config.ScheduleInitialPath = ofd.FileName;
                    return result;
                }
                catch (Exception e)
                {
                    if (reader != null)
                        reader.Close();
                    System.Windows.MessageBox.Show("Во время открытия графика произошла ошибка.",
                        "Открытие графика", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion Lighting shedule

        #region Energy shedule
        private void uiEnergySchedul_SaveInFile(object value, Type type)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.InitialDirectory = this._config.ScheduleInitialPath;
            sfd.Filter = "Файл графика энергосбережения|*.eschld";
            sfd.Title = "Сохранение графика в файл";
            sfd.FileName = "График1";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextWriter writer = null;
                try
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    writer = new StreamWriter(sfd.FileName);
                    serializer.Serialize(writer, value);
                    writer.Close();
                    System.Windows.MessageBox.Show("Сохранение графика прошло успешно.",
                        "Сохранение графика", MessageBoxButton.OK, MessageBoxImage.Information);
                    this._config.ScheduleInitialPath = new FileInfo(sfd.FileName).Directory.FullName;
                }
                catch (Exception e)
                {
                    if (writer != null)
                        writer.Close();
                    System.Windows.MessageBox.Show("Во время сохранения графика произошла ошибка.",
                        "Сохранение графика", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private object uiEnergySchedul_OpenFromFile(Type type)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Файл графика энергосбережения|*.eschld";
            ofd.Title = "Открытие файла графика";
            ofd.InitialDirectory = this._config.ScheduleInitialPath;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextReader reader = null;
                try
                {
                    object result = null;
                    XmlSerializer s = new XmlSerializer(type);
                    reader = new StreamReader(ofd.FileName);
                    result = s.Deserialize(reader);
                    reader.Close();
                    System.Windows.MessageBox.Show("Открытие графика прошло успешно.",
                        "Открытие графика", MessageBoxButton.OK, MessageBoxImage.Information);
                    this._config.ScheduleInitialPath = ofd.FileName;
                    return result;
                }
                catch (Exception e)
                {
                    if (reader != null)
                        reader.Close();
                    System.Windows.MessageBox.Show("Во время открытия графика произошла ошибка.",
                        "Открытие графика", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            else
            {
                return null;

            }
        }
        #endregion Energy

        #endregion Shedules

        #region Autonomous mode
        public void SetAllAutonomous()
        {
            if (AutonomusCheck == 0)
            {
                AutonomusCheck++;
                
                if (_uiUpdateTimer != null)
                {
                    _uiUpdateTimer.Dispose();
                    _semaphoreSlim.Dispose();
                }
                RTUConnectionGlobal.CloseConnection();
                if (MessageBox.Show("Связь с устройством потеряна. Перейти в автономный режим?", "Внимание!", MessageBoxButton.YesNo,
                        MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    SetAutonomusMode();
                    isAutonomus = true;
                }
                else
                {

                    this.Close();

                }
            }
            else
            {
                return;
            }
        }

        public void SetAutonomusMode()
        {
            uiConnect.IsEnabled = true;
            uiAutonomusPresenter.Visibility = Visibility.Visible;
            try
            {
                this.uiAutonomousBtn.Visibility = System.Windows.Visibility.Visible;
                if (!this._isAfterConnectionLost)
                {
                    this.uiReconnectBtn.Visibility = System.Windows.Visibility.Collapsed;
                    this.uiReconnect.IsEnabled = false;
                }
                else
                {
                    this.uiReconnectBtn.Visibility = System.Windows.Visibility.Visible;
                    this.uiReconnect.IsEnabled = true;
                }
                this.uiAutonomous.IsEnabled = false;
                this.uiDisconnect.IsEnabled = false;

                var tabItem = this.uiMainControl.Items[0] as TabItem;
                if (tabItem != null) { tabItem.Visibility = Visibility.Visible; }
                this.uiMainControl.SelectedIndex = 0;
                for (int i = 1; i < this.uiMainControl.Items.Count; i++)
                {
                    var item = this.uiMainControl.Items[i] as TabItem;
                    if (item != null)
                        item.IsEnabled = true;
                }
                DiagnosticTab.IsSelected = true;
                //uiMainControl.Visibility = Visibility.Hidden; 
                uiHider.Visibility = Visibility.Visible;
                if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
                {
                    uiRuno3Diagnostics.Visibility = Visibility.Visible;
                    uiScroll.Visibility = Visibility.Visible;
                    uiScrollViewer.Visibility = Visibility.Hidden;
                    uiDiagnosticsErrors.Visibility = Visibility.Hidden;
                    uiPiconDiagnostics.Visibility = Visibility.Hidden;
                    uiScrollViewerPicon2.Visibility = Visibility.Hidden;
                    uiPicon2Diagnostics.Visibility = Visibility.Hidden;
                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
                {
                    uiRuno3Diagnostics.Visibility = Visibility.Hidden;
                    uiScroll.Visibility = Visibility.Hidden;
                    uiScrollViewer.Visibility = Visibility.Visible;
                    uiDiagnosticsErrors.Visibility = Visibility.Visible;
                    uiPiconDiagnostics.Visibility = Visibility.Visible;
                    uiScrollViewerPicon2.Visibility = Visibility.Hidden;
                    uiPicon2Diagnostics.Visibility = Visibility.Hidden;

                }
                else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
                {

                    uiRuno3Diagnostics.Visibility = Visibility.Hidden;
                    uiScroll.Visibility = Visibility.Hidden;
                    uiScrollViewer.Visibility = Visibility.Hidden;
                    uiDiagnosticsErrors.Visibility = Visibility.Hidden;
                    uiPiconDiagnostics.Visibility = Visibility.Hidden;
                    uiPicon2DiagnosticsErrors.Visibility = Visibility.Visible;
                    uiScrollViewerPicon2.Visibility = Visibility.Visible;
                    uiPicon2Diagnostics.Visibility = Visibility.Visible;
                    //picon2LightingSheduleView.uiReadPicon2Schdule.IsEnabled = true;
                    //picon2LightingSheduleView.uiWritePicon2Schdule.IsEnabled = true;


                }
                //this.uiTime.SetAutonomous();

                this.uiSettings.IsAutonomous = _isAutonomous;
                this.uiTime.SetAutonomus();
                this.uiPiconDiagnostics.SetAutonomus();
                this.uiPicon2DiagnosticsErrors.SetAutonomus();
                this.uiRuno3Diagnostics.SetAutonomus();
                this.uiSystemJournal.SetAutonomous();
                this.uiLogicConfig.SetAutonomous();
                this.uiLightingSchedule.SetAutonomous();
                this.uiHeatingSchedule.SetAutonomous();
                this.uiGPRSConfig.SetAutonomous();
                this.uiIlluminationSchedule.SetAutonomous();
                this.uiBacklightSchedule.SetAutonomous();
                this.uiChannelsManagment.SetAutonomous();
                this.uiEnergySchedule.SetAutonomous();
                this.uiDiagnosticsErrors.SetAutonomus();
                this.uiSignalGSMLevel.SetAutonomus();
                this.uiSettings.SetAutonomus();

                this.Picon2ModuleRequest.SetAutonomus();
                this.picon2LightingSheduleView.SetAutonomus();
                this.uiPicon2ConfigurationView.SetAutonomus();
                //Dispatcher.CurrentDispatcher.InvokeShutdown();
                //this.uiStatePresenter.Text = "Автономный режим";

                this.UpdateLayout();
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
            }
        }

        private void DisableAutonomous()
        {
            this.uiConnectBtn.Visibility = System.Windows.Visibility.Collapsed;
            this.uiDisconnectBtn.Visibility = System.Windows.Visibility.Visible;
            this.uiAutonomousBtn.Visibility = System.Windows.Visibility.Collapsed;
            this.uiReconnectBtn.Visibility = System.Windows.Visibility.Visible;
            this.uiAutonomusPresenter.Visibility = Visibility.Hidden;
            this.uiStatePresenter.Visibility = Visibility.Visible;
            this.uiReconnect.IsEnabled = false;
            this.uiDisconnect.IsEnabled = true;
            this.uiConnect.IsEnabled = false;
            this.uiAutonomous.IsEnabled = false;

            this.uiMainControl.SelectedIndex = 1;
            for (int i = 1; i < this.uiMainControl.Items.Count; i++)
            {
                (this.uiMainControl.Items[i] as TabItem).IsEnabled = false;
            }
            //this.uiTime.DisableAutonomous();
            this.uiSettings.IsAutonomous = _isAutonomous;
            this.uiSystemJournal.DisableAutonomous();
            this.uiLogicConfig.DisableAutonomous();
            this.uiLightingSchedule.DisableAutonomous();
            this.uiHeatingSchedule.DisableAutonomous();
            this.uiGPRSConfig.DisableAutonomous();
            this.uiIlluminationSchedule.DisableAutonomous();
            this.uiEnergySchedule.DisableAutonomous();
            this.uiBacklightSchedule.DisableAutonomous();
            this.uiChannelsManagment.DisableAutonomus();

            this.uiSettings.DisableAutonomus();

            //todo:disableautonomus
            this.uiPicon2ConfigurationView.DisableAutonomus();
            this.Picon2ModuleRequest.DisableAutonomus();
            this.picon2LightingSheduleView.DisableAutonomus();

        }
        #endregion

        #region Common
        #region Thread managment
        public void Start()
        {
            uiSettings.uiPLCReset.IsEnabled = true;
            uiSettings.uiSignature.IsEnabled = true;
            uiSettings.uiWriteAll.IsEnabled = true;
            uiSettings.uiReadAll.IsEnabled = true;
            uiSystemJournal.uiImport.IsEnabled = true;
            uiSystemJournal.uiClear.IsEnabled = true;
            uiLightingSchedule.uiClearAll.IsEnabled = true;
            uiLightingSchedule.uiExport.IsEnabled = true;
            uiLightingSchedule.uiImport.IsEnabled = true;
            uiBacklightSchedule.uiClearAll.IsEnabled = true;
            uiBacklightSchedule.uiExport.IsEnabled = true;
            uiBacklightSchedule.uiImport.IsEnabled = true;
            uiIlluminationSchedule.uiClearAll.IsEnabled = true;
            uiIlluminationSchedule.uiExport.IsEnabled = true;
            uiIlluminationSchedule.uiImport.IsEnabled = true;
            uiEnergySchedule.uiClearAll.IsEnabled = true;
            uiEnergySchedule.uiExport.IsEnabled = true;
            uiEnergySchedule.uiImport.IsEnabled = true;
            uiHeatingSchedule.uiExport.IsEnabled = true;
            uiHeatingSchedule.uiImport.IsEnabled = true;
            uiLogicConfig.uiLogicExport.IsEnabled = true;
            uiLogicConfig.uiLogicImport.IsEnabled = true;
            uiGPRSConfig.uiExport.IsEnabled = true;
            uiGPRSConfig.uiImport.IsEnabled = true;

            this._shutDownEvent.Reset();

            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
            {
                uiRuno3Diagnostics.Visibility = Visibility.Visible;
                uiScroll.Visibility = Visibility.Visible;
                uiScrollViewer.Visibility = Visibility.Hidden;
                uiDiagnosticsErrors.Visibility = Visibility.Hidden;
                uiPiconDiagnostics.Visibility = Visibility.Hidden;

                //uiDiscretScroll.Visibility = Visibility.Hidden;
            }
            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
            {
                uiRuno3Diagnostics.Visibility = Visibility.Hidden;
                uiScroll.Visibility = Visibility.Hidden;
                uiScrollViewer.Visibility = Visibility.Visible;
                uiDiagnosticsErrors.Visibility = Visibility.Visible;
                uiPiconDiagnostics.Visibility = Visibility.Visible;


            }
            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                //uiRuno3Diagnostics.Visibility = Visibility.Hidden;
                //uiScroll.Visibility = Visibility.Hidden;
                //uiScrollViewer.Visibility = Visibility.Visible;
                uiPicon2Diagnostics.Visibility = Visibility.Visible;
                uiPicon2DiagnosticsErrors.Visibility = Visibility.Visible;

            }
            (this.uiMainControl.Items[0] as TabItem).Visibility = Visibility.Collapsed;
            this.uiMainControl.SelectedIndex = 1;
            for (int i = 1; i < this.uiMainControl.Items.Count; i++)
            {
                (this.uiMainControl.Items[i] as TabItem).IsEnabled = true;
            }
        }

        public void Stop()
        {
            this._shutDownEvent.Set();
            if (_work != null) this._work.Abort();
            this._work = null;
            _semaphoreSlim.Dispose();
            RTUConnectionGlobal.CloseConnection();

            DataTransfer.UnInit();
        }

        #endregion Thread

        private void ShowMessage(string message, string caption, MessageBoxImage image)
        {
            System.Windows.MessageBox.Show(message, caption, MessageBoxButton.OK, image);
        }
        private void SetAllDisable()
        {
            this.uiTime.Value = null;
            this.uiErrors.Value = null;
            this.uiMeter.Value = null;
            this.uiStates.Value = null;

            this.uiChannelsManagment.Value = null;
        }
        private void ControlStopWork()
        {
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void ControlStartWork()
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._config.Save();
            this.uiDisconnectBtn_Click(this, new RoutedEventArgs());
            _semaphoreSlim.Dispose();

            RTUConnectionGlobal.CloseConnection();
        }
        #endregion Common

        private void uiDeviceSelection_Click(object sender, RoutedEventArgs e)
        {
            _semaphoreSlim.Dispose();
            RTUConnectionGlobal.CloseConnection();
            this.Close();
        }

        private void UiMainControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (MrNetwork.IsSelected)
                {
                    var modbusMemoryViewModel = uiMrNetwork.DataContext as ModbusMemoryViewModel;
                    if (modbusMemoryViewModel != null)
                        modbusMemoryViewModel.IsQueriesAllowed = true;
                }
                else
                {
                    var modbusMemoryViewModel = uiMrNetwork.DataContext as ModbusMemoryViewModel;
                    if (modbusMemoryViewModel != null)
                        modbusMemoryViewModel.IsQueriesAllowed = false;
                }
            }
            catch
            {

            }

        }

        private async void TryReadPicon2ModuleInfo()
        {
            try
            {
                ushort[] ConnectionModuleId;
                {
                    ConnectionModuleId = await RTUConnectionGlobal.GetDataByAddress(1, 0x3004, 1);
                }
                string ModuleFirmwareVersion = null;
                string ModemVersion = null;
                string ModemFirmwareVersion = null;
                string ModemIMEI = null;
                if ((byte)ConnectionModuleId[0] != 0xE0 && (byte)ConnectionModuleId[0] != 0xE1)
                {
                    ShowMessage("Неверный модуль связи", "Ошибка", MessageBoxImage.Error);
                    this.uiSettings.uiPicon2ModuleInfo.IsEnabled = true;
                    this.uiSettings.uiPicon2ModuleInfo.Content = "Инф. по модулю связи";
                    return;
                }
                var data = await RTUConnectionGlobal.ExecuteFunction12Async(
                       (byte)ConnectionModuleId[0], "GetModuleFirmwareVersion", 0xF0);
                if (data != null)
                {
                    ModuleFirmwareVersion = Encoding.UTF8.GetString(data);
                }
                data = await RTUConnectionGlobal.ExecuteFunction12Async(
                    (byte)ConnectionModuleId[0], "GetModemVersion", 0xF1);
                if (data != null)
                {
                    ModemVersion = Encoding.UTF8.GetString(data);
                }
                data = await RTUConnectionGlobal.ExecuteFunction12Async(
                    (byte)ConnectionModuleId[0], "GetModemFirmwareVersion", 0xF2);
                if (data != null)
                {
                    ModemFirmwareVersion = Encoding.UTF8.GetString(data);
                }
                data = await RTUConnectionGlobal.ExecuteFunction12Async(
                    (byte)ConnectionModuleId[0], "GetModemIMEI", 0xF3);
                if (data != null)
                {
                    ModemIMEI = Encoding.UTF8.GetString(data);
                }
                ShowPicon2ModuleInfo(ModuleFirmwareVersion, ModemVersion, ModemFirmwareVersion, ModemIMEI);
            }
            catch (Exception exception)
            {

            }
            return;
        }
        private void ShowPicon2ModuleInfo(string moduleFirmwareVersion, string modemVersion, string modemFirmwareVersion, string modemIMEI)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (moduleFirmwareVersion.Length < 3) { sb.AppendLine("Версия прошивки модуля: нет данных"); }
                else { sb.AppendLine("Версия прошивки модуля: " + moduleFirmwareVersion); }
                if (modemVersion.Length < 3) { sb.AppendLine("Модель модема: нет данных"); }
                else { sb.AppendLine("Модель модема: " + modemVersion); }
                if (modemFirmwareVersion.Length < 3) { sb.AppendLine("Версия прошивки модема: нет данных"); }
                else { sb.AppendLine("Версия прошивки модема: " + modemFirmwareVersion); }
                if (modemIMEI.Length < 3) { sb.AppendLine("IMEI модема: нет данных"); }
                else { sb.AppendLine("IMEI модема: " + modemIMEI); }
            }
            catch (Exception ex)
            {

            }
            ShowMessage(sb.ToString(), "Информация по модулю связи", MessageBoxImage.Information);

            this.uiSettings.uiPicon2ModuleInfo.IsEnabled = true;
            this.uiSettings.uiPicon2ModuleInfo.Content = "Инф. по модулю связи";

        }


    }
}