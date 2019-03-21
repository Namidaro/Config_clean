using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;
using UniconGS.Source;
using UniconGS.Interfaces;
using System.Threading;

namespace UniconGS.UI
{
    /// <summary>
    /// Логика взаимодействия для Runo3Diagnostics.xaml
    /// </summary>
    public partial class Runo3Diagnostics : UserControl, IUpdatableControl
    {

        private ushort[] _value = null;
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        private delegate void ReadComplete(ushort[] res);
        public Runo3Diagnostics()
        {
            InitializeComponent();

        }



        private void SetAllOffline()
        {
           foreach (Lightning item in this.uiReleRunoModule.Children)
           {
               item.Value = null;
           }
            foreach (Lightning item in this.uiCh.Children)
            {
                item.Value = null;
            }

            foreach (Lightning item in this.uiGSM1.Children)
            {
                item.Value = null;
            }

            foreach (Lightning item in this.uiGSM2.Children)
            {
                item.Value = null;
            }

            foreach (Lightning item in this.uiReleWork.Children)
            {
                item.Value = null;
            }
           }
        private void SetValue(ushort[] value)
        {

            var discretModule1 = Converter.GetBitsFromWord(value[0]);
            var discretModule2 = Converter.GetBitsFromWord(value[1]);
            var discretModule3 = Converter.GetBitsFromWord(value[2]);
            var discretModule4 = Converter.GetBitsFromWord(value[3]);
            var releRunoModule = Converter.GetBitsFromWord(value[4]);
            this.SetReleLight(releRunoModule);
            
        }

        private void SetReleLight(BitArray value)
        {
            for (int i = 0; i < this.uiReleRunoModule.Children.Count; i++)
            {
                (this.uiReleRunoModule.Children[i] as Lightning).ValueRuno = value[i];

                foreach (Lightning item in this.uiCh.Children)
                {
                    item.SetCh();
                }
                foreach (Lightning item in this.uiGSM1.Children)
                {
                    item.SetOff();
                }
                foreach (Lightning item in this.uiGSM2.Children)
                {
                    item.SetOn();
                }
                foreach (Lightning item in this.uiReleWork.Children)
                {
                    item.SetOff();
                }
            }

            

        }

        public ushort[] Value
        {
            get
            {
                return this._value;
            }
            set
            {
            }
        }
        //public bool WriteContext()
        //{
        //    throw new NotImplementedException();
        //}
        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();
            ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0200, 5);
            Application.Current.Dispatcher.Invoke(() =>
            {

                if (value == null)
                {
                    this.SetAllOffline();
                }
                else
                {
                    this.SetValue(value);
                }
            });

            //if (_semaphoreSlim.CurrentCount == 0)
            //{
            //    _semaphoreSlim.Release();
            //}

        }

        public void SetAutonomus()
        {
            SetAllOffline();
        }



        //public void Update()
        //{
        //    var res = DataTransfer.ReadWords(this._querer);
        //    this.Dispatcher.BeginInvoke(new ReadComplete(ReadCompleted), DispatcherPriority.SystemIdle,
        //        new object[] { res });
        //}


        //public Slot Querer
        //{
        //    get
        //    {
        //        return this._querer;
        //    }
        //    set
        //    {
        //        this._querer = value;
        //    }
        //}
    }
}
