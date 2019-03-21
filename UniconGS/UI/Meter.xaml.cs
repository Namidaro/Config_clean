using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UniconGS.Interfaces;
using UniconGS.Source;
using UniconGS.Enums;
using System.Text;
using UniconGS.UI.Picon2;
using System.Threading;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for Meter.xaml
    /// </summary>
    public partial class Meter : UserControl, IUpdatableControl
    {
        #region Globals
        private ushort[] _value;
        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();
        public delegate void ShowMessageEventHandler(string message, string caption, MessageBoxImage image);

        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        #endregion

        public Meter()
        {
            InitializeComponent();
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
            if (DeviceSelection.SelectedDevice==(int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                uiEnergyM.Visibility = Visibility.Collapsed;
                uiEnergyD.Visibility = Visibility.Collapsed;
                EnM.Visibility = Visibility.Collapsed;
                EnD.Visibility = Visibility.Collapsed;
            }
            else
            {
                uiEnergyM.Visibility = Visibility.Visible;
                uiEnergyD.Visibility = Visibility.Visible;
                EnM.Visibility = Visibility.Visible;
                EnD.Visibility = Visibility.Visible;
            }
        }


        private void DisableAll()
        {
            foreach (var item in this.PART_DATAHOLDER.Children)
            {
                if (item is Label)
                {
                    (item as Label).Content = "Нет значения";
                }
            }
        }

        private void SetAll(Label lable, ushort[] value, int sourceIndex)
        {
            //var valFromMeter = await ReadAll();
            //SetAll(valFromMeter);
            ushort[] tmp = new ushort[16];


            if (value[0] == 0)
            {
                lable.Content = "Нет значения";
            }
            else
            {
                Array.Copy(value, sourceIndex, tmp, 0, 8);
                lable.Content = Converter.GetStringFromWords(tmp);
            }
        }


        private void SetAllPicon2(Label lable, byte[] value, int sourceIndex, int length, Picon2MeterFormatterSelector formatterSelector)
        {

            byte[] tmp = new byte[length];
            Array.Copy(value, sourceIndex, tmp, 0, length);
            switch (formatterSelector)
            {
                case  Picon2MeterFormatterSelector.SELECTOR_VOLTAGE:
                    {
                        lable.Content = Converter.BytesToIntVoltageFormatterPicon2(tmp).ToString();

                        break;
                    }
                case Picon2MeterFormatterSelector.SELECTOR_CURRENT:
                    {
                        lable.Content = Converter.BytesToIntCurrentFormatterPicon2(tmp).ToString();

                        break;
                    }
                case Picon2MeterFormatterSelector.SELECTOR_POWER:
                    {
                        lable.Content = Converter.BytesToIntPowerFormatterPicon2(tmp).ToString();

                        break;
                    }
                case Picon2MeterFormatterSelector.SELECTOR_ENERGY:
                    {
                        lable.Content = Converter.BytesToLongEnergyFormatterPicon2(tmp).ToString();

                        break;
                    }
            }
            

        }
        private void SetAll(ushort[] value)
        {
            this.SetAll(this.uiDate, value, 8);
            this.SetAll(this.uiTime, value, 16);
            this.SetAll(this.uiSerialNumber, value, 24);
            this.SetAll(this.uiPowerA, value, 40);
            this.SetAll(this.uiPowerB, value, 48);
            this.SetAll(this.uiPowerC, value, 56);
            this.SetAll(this.uiVoltageA, value, 64);
            this.SetAll(this.uiVoltageB, value, 72);
            this.SetAll(this.uiVoltageC, value, 80);
            this.SetAll(this.uiCurrentA, value, 88);
            this.SetAll(this.uiCurrentB, value, 96);
            this.SetAll(this.uiCurrentC, value, 104);
            this.SetAll(this.uiEnergyO, value, 120);
            this.SetAll(this.uiEnergyM, value, 128);
            this.SetAll(this.uiEnergyD, value, 136);
        }

        private void SetAllPicon2(ushort[] value)
        {
            byte[] bytesValue = ArrayExtension.UshortArrayToByteArray(value);

            this.SetAllPicon2(this.uiVoltageA, bytesValue, 0, 2, Picon2MeterFormatterSelector.SELECTOR_VOLTAGE);
            this.SetAllPicon2(this.uiVoltageB, bytesValue, 2, 2, Picon2MeterFormatterSelector.SELECTOR_VOLTAGE);
            this.SetAllPicon2(this.uiVoltageC, bytesValue, 4, 2, Picon2MeterFormatterSelector.SELECTOR_VOLTAGE);
            this.SetAllPicon2(this.uiCurrentA, bytesValue, 6, 2, Picon2MeterFormatterSelector.SELECTOR_CURRENT);
            this.SetAllPicon2(this.uiCurrentB, bytesValue, 8, 2, Picon2MeterFormatterSelector.SELECTOR_CURRENT);
            this.SetAllPicon2(this.uiCurrentC, bytesValue, 10, 2, Picon2MeterFormatterSelector.SELECTOR_CURRENT);
            this.SetAllPicon2(this.uiPowerA, bytesValue, 12, 2, Picon2MeterFormatterSelector.SELECTOR_POWER);
            this.SetAllPicon2(this.uiPowerB, bytesValue, 14, 2, Picon2MeterFormatterSelector.SELECTOR_POWER);
            this.SetAllPicon2(this.uiPowerC, bytesValue, 16, 2, Picon2MeterFormatterSelector.SELECTOR_POWER);
            this.SetAllPicon2(this.uiEnergyO, bytesValue, 18, 4, Picon2MeterFormatterSelector.SELECTOR_ENERGY);
            //this.SetAllPicon2(this.uiEnergyM, bytesValue, 26, 4, Picon2MeterFormatterSelector.SELECTOR_ENERGY);
            //this.SetAllPicon2(this.uiEnergyD, bytesValue, 30, 4, Picon2MeterFormatterSelector.SELECTOR_ENERGY);
        }

        void ReadCompleted(ushort[] value)
        {
            this.Value = value;
        }

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
                    this.DisableAll();
                }
                else
                {
                    this.SetAll(value);
                    this._value = value;
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
                    ushort[] value = await ReadAllPicon2();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetAllPicon2(value);
                    });
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

                try
                {
                    ushort[] value = await ReadAll();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetAll(value);
                    });
                }
                catch (Exception ex)
                {

                }
            }

            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}

        }

        private async Task<ushort[]> ReadAllPicon2()
        {
            List<ushort> ushorts = new List<ushort>();
            ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x000E), 20));
            return ushorts.ToArray();
        }

        private async Task<ushort[]> ReadAll()
        {
            List<ushort> ushorts = new List<ushort>();
            for (ushort i = 0; i < 200; i += 100)
            {
                ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x0208 + i), 100));
            }
            ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, 0x0208 + 200, 86));

            return ushorts.ToArray();
        }

        #endregion
    }
}
