using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UniconGS.Interfaces;
using UniconGS.Source;
using UniconGS.Enums;
using System.Threading;

namespace UniconGS.UI.Channels
{
    /// <summary>
    ///     Interaction logic for ChannelsManagment.xaml
    /// </summary>
    public partial class ChannelsManagment : UserControl, IUpdatableControl

    {
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public ChannelsManagment()
        {
            InitializeComponent();
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
            _channelsList = new List<ChannelManagment>
            {
                uiChannel1,
                uiChannel2,
                uiChannel3,
                uiChannel4,
                uiChannel5,
                uiChannel6,
                uiChannel7,
                uiChannel8
            };

            foreach (var channel in _channelsList)
                channel.WriteValue += uiChannel_WriteValue;
        }

        public void SetAutonomous()
        {
            uiChannel1.SetAutonomous();
            uiChannel2.SetAutonomous();
            uiChannel3.SetAutonomous();
            uiChannel4.SetAutonomous();
            uiChannel5.SetAutonomous();
            uiChannel6.SetAutonomous();
            uiChannel7.SetAutonomous();
            uiChannel8.SetAutonomous();
        }

        public void DisableAutonomus()
        {
            uiChannel1.DisableAutonomus();
            uiChannel2.DisableAutonomus();
            uiChannel3.DisableAutonomus();
            uiChannel4.DisableAutonomus();
            uiChannel5.DisableAutonomus();
            uiChannel6.DisableAutonomus();
            uiChannel7.DisableAutonomus();
            uiChannel8.DisableAutonomus();
        }

        private void DisableAll()
        {
            uiChannel1.Value = null;
            uiChannel2.Value = null;
            uiChannel3.Value = null;
            uiChannel4.Value = null;
            uiChannel5.Value = null;
            uiChannel6.Value = null;
            uiChannel7.Value = null;
            uiChannel8.Value = null;
        }

        private void SetChannelsValue(ushort[] value)
        {
            var outErrorsBits = Converter.GetBitsFromWords(new[] { value[2], value[3] });
            var localCommandsBits = Converter.GetBitsFromWords(new[] { value[0], value[1] });

            var controls = new BitArray(new[] { (byte)BitConverter.GetBytes(value[4]).GetValue(1) }).OfType<bool>()
                .ToArray();

            uiChannel1.Value = new List<bool>
            {
                outErrorsBits[0],
                outErrorsBits[3],
                outErrorsBits[2],
                localCommandsBits[0],
                localCommandsBits[3],
                localCommandsBits[2],
                controls[0]
            };
            uiChannel2.Value = new List<bool>
            {
                outErrorsBits[4],
                outErrorsBits[7],
                outErrorsBits[6],
                localCommandsBits[4],
                localCommandsBits[7],
                localCommandsBits[6],
                controls[1]
            };
            uiChannel3.Value = new List<bool>
            {
                outErrorsBits[8],
                outErrorsBits[11],
                outErrorsBits[10],
                localCommandsBits[8],
                localCommandsBits[11],
                localCommandsBits[10],
                controls[2]
            };
            uiChannel4.Value = new List<bool>
            {
                outErrorsBits[12],
                outErrorsBits[15],
                outErrorsBits[14],
                localCommandsBits[12],
                localCommandsBits[15],
                localCommandsBits[14],
                controls[3]
            };
            uiChannel5.Value = new List<bool>
            {
                outErrorsBits[16],
                outErrorsBits[19],
                outErrorsBits[18],
                localCommandsBits[16],
                localCommandsBits[19],
                localCommandsBits[18],
                controls[4]
            };
            uiChannel6.Value = new List<bool>
            {
                outErrorsBits[20],
                outErrorsBits[23],
                outErrorsBits[22],
                localCommandsBits[20],
                localCommandsBits[23],
                localCommandsBits[22],
                controls[5]
            };
            uiChannel7.Value = new List<bool>
            {
                outErrorsBits[24],
                outErrorsBits[27],
                outErrorsBits[26],
                localCommandsBits[24],
                localCommandsBits[27],
                localCommandsBits[26],
                controls[6]
            };
            uiChannel8.Value = new List<bool>
            {
                outErrorsBits[28],
                outErrorsBits[31],
                outErrorsBits[30],
                localCommandsBits[28],
                localCommandsBits[31],
                localCommandsBits[30],
                controls[7]
            };
        }

        private async void uiChannel_WriteValue(object sender, List<bool> value)
        {
            if (IsEnabled)
            {
                //if (_semaphoreSlim.CurrentCount == 0) return;
                // await _semaphoreSlim.WaitAsync();

                var tmp = new List<bool>();
                if (sender.Equals(uiChannel1))
                {
                    var val = value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { val[0], false, val[2], val[1] });
                    var tmp1 = uiChannel2.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel3.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel4.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel5.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel6.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel7.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel8.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(uiChannel2))
                {
                    var tmp1 = uiChannel1.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { val[0], false, val[2], val[1] });
                    tmp1 = uiChannel3.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel4.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel5.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel6.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel7.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel8.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(uiChannel3))
                {
                    var tmp1 = uiChannel1.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel2.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { val[0], false, val[2], val[1] });
                    tmp1 = uiChannel4.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel5.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel6.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel7.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel8.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(uiChannel4))
                {
                    var tmp1 = uiChannel1.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel2.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel3.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { val[0], false, val[2], val[1] });
                    tmp1 = uiChannel5.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel6.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel7.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel8.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(uiChannel5))
                {
                    var tmp1 = uiChannel1.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel2.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel3.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel4.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { val[0], false, val[2], val[1] });
                    tmp1 = uiChannel6.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel7.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel8.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(uiChannel6))
                {
                    var tmp1 = uiChannel1.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel2.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel3.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel4.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel5.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { val[0], false, val[2], val[1] });
                    tmp1 = uiChannel7.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel8.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(uiChannel7))
                {
                    var tmp1 = uiChannel1.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel2.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel3.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel4.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel5.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel6.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { val[0], false, val[2], val[1] });
                    tmp1 = uiChannel8.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if
                    (sender.Equals(uiChannel8))
                {
                    var tmp1 = uiChannel1.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel2.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel3.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel4.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel5.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel6.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = uiChannel7.Value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = value.GetRange(3, 3);
                    tmp.AddRange(new List<bool> { val[0], false, val[2], val[1] });
                }
                

                await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x0000, Converter.GetWordsFromBits(tmp));


                _writeValue = tmp;

                //if (_semaphoreSlim.CurrentCount == 0)
                //{
                //    _semaphoreSlim.Release();
                //}
                //DataTransfer.SetTopInQueue(this, Accsess.Write, false);
            }
        }

        private void RunWorkerCompleted(ushort[] res)
        {
            Value = res;
            if (res == null)
                DisableAll();
            else
                SetChannelsValue(res);
        }


        private void CommandSetted(bool res)
        {
            if (!res)
                MessageBox.Show("Невозможно выполнить команду", "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        #region Globals

        private List<bool> _writeValue;
        private readonly List<ChannelManagment> _channelsList;

        private delegate void ReadComplete(ushort[] res);

        private delegate void WriteComplete(bool res);

        public delegate void StartWorkEventHandler();

        public delegate void StopWorkEventHandler();

        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;

        #endregion

        #region IQueryMember 

        //public Slot Querer { get; set; }

        public ushort[] Value { get; set; }

        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();

            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
            {
                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0000, 5);
                //var res = DataTransfer.ReadWords(Querer);
                //Dispatcher.BeginInvoke(new ReadComplete(RunWorkerCompleted), DispatcherPriority.SystemIdle, res);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetChannelsValue(value);
                });



            }
            else
            {
                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0000, 5);
                //var res = DataTransfer.ReadWords(Querer);
                //Dispatcher.BeginInvoke(new ReadComplete(RunWorkerCompleted), DispatcherPriority.SystemIdle, res);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetChannelsValue(value);
                });

            }

            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task<bool> WriteContext()
        {
            //if (StartWork != null)
            //    Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(StartWork));
            bool res = true;
            try
            {
                await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x0000, Value);
                {
                    Value = Converter.GetWordsFromBits(_writeValue);
                }
                await Dispatcher.BeginInvoke(new WriteComplete(CommandSetted), DispatcherPriority.SystemIdle, res);
            }


            catch (Exception e)
            {
                res = false;
            }
            if (StopWork != null)
                await Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(StopWork));
            return res;
        }

        #endregion
    }
}