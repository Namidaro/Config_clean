using System;
using System.Windows;
using UniconGS.Source;

namespace UniconGS.UI.SimpleIBaseControl
{
    public class ResetController: IQuery
    {
        private delegate void WriteCompleteDelegate(bool res);
        public ResetController()
        {
            ReadData = false;
            Querer = new Slot(0x0302, 1, "PLC Reset Command");
            Querer.Value = new ushort[] { 1 };
        }

        private void WriteComplete(bool res)
        {
            if(res)
            {
                MessageBox.Show("Контроллер был успешно сброшен.", "Внимание", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Во время сброса контроллера произошла ошибка.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #region IQueryMember
        public Slot Querer
        { get; set; }

        public ushort[] Value
        { get; set; }

        public bool ReadData
        { get; set;}

        public void Update()
        {
            throw new NotImplementedException();
        }

        public bool WriteContext()
        {
            var res = DataTransfer.WriteWord(Querer);
            WriteCompleteDelegate writeComplete = new WriteCompleteDelegate(WriteComplete);
            writeComplete.BeginInvoke(res, null, null);
            return res;
        }
        #endregion
    }
}
