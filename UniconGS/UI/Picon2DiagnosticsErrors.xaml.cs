using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using UniconGS.Source;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;
using UniconGS.Interfaces;
using System.Threading;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for DiagnosticsErrors.xaml
    /// </summary>
    public partial class Picon2DiagnosticsErrors : UserControl, IUpdatableControl
    {
        #region Globals

        private ushort[] _value;
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        private delegate void ReadComplete(ushort[] res);

        #endregion

        public Picon2DiagnosticsErrors()
        {
            InitializeComponent();
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
        }

        private void DisableAll()
        {
            this.uiProgramCodeFail.Value =
                this.uiClockFail.Value =
                    this.uiPPZUFail.Value =
                        this.uiPowerFail.Value =
                            this.uiModuleFail.Value =
                                this.uiModuleRequestFail.Value =
                                    this.uiSlaveConnectionFail.Value =
                                        this.uiSlaveRequestFail.Value =
                                            this.uiLogicFail.Value = null;
            this.SetInfo();
        }

        private void SetIndicators(BitArray value)
        {
            //this.uiModulesFail.Value = value[6]; // - ошибка модулей
            //this.uiModule1Fail.Value = value[7]; // - реле
            //this.uiModule2Fail.Value = value[8]; // - дискрет 1
            //this.uiModule3Fail.Value = value[9]; // - дискрет 2 
            //this.uiModule4Fail.Value = value[10]; // - дискрет 3
            //this.uiModule5Fail.Value = value[11]; // - дискрет 4 
            this.uiProgramCodeFail.Value = value[0];
            this.uiClockFail.Value = value[1];
            this.uiPPZUFail.Value = value[2];
            this.uiPowerFail.Value = value[3];
            this.uiModuleFail.Value = value[4];
            this.uiModuleRequestFail.Value = value[5];
            this.uiSlaveConnectionFail.Value = value[6];
            this.uiSlaveRequestFail.Value = value[7];
            this.uiLogicFail.Value = value[8];

            this.SetInfo();

        }

        private void SetInfo()
        {
            #region [Program code]

            if (this.uiProgramCodeFail.Value == null)
            {
                this.uiProgramCodeFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiProgramCodeFail.Value))
            {
                this.uiProgramCodeFail.ToolTip = "Программа неисправна";
            }
            else
            {
                this.uiProgramCodeFail.ToolTip = "Программа работает нормально";
            }

            #endregion

            #region [Clock fail]

            if (this.uiClockFail.Value == null)
            {
                this.uiClockFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiClockFail.Value))
            {
                this.uiClockFail.ToolTip = "Часы неисправны";
            }
            else
            {
                this.uiClockFail.ToolTip = "Часы работают нормально";
            }

            #endregion

            #region [PPZU fail]

            if (this.uiPPZUFail.Value == null)
            {
                this.uiPPZUFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiPPZUFail.Value))
            {
                this.uiPPZUFail.ToolTip = "Ошибка в работе ППЗУ";
            }
            else
            {
                this.uiPPZUFail.ToolTip = "ППЗУ работает нормально";
            }

            #endregion

            #region [Power fail]

            if (this.uiPowerFail.Value == null)
            {
                this.uiPowerFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiPowerFail.Value))
            {
                this.uiPowerFail.ToolTip = "Ошибка питания устройства";
            }
            else
            {
                this.uiPowerFail.ToolTip = "Питание работает нормально";
            }

            #endregion

            #region [Module fail]

            if (this.uiModuleFail.Value == null)
            {
                this.uiModuleFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiModuleFail.Value))
            {
                this.uiModuleFail.ToolTip = "Ошибка модулей";
            }
            else
            {
                this.uiModuleFail.ToolTip = "Модули работают нормально";
            }

            #endregion

            #region [Module request fail]

            if (this.uiModuleRequestFail.Value == null)
            {
                this.uiModuleRequestFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiModuleRequestFail.Value))
            {
                this.uiModuleRequestFail.ToolTip = "Ошибка запросов к модулям";
            }
            else
            {
                this.uiModuleRequestFail.ToolTip = "Запросы к модулям работают нормально";
            }

            #endregion

            #region [Slave connection fail]

            if (this.uiSlaveConnectionFail.Value == null)
            {
                this.uiSlaveConnectionFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiSlaveConnectionFail.Value))
            {
                this.uiSlaveConnectionFail.ToolTip = "Ошибка связи с подчиненным модулем";
            }
            else
            {
                this.uiSlaveConnectionFail.ToolTip = "Связь с подчиненным работает нормально";
            }

            #endregion

            #region [Slave request fail]

            if (this.uiSlaveRequestFail.Value == null)
            {
                this.uiSlaveRequestFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiSlaveRequestFail.Value))
            {
                this.uiSlaveRequestFail.ToolTip = "Ошибка запросов к подчиненному модулю";
            }
            else
            {
                this.uiSlaveRequestFail.ToolTip = "Запросы к подчиненному модулю работают нормально";
            }

            #endregion

            #region [Logic prog fail]

            if (this.uiLogicFail.Value == null)
            {
                this.uiLogicFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiLogicFail.Value))
            {
                this.uiLogicFail.ToolTip = "Ошибка логической программы";
            }
            else
            {
                this.uiLogicFail.ToolTip = "Логическая программа работает нормально";
            }

            #endregion



        }


        #region IQueryMember

        public ushort[] Value
        {
            get { return this._value; }
            set
            {
                if (value != null)
                {
                    this._value = value;
                    this.SetIndicators(Converter.GetBitsFromWords(value));
                }
                else
                {
                    this.DisableAll();
                }
            }
        }

        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();
            try
            {
                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x1100, 1);
                if (value == null)
                    return;
                BitArray d = new BitArray(new int[] { value[0] });
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetIndicators(d);
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
            DisableAll();
        }


        #endregion
    }
}
