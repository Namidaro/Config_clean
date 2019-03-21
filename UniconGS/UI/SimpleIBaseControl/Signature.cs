using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UniconGS.Source;

namespace UniconGS.UI.SimpleIBaseControl
{
    public class Signature: IQuery
    {
        private delegate void ReadComplete(ushort[] res);
        public Signature()
        {
            Querer = new Slot(0x0400, 52, "PLC Signature");
        }
        private string GetSignatureString(List<ushort> value)
        {
            ushort[] deviceName = value.GetRange(0, 8).ToArray();
            ushort[] version = value.GetRange(8, 2).ToArray();
            ushort[] date = value.GetRange(16, 10).ToArray();

            var devName = "Имя устройства: " + Converter.GetStringFromWords(deviceName) + ";\r\n";
            var v = "Версия:  " + ((byte)(version[1] >> 8)).ToString() + "."
                    + ((byte)version[1]).ToString() + "."
                    + ((byte)(version[0] >> 8)).ToString() + "."
                    + ((byte)version[0]).ToString() + ";\n\r";
            var d = "Дата: " + Converter.GetStringFromWords(date) + ".";

            return devName + v + d;
        }

        private void RunWorkerCompleted(ushort[] value)
        {
            if (value == null)
            {
                MessageBox.Show("Ошибка чтения сигнауры", "Сигнатура устройства.", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(this.GetSignatureString((value).ToList()),
                    "Сигнатура устройства.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #region IBaseMember
        public Slot Querer
        { get; set; }

        public ushort[] Value
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
        public void Update()
        {
            var res = DataTransfer.ReadWords(Querer);
            ReadComplete rc = new ReadComplete(RunWorkerCompleted);
            rc.BeginInvoke(res, null, null);
        }

        public bool WriteContext()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
