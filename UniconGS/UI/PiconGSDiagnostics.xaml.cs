using System;
using System.Linq;
using System.Windows.Controls;
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
    /// Interaction logic for PiconGSDiagnostics.xaml
    /// </summary>
    public partial class PiconGSDiagnostics : UserControl, IUpdatableControl
    {
        #region Globals
        private ushort[] _value = null;
        //private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

       
        private delegate void ReadComplete(ushort[] res);
        #endregion

        public PiconGSDiagnostics()
        {
            InitializeComponent();
        }

        public void SetAllOffline()
        {
            foreach (Lightning item in this.uiDiscretModule1.Children)
            {
                item.Value = null;
            }
            foreach (Lightning item in this.uiDiscretModule2.Children)
            {
                item.Value = null;
            }
            foreach (Lightning item in this.uiDiscretModule3.Children)
            {
                item.Value = null;
            }
            foreach (Lightning item in this.uiDiscretModule4.Children)
            {
                item.Value = null;
            }
            foreach (Lightning item in this.uiReleModule.Children)
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
            var releModule = Converter.GetBitsFromWord(value[4]);
            this.SetReleLight(releModule);
            this.SetDiscretes(discretModule1, discretModule2, discretModule3, discretModule4);
        }

        private void SetReleLight(BitArray value)
        {
            for (int i = 0; i < this.uiReleModule.Children.Count; i++)
            {
                (this.uiReleModule.Children[i] as Lightning).Value = value[i];
            }
        }

        private void SetDiscretes(BitArray module1Value, BitArray module2Value, BitArray module3Value, BitArray module4Value)
        {
            for (int i = 0; i < this.uiDiscretModule1.Children.Count; i++)
            {
                (this.uiDiscretModule1.Children[i] as Lightning).Value = module1Value[i];
                (this.uiDiscretModule2.Children[i] as Lightning).Value = module2Value[i];
                (this.uiDiscretModule3.Children[i] as Lightning).Value = module3Value[i];
                (this.uiDiscretModule4.Children[i] as Lightning).Value = module4Value[i];
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
                    this.SetAllOffline();
                }
                else
                {
                    this.SetValue(value);
                }
            }
        }
        public async Task Update()
        {
            //if (_semaphoreSlim.CurrentCount == 0) return;
            //await _semaphoreSlim.WaitAsync();

            ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x0200, 5);
            Application.Current.Dispatcher.Invoke(() =>
            {
                SetValue(value);
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

        #endregion
    }
}
