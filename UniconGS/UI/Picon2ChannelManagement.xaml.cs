using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniconGS.Source;
using UniconGS.Enums;
using System.Threading;

namespace UniconGS.UI
{
    /// <summary>
    /// Логика взаимодействия для TurnOnError.xaml
    /// </summary>
    public partial class Picon2ChannelManagement : UserControl
    {
        public Picon2ChannelManagement()
        {
            InitializeComponent();
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
        }
        #region Globals
        private ushort[] _value;
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        private delegate void ReadComplete(ushort[] res);
        #endregion


        private void DisableAllFlags()
        {
            foreach (var item in this.uiPicon2ChannelManagement.Children)
            {
                (item as BitViewer).Value = null;
            }

        }

        private void SetAllFlags(ushort value)
        {
            BitArray array = Converter.GetBitsFromWord(value);

            for (int i = 0; i < this.uiPicon2ChannelManagement.Children.Count; i++)
            {
                (this.uiPicon2ChannelManagement.Children[i] as BitViewer).Value = array[i];
            }
        }
        private void SetAllFlagsPicon2(ushort value)
        {
            BitArray array = Converter.GetBitsFromWord(value);

            for (int i = 0; i < this.uiPicon2ChannelManagement.Children.Count; i++)
            {
                (this.uiPicon2ChannelManagement.Children[i] as BitViewer).Value = array[i + 8];//сдвиг на 8, т.к. нужны биты 8-15
            }
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
                    this.DisableAllFlags();
                }
                else
                {
                    this.SetAllFlags(value[0]);
                }
            }
        }

        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {

                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0004, 1);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetAllFlagsPicon2(value[0]);
                });
            }
            else
            {


                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0304, 1);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetAllFlags(value[0]);
                });
            }
            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}
        }
        #endregion



    }

}
