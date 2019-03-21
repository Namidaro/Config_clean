using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;
using UniconGS.Source;
using System.Collections;

namespace UniconGS.UI
{
    /// <summary>
    /// Interaction logic for ChannelsManagment.xaml
    /// </summary>
    public partial class ChannelsManagment : UserControl, IQuery
    {
        #region Globals

        private ushort[] _value;

        private delegate void ReadComplete(ushort[] res);
        #endregion

        public ChannelsManagment()
        {
            InitializeComponent();
            /* убрано нажатие на кнопки, потому что КН не дает записывать команды */
            //this.uiChannel1.WriteValue += new ChannelManagment.WriteValueEventHandler(uiChannel_WriteValue);
            //this.uiChannel2.WriteValue += new ChannelManagment.WriteValueEventHandler(uiChannel_WriteValue);
            //this.uiChannel3.WriteValue += new ChannelManagment.WriteValueEventHandler(uiChannel_WriteValue);
            //this.uiChannel4.WriteValue += new ChannelManagment.WriteValueEventHandler(uiChannel_WriteValue);
            //this.uiChannel5.WriteValue += new ChannelManagment.WriteValueEventHandler(uiChannel_WriteValue);
            //this.uiChannel6.WriteValue += new ChannelManagment.WriteValueEventHandler(uiChannel_WriteValue);
            //this.uiChannel7.WriteValue += new ChannelManagment.WriteValueEventHandler(uiChannel_WriteValue);
            //this.uiChannel8.WriteValue += new ChannelManagment.WriteValueEventHandler(uiChannel_WriteValue);
        }

        #region Methods
        public void SetAutonomous()
        {
            this.uiChannel1.SetAutonomous();
            this.uiChannel2.SetAutonomous();
            this.uiChannel3.SetAutonomous();
            this.uiChannel4.SetAutonomous();
            this.uiChannel5.SetAutonomous();
            this.uiChannel6.SetAutonomous();
            this.uiChannel7.SetAutonomous();
            this.uiChannel8.SetAutonomous();
        }

        private void DisableAll()
        {
            this.uiChannel1.Value = null;
            this.uiChannel2.Value = null;
            this.uiChannel3.Value = null;
            this.uiChannel4.Value = null;
            this.uiChannel5.Value = null;
            this.uiChannel6.Value = null;
            this.uiChannel7.Value = null;
            this.uiChannel8.Value = null;
        }

        private void SetChannelsValue(ushort[] value)
        {
            BitArray outErrorsBits = Converter.GetBitsFromWords(new ushort[] { value[2], value[3] });
            BitArray localCommandsBits = Converter.GetBitsFromWords(new ushort[] { value[0], value[1] });
            BitArray controlBits = Converter.GetBitsFromWord(value[4]);

            this.uiChannel1.Value = new List<bool>(7) { outErrorsBits[0], outErrorsBits[3], outErrorsBits[2],
                    localCommandsBits[0], localCommandsBits[3], localCommandsBits[2], controlBits[0] };
            this.uiChannel2.Value = new List<bool>(7) { outErrorsBits[4], outErrorsBits[7], outErrorsBits[6],
                    localCommandsBits[4], localCommandsBits[7], localCommandsBits[6], controlBits[1] };
            this.uiChannel3.Value = new List<bool>(7) { outErrorsBits[8], outErrorsBits[11], outErrorsBits[10],
                    localCommandsBits[8], localCommandsBits[11], localCommandsBits[10], controlBits[2] };
            this.uiChannel4.Value = new List<bool>(7) { outErrorsBits[12], outErrorsBits[15], outErrorsBits[14],
                    localCommandsBits[12], localCommandsBits[15], localCommandsBits[14], controlBits[3] };
            this.uiChannel5.Value = new List<bool>(7) { outErrorsBits[16], outErrorsBits[19], outErrorsBits[18],
                    localCommandsBits[16], localCommandsBits[19], localCommandsBits[18], controlBits[4] };
            this.uiChannel6.Value = new List<bool>(7) { outErrorsBits[20], outErrorsBits[23], outErrorsBits[22],
                    localCommandsBits[20], localCommandsBits[23], localCommandsBits[22], controlBits[5] };
            this.uiChannel7.Value = new List<bool>(7) { outErrorsBits[24], outErrorsBits[27], outErrorsBits[26],
                    localCommandsBits[24], localCommandsBits[27], localCommandsBits[26], controlBits[6] };
            this.uiChannel8.Value = new List<bool>(7) { outErrorsBits[28], outErrorsBits[31], outErrorsBits[30],
                    localCommandsBits[28], localCommandsBits[31], localCommandsBits[30], controlBits[7] };
        }

        #region Убрано нажатие на кнопки, потому что КН не дает записывать команды
        /*private void uiChannel_WriteValue(object sender, object value)
        {
            if (this.IsEnabled)
            {
                List<bool> tmp = new List<bool>();
                if (sender.Equals(this.uiChannel1))
                {
                    var val = (value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { val[0], false, val[2], val[1] });
                    var tmp1 = (this.uiChannel2.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel3.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel4.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel5.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel6.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel7.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel8.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(this.uiChannel2))
                {
                    var tmp1 = (this.uiChannel1.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = (value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { val[0], false, val[2], val[1] });
                    tmp1 = (this.uiChannel3.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel4.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel5.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel6.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel7.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel8.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(this.uiChannel3))
                {
                    var tmp1 = (this.uiChannel1.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel2.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = (value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { val[0], false, val[2], val[1] });
                    tmp1 = (this.uiChannel4.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel5.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel6.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel7.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel8.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(this.uiChannel4))
                {
                    var tmp1 = (this.uiChannel1.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel2.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel3.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = (value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { val[0], false, val[2], val[1] });
                    tmp1 = (this.uiChannel5.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel6.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel7.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel8.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(this.uiChannel5))
                {
                    var tmp1 = (this.uiChannel1.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel2.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel3.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel4.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = (value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { val[0], false, val[2], val[1] });
                    tmp1 = (this.uiChannel6.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel7.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel8.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(this.uiChannel6))
                {
                    var tmp1 = (this.uiChannel1.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel2.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel3.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel4.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel5.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = (value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { val[0], false, val[2], val[1] });
                    tmp1 = (this.uiChannel7.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel8.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(this.uiChannel7))
                {
                    var tmp1 = (this.uiChannel1.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel2.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel3.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel4.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel5.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel6.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = (value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { val[0], false, val[2], val[1] });
                    tmp1 = (this.uiChannel8.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                }
                else if (sender.Equals(this.uiChannel8))
                {
                    var tmp1 = (this.uiChannel1.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel2.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel3.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel4.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel5.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel6.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    tmp1 = (this.uiChannel7.Value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { tmp1[0], false, tmp1[2], tmp1[1] });
                    var val = (value as List<bool>).GetRange(3, 3);
                    tmp.AddRange(new List<bool>() { val[0], false, val[2], val[1] });
                }

                this._querers[1].Value = Converter.GetWordsFromBits(tmp);
                this._worker.CancelAsync();
                DataTransfer.WriteWords(this._querers[1]);
            }
        }*/
        #endregion

        void RunWorkerCompleted(ushort[] res)
        {
            this.Value = res;
        }
        #endregion

        #region IQueryMember 

        public Slot Querer { get; set; }
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
                    _value = value;
                    this.SetChannelsValue(_value);
                }
            }
        }

        public void Update()
        {
            var res = DataTransfer.ReadWords(this.Querer);
            this.Dispatcher.BeginInvoke(new ReadComplete(RunWorkerCompleted), DispatcherPriority.SystemIdle, res);
        }

        public bool WriteContext()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
