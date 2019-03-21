using System;
using System.Windows.Controls;
using System.Windows.Threading;
using UniconGS.Source;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for AllStates.xaml
    /// </summary>
    public partial class AllStates : UserControl, IQuery
    {
        #region Globals
        private Slot _querer;
        private ushort[] _value;

        private delegate void ReadComplete(ushort[] res);
        #endregion

        public AllStates()
        {
            InitializeComponent();
        }

        #region Privtaes
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
            this.uiState1.Value = new ushort[] {value[0], value[1]};
            this.uiState2.Value = new ushort[] {value[2], value[3]};
            this.uiState3.Value = new ushort[] {value[4], value[5]};
            this.uiState4.Value = new ushort[] {value[6], value[7]};
        }
        #endregion

        #region IQueryMember
        public Slot Querer
        {
            get
            {
                return this._querer;
            }
            set
            {
                this._querer = value;
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

        public void Update()
        {
            var res = DataTransfer.ReadWords(this.Querer);
            this.Dispatcher.BeginInvoke(new ReadComplete(ReadCompleted), DispatcherPriority.SystemIdle, res);
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
