using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UniconGS.Source;

namespace UniconGS.UI.HeatingSchedule
{
    /// <summary>
    /// Interaction logic for HeatingSchedule.xaml
    /// </summary>
    public partial class HeatingSchedule : UserControl
    {
        #region Event
        public delegate void ShowMessageEventHandler(string message, string caption, MessageBoxImage image);
        public event ShowMessageEventHandler ShowMessage;

        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();
        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;

        private delegate void ReadComplete(ushort[] res);
        private delegate void WriteComplete(bool res);
        #endregion

        #region Globals
        private Heating _value = new Heating();
        #endregion

        public Heating HeatingValue
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                this.UpdateBinding();
            }
        }

        public HeatingSchedule()
        {
            InitializeComponent();
            this.UpdateBinding();
        }

        public void SetAutonomous()
        {
            this.uiExport.IsEnabled = false;
            this.uiImport.IsEnabled = false;
        }

        public void DisableAutonomous()
        {
            this.uiExport.IsEnabled = true;
            this.uiImport.IsEnabled = true;
        }

        private async void uiImport_Click(object sender, RoutedEventArgs e)
        {
          await  UpdateState();
            if (this.ShowMessage != null)
            {
                this.ShowMessage("Чтение графика обогрева прошло успешно",
                    "Чтение графика обогрева", MessageBoxImage.Information);
            }
        }

        private void ImportComplete(ushort[] value)
        {
            if (value != null)
            {
                this.Value = value;
              
            }
            else
            {
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время чтения графика обогрева из устройства произошла ошибка.",
                        "Чтение графика обогрева", MessageBoxImage.Error);
                }
            }
            uiExport.IsEnabled = uiImport.IsEnabled = true;
        }


        public async Task UpdateState()
        {

            uiExport.IsEnabled = uiImport.IsEnabled = false;

            ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x9108, 2);   

           
            ImportComplete(value);

            uiExport.IsEnabled = uiImport.IsEnabled = true;

        }

        private async void uiExport_Click(object sender, RoutedEventArgs e)
        {
           await WriteAll();

            if (this.ShowMessage != null)
            {
                this.ShowMessage("Запись графика обогрева в устройство прошла успешно.",
                    "Запись графика обогрева в устройство", MessageBoxImage.Information);
            }

        }

        public void ExportComplete(bool res)
        {
            if (res)
            {
            }
            else
            {
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время записи графика обогрева в устройство произошла ошибка.",
                        "Запись графика обогрева в устройство", MessageBoxImage.Error);
                }
            }
            uiExport.IsEnabled = uiImport.IsEnabled = true;
        }


        public async Task WriteAll()
        {
            uiExport.IsEnabled = uiImport.IsEnabled = false;
            {
               await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x9108, Value);             
            }
            ExportComplete(true);
            uiExport.IsEnabled = uiImport.IsEnabled = true;
        }

        private void UpdateBinding()
        {
            this.PART_TURNOFF.DataContext = this._value.TurnOff;
            this.PART_TURNON.DataContext = this._value.TurnOn;
        }

        #region IQueryMember
     
        public bool ReadData
        { get; set; }

        public ushort[] Value
        {
            get
            {
                return this._value.GetValue();
            }
            set
            {
                if (value != null)
                {
                    this._value.SetValue(value);
                    this.UpdateBinding();
                }
                else
                {
                    this._value = new Heating();
                    this.UpdateBinding();
                }
            }
        }

     

       
        #endregion
    }
}
