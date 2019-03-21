using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using UniconGS.Interfaces;
using UniconGS.Source;
using UniconGS.Enums;
using UniconGS.UI.Picon2;
using System.Text;


namespace UniconGS.UI.Journal
{
    /// <summary>
    /// Interaction logic for SystemJournal.xaml
    /// </summary>
    public partial class SystemJournal : UserControl, IUpdatableControl
    {
        #region Events
        public delegate void ShowMessageEventHandler(string message, string caption, MessageBoxImage image);
        public event ShowMessageEventHandler ShowMessage;

        public delegate void ClearInProgressEventHandler();
        public event ClearInProgressEventHandler ClearInProcess;

        public delegate void ClearCompletedEventHandler();
        public event ClearCompletedEventHandler ClearCompleted;

        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();

        private delegate void ReadComplete(ushort[] res);
        #endregion

        #region [CONST]
        private const ushort PICON2_JOURNAL_RECORD_COUNT_ADDRESS = 0x6000;
        //private const ushort PICON2_JOURNAL_
        private const ushort PICON2_JOURNAL_STRING_SIZE = 0x0;//?????????????????????? 

        #endregion


        #region Globals
        private ObservableCollection<EventJournalItem> _eventJournal = new ObservableCollection<EventJournalItem>();
        private ObservableCollection<Picon2JournalEventRecord> _picon2EventsCollection = new ObservableCollection<Picon2JournalEventRecord>();
        #endregion

        public void SetAutonomous()
        {
            this.uiImport.IsEnabled = false;
            this.uiClear.IsEnabled = false;
        }

        public void DisableAutonomous()
        {
            this.uiImport.IsEnabled = true;
            this.uiClear.IsEnabled = true;
        }

        public ObservableCollection<EventJournalItem> EventJournal
        {
            get
            {
                return this._eventJournal;
            }
            set
            {
                this._eventJournal = value;
            }
        }
        public ObservableCollection<Picon2JournalEventRecord> Picon2EventsCollection
        {
            get
            {
                return this._picon2EventsCollection;
            }
            set
            {
                this._picon2EventsCollection = value;
            }
        }


        public SystemJournal()
        {


            InitializeComponent();



            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
            {
                uiJournal.Visibility = Visibility.Collapsed;
                uiPicon2Journal.Visibility = Visibility.Visible;
            }
            else
            {
                uiJournal.Visibility = Visibility.Visible;
                uiPicon2Journal.Visibility = Visibility.Collapsed;
            }
            //if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            //{
            //    uiImport.IsEnabled = false;

            //}

            //else
            //{
            //    uiImport.IsEnabled = true;

            //}
        }

        private void uiImport_Click(object sender, RoutedEventArgs e)
        {
            ImportJournal();
        }

        private async void ImportJournal()
        {
            if (DeviceSelection.SelectedDevice != (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                try
                {
                    uiImport.IsEnabled = false;
                    var valFromDevice = await ReadJournalValue();
                    SetJournalValue(valFromDevice);
                }



                //var result = RTUConnectionGlobal.ModbusMaster.ReadInputRegistersAsync(1, 0x2001, 3910);
                catch (SystemException ex)
                {

                }
                finally
                {
                    uiImport.IsEnabled = true;
                }
            }
            else
            {

                await ReadJournalPicon2();
                //ShowMessage("Функция не реализована!", "Внимание", MessageBoxImage.Information);
            }
        }

        #region Privtaes

        public async Task ReadJournalPicon2()
        {

            Picon2EventsCollection.Clear();
            ushort[] Picon2JournalReportCountUshort = new ushort[1];
            Picon2JournalReportCountUshort = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)0x4000, 1);
            byte Picon2JournalReportCountLOByte = LOBYTE(Picon2JournalReportCountUshort[0]);
            byte Picon2JournalReportCountHIByte = HIBYTE(Picon2JournalReportCountUshort[0]);

            for (int i = 0; i < Picon2JournalReportCountLOByte; i++)
            {
                this.Picon2EventsCollection.Add(new Picon2JournalEventRecord(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x4300 + i), 8)));
            }
            //todo: попробовать подогнать записи журнала под одни правила для всех устройств(не получится скорее всего)
        }


        public async Task<ushort[]> ReadJournalValue()
        {
            //List<ushort> ushorts = new List<ushort>();
            //for (ushort i = 0; i < 3900; i += 100)
            //{
            //    ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + i), 100));
            //}
            //ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, 0x2001 + 3900, 10));

            //return ushorts.ToArray();

            List<ushort> ushorts = new List<ushort>();
            for (ushort i = 0; i < 5635; i += 92)
            {
                ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + i), 92));
            }
            ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + 5704), 23));


            return ushorts.ToArray();
        }

        private void SetJournalValue(ushort[] value)
        {
            try
            {
                this.EventJournal.Clear();
                var longMessage = Converter.GetStringWithNullsFromWords(value).ToCharArray().ToList();
                /*Разделить строку на подстроки и составить список*/
                List<EventJournalItem> tmp = new List<EventJournalItem>();
                //var str = string.Empty;
                for (int i = 0; i < 249; i++)
                {
                    tmp.Add(new EventJournalItem(new String(longMessage.GetRange(i * 46, 46).ToArray())));
                }
                tmp.Sort(delegate (EventJournalItem first, EventJournalItem second)
                {
                    if ((double)first.JournalDateTime.Ticks / TimeSpan.TicksPerSecond >
                        (double)second.JournalDateTime.Ticks / TimeSpan.TicksPerSecond)
                        return -1;
                    if ((double)first.JournalDateTime.Ticks / TimeSpan.TicksPerSecond <
                        (double)second.JournalDateTime.Ticks / TimeSpan.TicksPerSecond)
                        return 1;
                    else
                        return 0;
                });
                foreach (var item in tmp)
                {
                    //если из памяти читает нули - в eventmessage пишется "null", ее мы не выводим
                    if (!item.EventMessage.Equals("null"))
                        this.EventJournal.Add(item);
                }
                if (this.ShowMessage != null)
                {
                    if (EventJournal.Count != 0)
                    {
                        this.ShowMessage("Чтение журнала системы прошло успешно", "Чтение журнала системы",
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        this.ShowMessage("Журнал системы пуст.", "Чтение журнала системы", MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception e)
            {
                if (this.ShowMessage != null)
                {
                    this.ShowMessage("Во время чтения журнала системы произошла ошибка", "Чтение журнала системы",
                        MessageBoxImage.Error);
                }
            }
        }
        #endregion
        /// <summary>
        /// Изменение последовательности байт с преобразованием в слова
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public static ushort TOWORD(byte high, byte low)
        {
            UInt16 ret = (UInt16)high;
            return (ushort)((ushort)(ret << 8) + (ushort)low);
        }
        /// <summary>
        /// Возвращает младший байт слова
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Мл.байт</returns>
        public static byte LOBYTE(int v)
        {
            return (byte)(v & 0xff);
        }
        /// <summary>
        /// Возвращает старший байт слова.
        /// </summary>
        /// <param name="v">Слово.</param>
        /// <returns>Ст.байт</returns>
        public static byte HIBYTE(int v)
        {
            return (byte)(v >> 8);
        }
        /// <summary>
        /// Форматирование строки даты и время счетчика
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ByteArrayToString(byte[] value)
        {
            ushort[] arr = new ushort[8];
            byte[] values = new byte[16];
            Array.ConstrainedCopy(value, 0, values, 0, 16);
            var j = 0;
            for (int i = 0; i < values.Length / 2; i++)
            {
                arr[i] = TOWORD(values[j], values[j + 1]);
                j += 2;
            }
            string tmp = string.Empty;
            int a = 0;
            foreach (var item in arr)
            {
                foreach (var @byte in BitConverter.GetBytes(item))
                {
                    if (@byte == 0)
                    {
                        break;
                    }
                    else
                    {
                        tmp += Convert.ToChar(@byte);
                    }
                }
                a++;
            }
            //if ((_index == 0) && (tmp.Length >= 4)) //Индекс даты
            //{
            //    tmp = tmp.Remove(0, 3);
            //}
            return tmp;
        }



        public ushort[] Value { get; set; }

        public async Task Update()
        {
            if (DeviceSelection.SelectedDevice == (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                try
                {
                    //ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x6000, 2);
                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    SetJournalValue(value);

                    //});

                    //this.ReadJournalValue();
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Error", MessageBoxImage.Error);
                }
            }
            else
            {
                try
                {
                    ushort[] value = await RTUConnectionGlobal.GetDataByAddress(1, 0x2001, 5727);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetJournalValue(value);

                    });

                    this.ReadJournalValue();
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message, "Error", MessageBoxImage.Error);
                }
            }
            //if (this.StopWork != null)
            //{
            //    this.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(() =>
            //    {
            //        StopWork();
            //        StateTextBlock.Text = string.Empty;
            //    }));
            //}



        }

        private void uiClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearJournal();
            }
            catch (Exception) { }
            //ClearCompleted.Invoke();
        }
        private async void ClearJournal()
        {
            var win = new JournalPassword();
            win.ShowDialog();

            if (win.DialogResult == true)
            {

                ClearInProcess.Invoke();

                if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x4000, new ushort[] { 0 });
                    ClearCompleted.Invoke();
                    MessageBox.Show("Журнал очищен", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                }
                else if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON_GS)
                {
                    ClearPiconGSJournal();
                }
                else
                {
                    ClearRunoJournal();
                }


                //Application.Current.MainWindow.Cursor = Cursors.Arrow;
            }
            else
            {
                MessageBox.Show("Неверный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
        private async void ClearRunoJournal()
        {
            uiClear.IsEnabled = false;
            uiClear.Content = "Ожидайте";



            await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x2000, new ushort[] { 1 });

            ushort[] zeros = new ushort[92];
            for (ushort i = 0; i < 5635; i += 92)
            {
                await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(0x2001 + i), zeros);
            }
            await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(0x2001 + 5704), new ushort[23]);
            MessageBox.Show("Журнал очищен", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);

            uiClear.IsEnabled = true;
            uiClear.Content = "Очистить";

            ClearCompleted.Invoke();

        }
        private async void ClearPiconGSJournal()
        {
            uiClear.IsEnabled = false;
            uiClear.Content = "Ожидайте";



            await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x2000, new ushort[] { 1 });

            ushort[] zeros = new ushort[100];
            for (ushort i = 0; i < 4600; i += 100)
            {
                await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(0x2001 + i), zeros);
            }
            MessageBox.Show("Журнал очищен", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);

            uiClear.IsEnabled = true;
            uiClear.Content = "Очистить";

            ClearCompleted.Invoke();

        }
    }

}

