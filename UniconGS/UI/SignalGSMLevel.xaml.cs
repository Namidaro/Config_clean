using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using UniconGS.Source;
using System.Windows.Media;
using System.Globalization;
using System.Threading.Tasks;
using UniconGS.Interfaces;
using UniconGS.UI;
using UniconGS.Enums;
using UniconGS.UI.Picon2;
using Brushes = System.Windows.Media.Brushes;
using System.Threading;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for SignalGSMLevel.xaml
    /// </summary>
    public partial class SignalGSMLevel : UserControl, IUpdatableControl
    {
        private Slot _query;
        internal SignalGSMLevel.ShowMessageEventHandler ShowMessage;
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        //private delegate void ReadComplete(ushort[] res);
        public SignalGSMLevel()
        {
            InitializeComponent();
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
        }


        private void SetGsm(Label uiSignalGSM, ushort[] value, int sourceIndex)
        {
            ushort[] tmp = new ushort[16];
            var SignalValue = value[0];
            if (SignalValue == 0)
            {
                UiSignalGSM.Visibility = Visibility.Hidden;
                SignalLevelMapping.Visibility = Visibility.Hidden;
                this.uiLevelLabel.Visibility = Visibility.Hidden;
                uiNoLevelLabel.Visibility = Visibility.Visible;
                uiSignalGSM.Background = System.Windows.Media.Brushes.White;
            }
            if (SignalValue > 0 && SignalValue <= 10)
            {
                uiSignalGSM.Content = value[0];
                uiSignalGSM.Background = System.Windows.Media.Brushes.Red;
            }
            if (SignalValue >= 11 && SignalValue <= 20)
            {
                uiSignalGSM.Content = value[0];
                uiSignalGSM.Background = System.Windows.Media.Brushes.Yellow;
            }
            if (SignalValue >= 21 && SignalValue != 99)
            {
                uiSignalGSM.Content = value[0];
                uiSignalGSM.Background = System.Windows.Media.Brushes.LimeGreen;
            }
            if (SignalValue == 99)
            {
                UiSignalGSM.Visibility = Visibility.Hidden;
                SignalLevelMapping.Visibility = Visibility.Hidden;
                this.uiLevelLabel.Visibility = Visibility.Hidden;
                uiNoLevelLabel.Visibility = Visibility.Visible;
                uiSignalGSM.Background = System.Windows.Media.Brushes.White;
            }

            /*else
                {
                    uiSignalGSM.Content = value[0];
                }
                */
        }

        void SetGsm(ushort[] value)
        {
            this.SetGsm(this.UiSignalGSM, value, 1);
            this.SignalLevelMapping.UpdateState(value[0]);

        }

        private void SetGsmPicon2(byte[] value)
        {
            ArrayExtension.SwapArrayItems(ref value);
            ushort[] _val = ArrayExtension.ByteArrayToUshortArray(value);
            this.SetGsm(this.UiSignalGSM, _val, 1);
            this.SignalLevelMapping.UpdateState(_val[0]);

        }

        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
            {
                ushort[] ConnectionModuleId;
                {
                    ConnectionModuleId = await RTUConnectionGlobal.GetDataByAddress(1, 0x3004, 1);
                }
                if ((byte)ConnectionModuleId[0] != 0xE0 && (byte)ConnectionModuleId[0] != 0xE1)
                    return;
                byte[] value = await RTUConnectionGlobal.ExecuteFunction12Async((byte)ConnectionModuleId[0], "Get Picon SignalLevel", 0x60);
                if (value == null)
                    return;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetGsmPicon2(value);
                });
            }
            else
            {

                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x001F, 8);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetGsm(value);
                });
            }
            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}
        }



        public void SetAutonomus()
        {
            UiSignalGSM.Visibility = Visibility.Hidden;
            SignalLevelMapping.Visibility = Visibility.Hidden;
            this.uiLevelLabel.Visibility = Visibility.Hidden;
            uiNoLevelLabel.Visibility = Visibility.Visible;
        }

        internal class ShowMessageEventHandler
        {
            private Action<string, string, MessageBoxImage> showMessage;

            public ShowMessageEventHandler(Action<string, string, MessageBoxImage> showMessage)
            {
                this.showMessage = showMessage;
            }
        }


    }

}
