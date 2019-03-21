using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using UniconGS.UI;
using UniconGS.Source;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using UniconGS.Annotations;
using UniconGS.Interfaces;
using UniconGS.UI.Configuration;
using UniconGS.UI.GPRS;
using UniconGS.UI.HeatingSchedule;
using UniconGS.UI.Journal;
using UniconGS.UI.Schedule;
using UniconGS.UI.Settings;
using UniconGS.UI.Time;
using UniconGS.Enums;
using UniconGS.UI.Picon2;
using TabControl = System.Windows.Controls.TabControl;

namespace UniconGS.UI.DiscretModules
{
    /// <summary>
    /// Interaction logic for AllStates.xaml
    /// </summary>
    public partial class AllStates : UserControl, IUpdatableControl
    {
        #region Globals
        private ushort[] _value;
        private delegate void ReadComplete(ushort[] res);
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        #endregion

        public AllStates()
        {
            InitializeComponent();
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
            //todo: переделеать это дерьмо на binding'и
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_RUNO)
            {
                uiGroupBox1.Visibility = Visibility.Visible;
                uiGroupBox3.Visibility = Visibility.Collapsed;
                uiGroupBox4.Visibility = Visibility.Collapsed;
                uiGroupBox2.Visibility = Visibility.Collapsed;

            }
            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON_GS)
            {
                uiGroupBox1.Visibility = Visibility.Visible;
                uiGroupBox3.Visibility = Visibility.Visible;
                uiGroupBox4.Visibility = Visibility.Visible;
                uiGroupBox2.Visibility = Visibility.Visible;
            }
            else if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                uiGroupBox1.Visibility = Visibility.Visible;
                uiGroupBox3.Visibility = Visibility.Visible;
                uiGroupBox4.Visibility = Visibility.Visible;
                uiGroupBox2.Visibility = Visibility.Visible;

                uiState1.Visibility = Visibility.Collapsed;
                uiState2.Visibility = Visibility.Collapsed;
                uiState3.Visibility = Visibility.Collapsed;
                uiState4.Visibility = Visibility.Collapsed;

                uiPicon2State1.Visibility = Visibility.Visible;
                uiPicon2State2.Visibility = Visibility.Visible;
                uiPicon2State3.Visibility = Visibility.Visible;
                uiPicon2State4.Visibility = Visibility.Visible;

            }
        }




        #region Privates
        private void SetDefault()
        {
            this.uiState1.Value = null;
            this.uiState2.Value = null;
            this.uiState3.Value = null;
            this.uiState4.Value = null;
        }

        private void SetStates(ushort[] value)
        {
            /*Т.к. дискретных модулей 4*/
            this.uiState1.Value = new ushort[] { value[0], value[1] };
            this.uiState2.Value = new ushort[] { value[2], value[3] };
            this.uiState3.Value = new ushort[] { value[4], value[5] };
            this.uiState4.Value = new ushort[] { value[6], value[7] };
        }
        private void SetPicon2States(ushort[] value)
        {
            /*Т.к. дискретных модулей 4*/
            byte[] workBytes = ArrayExtension.UshortArrayToByteArray(value);
            this.uiPicon2State1.Value = new byte[] { workBytes[1], workBytes[0] };
            this.uiPicon2State2.Value = new byte[] { workBytes[3], workBytes[2] };
            this.uiPicon2State3.Value = new byte[] { workBytes[5], workBytes[4] };
            this.uiPicon2State4.Value = new byte[] { workBytes[7], workBytes[6] };
        }
        #endregion

        #region IQueryMember

        public ushort[] Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (value == null)
                {
                    this.SetDefault();
                }
                else
                {
                    this._value = value;
                    this.SetStates(value);
                }

            }
        }

        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();

            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                try
                {
                    ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0005, 4);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetPicon2States(value);
                    });
                }
                catch (Exception ex)
                { }
            }
            else
            {
                try
                {
                    ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0005, 8);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetStates(value);
                    });
                }
                catch (Exception ex)
                { }
            }

            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        private void ReadCompleted(ushort[] res)
        {
            this.Value = res;
        }

        public bool WriteContext()
        {
            throw new NotImplementedException();
        }
        #endregion

    }


}
