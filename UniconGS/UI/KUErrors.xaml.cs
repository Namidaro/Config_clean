using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UniconGS.Interfaces;
using UniconGS.Source;
using System.Threading.Tasks;
using UniconGS.Enums;
using System.Threading;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for KUErrors.xaml
    /// </summary>
    public partial class KUErrors : UserControl, IUpdatableControl
    {
        #region Globals
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private ushort[] _value;
        private Slot _query;

        private delegate void ReadComplete(ushort[] res);
        #endregion

        public KUErrors()
        {
            InitializeComponent();
            if(DeviceSelection.SelectedDevice==(byte)DeviceSelectionEnum.DEVICE_PICON2)
            {
                this.uiErrors.Children[5].Visibility = Visibility.Visible;
            }
            else
            {
                this.uiErrors.Children[5].Visibility = Visibility.Collapsed;
            }
            //if (_semaphoreSlim.CurrentCount == 0)
            //    _semaphoreSlim.Release();
        }

        private void SetDefault()
        {
            this.uiErrors.Children.OfType<BitViewer>().ToList().ForEach((alarm) => { alarm.Value = null; });
        }

        private void SetErrors(ushort[] value)
        {
            var tmp = Converter.GetBitsFromWord(value[0]).OfType<bool>().ToList();
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
            {
                (this.uiErrors.Children[0] as BitViewer).Value = tmp[0];//неисправность питания
                (this.uiErrors.Children[1] as BitViewer).Value = tmp[3];//неисправность цепей управления
                (this.uiErrors.Children[2] as BitViewer).Value = tmp[2];//неисправность охраны
                (this.uiErrors.Children[3] as BitViewer).Value = tmp[1];//неисправность управления
                (this.uiErrors.Children[4] as BitViewer).Value = tmp[4];//неисправность предохранителей

                (this.uiErrors.Children[5] as BitViewer).Value = tmp[7];//неисправность контроллера

            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    (this.uiErrors.Children[i] as BitViewer).Value = tmp[i];
                }
            }
        }

        void RunWorkerComplete(ushort[] value)
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
                    this.SetDefault();
                }
                else
                {
                    SetErrors(value);
                }
            }
        }
        //public Slot Querer
        //{
        //    get
        //    {
        //        return this._query;
        //    }
        //    set
        //    {
        //        this._query = value;
        //    }
        //}
        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();


            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
            {
                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0004, 1);
                //var res = DataTransfer.ReadWords(this._query);new Slot(0x0004, 1, "Errors");
                //this.Dispatcher.BeginInvoke(new ReadComplete(RunWorkerCompleted), DispatcherPriority.SystemIdle,res);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetErrors(value);
                });
            }
            else
            {

                ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0004, 1);
                //var res = DataTransfer.ReadWords(this._query);new Slot(0x0004, 1, "Errors");
                //this.Dispatcher.BeginInvoke(new ReadComplete(RunWorkerCompleted), DispatcherPriority.SystemIdle,res);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetErrors(value);
                });
            }

            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}
        }
        //public bool WriteContext()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
