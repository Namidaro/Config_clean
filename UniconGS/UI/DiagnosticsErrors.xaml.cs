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
    public partial class DiagnosticsErrors : UserControl, IUpdatableControl
    {
        #region Globals

        private ushort[] _value;
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        private delegate void ReadComplete(ushort[] res);

        #endregion

        public DiagnosticsErrors()
        {
            InitializeComponent();
        }

        private void DisableAll()
        {
            this.uiModulesFail.Value =
                this.uiModule1Fail.Value =
                    this.uiModule2Fail.Value =
                        this.uiModule3Fail.Value =
                            this.uiModule4Fail.Value =
                                this.uiModule5Fail.Value = null;
            this.SetInfo();
        }

        private void SetIndicators(BitArray value)
        {
            this.uiModulesFail.Value = value[6]; // - ошибка модулей
            this.uiModule1Fail.Value = value[7]; // - реле
            this.uiModule2Fail.Value = value[8]; // - дискрет 1
            this.uiModule3Fail.Value = value[9]; // - дискрет 2 
            this.uiModule4Fail.Value = value[10]; // - дискрет 3
            this.uiModule5Fail.Value = value[11]; // - дискрет 4 
            this.SetInfo();

        }

        private void SetInfo()
        {
            #region Modules errors info

            if (this.uiModulesFail.Value == null)
            {
                this.uiModulesFail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiModulesFail.Value))
            {
                this.uiModulesFail.ToolTip = "Ошибка модулей";
            }
            else
            {
                this.uiModulesFail.ToolTip = "Модули работают нормально";
            }

            #endregion

            #region Rele module info

            if (this.uiModule1Fail.Value == null)
            {
                this.uiModule1Fail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiModule1Fail.Value))
            {
                this.uiModule1Fail.ToolTip = "Ошибка в работе модуля реле";
            }
            else
            {
                this.uiModule1Fail.ToolTip = "Модуль реле работает нормально";
            }

            #endregion

            #region First discret module info

            if (this.uiModule2Fail.Value == null)
            {
                this.uiModule2Fail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiModule2Fail.Value))
            {
                this.uiModule2Fail.ToolTip = "Ошибка в работе первого дискретного модуля";
            }
            else
            {
                this.uiModule2Fail.ToolTip = "Первый дискретный модуль работает нормально";
            }

            #endregion

            #region Second discret module info

            if (this.uiModule3Fail.Value == null)
            {
                this.uiModule3Fail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiModule3Fail.Value))
            {
                this.uiModule3Fail.ToolTip = "Ошибка в работе второго дискретного модуля";
            }
            else
            {
                this.uiModule3Fail.ToolTip = "Второй дискретный модуль работает нормально";
            }

            #endregion

            #region Third discret module info

            if (this.uiModule4Fail.Value == null)
            {
                this.uiModule4Fail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiModule4Fail.Value))
            {
                this.uiModule4Fail.ToolTip = "Ошибка в работе третьего дискретного модуля";
            }
            else
            {
                this.uiModule4Fail.ToolTip = "Третий дискретный модуль работает нормально";
            }

            #endregion

            #region Third discret module info

            if (this.uiModule5Fail.Value == null)
            {
                this.uiModule5Fail.ToolTip = "Состояние не известно";
            }
            else if (Convert.ToBoolean(this.uiModule5Fail.Value))
            {
                this.uiModule5Fail.ToolTip = "Ошибка в работе четвертого дискретного модуля";
            }
            else
            {
                this.uiModule5Fail.ToolTip = "Четвертый дискретный модуль работает нормально";
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
            ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0300, 1);
            BitArray d = new BitArray(new int[] {value[0]});
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetIndicators(d);
            });
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
