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
using System.IO;
using Microsoft.Win32;
using System.Threading;

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
        private const string JOURNAL_DIRECTORY = "Journal";
        #endregion


        #region Globals
        private ObservableCollection<EventJournalItem> _eventJournal = new ObservableCollection<EventJournalItem>();
        private ObservableCollection<Picon2JournalEventRecord> _picon2EventsCollection = new ObservableCollection<Picon2JournalEventRecord>();

        private int _runoJournalSize;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _token;

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
            _token = this._cancellationTokenSource.Token;
            uiCancelImport.IsEnabled = false;

            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
            {
                uiJournal.Visibility = Visibility.Collapsed;
                uiPicon2Journal.Visibility = Visibility.Visible;

                uiJournalLengthTextBlock.Visibility = Visibility.Collapsed;
                uiToggleJournalSize.Visibility = Visibility.Collapsed;
            }
            else
            {
                uiJournal.Visibility = Visibility.Visible;
                uiPicon2Journal.Visibility = Visibility.Collapsed;
            }
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_RUNO)
            {
                _runoJournalSize = 169;
                uiJournalLengthTextBlock.Visibility = Visibility.Visible;
                uiToggleJournalSize.Visibility = Visibility.Visible;
            }
            else
            {
                uiJournalLengthTextBlock.Visibility = Visibility.Collapsed;
                uiToggleJournalSize.Visibility = Visibility.Collapsed;
            }
        }

        private void uiImport_Click(object sender, RoutedEventArgs e)
        {
            ImportJournal(_token);
        }

        private void CancelImport()
        {
            _cancellationTokenSource.Cancel();
            ShowMessage("Чтение журнала прервано.", "Внимание", MessageBoxImage.Information);
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
            uiCancelImport.IsEnabled = false;
        }

        private async Task ImportJournal(CancellationToken token)
        {
            uiCancelImport.IsEnabled = true;

            if (DeviceSelection.SelectedDevice != (int)DeviceSelectionEnum.DEVICE_PICON2)
            {
                try
                {
                    uiImport.IsEnabled = false;
                    uiClear.IsEnabled = false;
                    var valFromDevice = await ReadJournalValue(token);
                    SetJournalValue(valFromDevice);
                    this.ShowMessage("Чтение журнала системы завершено", "Чтение журнала системы",
                            MessageBoxImage.Information);
                }
                catch (SystemException ex)
                {

                }
                finally
                {
                    uiClear.IsEnabled = true;
                    uiImport.IsEnabled = true;
                }
            }
            else
            {

                await ReadJournalPicon2(token);
            }
        }

        #region Privtaes

        public async Task ReadJournalPicon2(CancellationToken token)
        {
            Picon2EventsCollection.Clear();
            ushort[] Picon2JournalReportCountUshort = new ushort[1];
            Picon2JournalReportCountUshort = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)0x4000, 1);
            byte Picon2JournalReportCountLOByte = LOBYTE(Picon2JournalReportCountUshort[0]);
            byte Picon2JournalReportCountHIByte = HIBYTE(Picon2JournalReportCountUshort[0]);

            for (int i = 0; i < Picon2JournalReportCountLOByte; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                this.Picon2EventsCollection.Add(new Picon2JournalEventRecord((await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x4300 + i), 8))));
            }
            //todo: попробовать подогнать записи журнала под одни правила для всех устройств(не получится скорее всего)
        }


        public async Task<ushort[]> ReadJournalValue(CancellationToken token)
        {
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_RUNO)
            {
                //int i = 0;
                //List<ushort> ushorts = new List<ushort>();
                //for (ushort j = 0; j < _runoJournalSize; j++)
                //{
                //    if (token.IsCancellationRequested)
                //    {
                //        return ushorts.ToArray();
                //    }

                //    ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + i), 23));
                //    SetJournalValue(ushorts.ToArray());
                //    i += 23;
                //}

                int i = 0;
                List<ushort> ushorts = new List<ushort>();
                ushort[] _journalCount = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2000), 1);
                bool lastRecord = false;
                bool cycled = false;
                try
                {
                    ushort[] _journalLastRecordAboveCount = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2000 + _journalCount[0]), 23);
                    if ((_journalLastRecordAboveCount.All(o => o == 0) || _journalLastRecordAboveCount.All(o => o == 0xffff)))
                        cycled = false;
                    else cycled = true;
                }
                catch { lastRecord = true; }

                if (cycled)
                {
                    ushort startAddress = (ushort)(0x2000 + _journalCount[0]);
                    int journalDelta = _runoJournalSize - (_runoJournalSize - (_journalCount[0] - 1) / 23);

                    for (int j = journalDelta; j < _runoJournalSize; j++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return ushorts.ToArray();
                        }
                        ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(startAddress + i), 23));
                        SetJournalValue(ushorts.ToArray());
                        i += 23;
                    }
                    i = 0;
                    for (int j = 0; j < journalDelta; j++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return ushorts.ToArray();
                        }
                        ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + i), 23));
                        SetJournalValue(ushorts.ToArray());
                        i += 23;
                    }


                }
                else
                {
                    int journalDelta = _runoJournalSize - (_journalCount[0] - 1) / 23;
                    for (int j = 0; j < journalDelta; j++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return ushorts.ToArray();
                        }
                        ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + i), 23));
                        SetJournalValue(ushorts.ToArray());
                        i += 23;
                    }

                }


                return ushorts.ToArray();

            }
            else
            {
                _runoJournalSize = 199;
                int i = 0;
                List<ushort> ushorts = new List<ushort>();
                ushort[] _journalCount = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2000), 1);
                bool lastRecord = false;
                bool cycled = false;
                try
                {
                    ushort[] _journalLastRecordAboveCount = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2000 + _journalCount[0]), 23);
                    if ((_journalLastRecordAboveCount.All(o => o == 0) || _journalLastRecordAboveCount.All(o => o == 0xffff)))
                        cycled = false;
                    else cycled = true;
                }
                catch { lastRecord = true; }

                if (cycled)
                {
                    ushort startAddress = (ushort)(0x2000 + _journalCount[0]);
                    int journalDelta = _runoJournalSize - (_runoJournalSize - (_journalCount[0] - 1) / 23);

                    for (int j = journalDelta; j < _runoJournalSize; j++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return ushorts.ToArray();
                        }
                        ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(startAddress + i), 23));
                        SetJournalValue(ushorts.ToArray());
                        i += 23;
                    }
                    i = 0;
                    for (int j = 0; j < journalDelta; j++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return ushorts.ToArray();
                        }
                        ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + i), 23));
                        SetJournalValue(ushorts.ToArray());
                        i += 23;
                    }


                }
                else
                {
                    int journalDelta = _runoJournalSize - (_runoJournalSize - (_journalCount[0] - 1) / 23);
                    for (int j = 0; j < journalDelta; j++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return ushorts.ToArray();
                        }
                        ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + i), 23));
                        SetJournalValue(ushorts.ToArray());
                        i += 23;
                    }

                }


                //for (ushort j = 0; j < _runoJournalSize; j++)
                //{
                //    if (token.IsCancellationRequested)
                //    {
                //        return ushorts.ToArray();
                //    }
                //    ushorts.AddRange(await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x2001 + i), 23));
                //    SetJournalValue(ushorts.ToArray());
                //    i += 23;
                //}

                return ushorts.ToArray();
            }
        }

        private void ReadJournalExecute(int size = 0)
        {

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

                for (int i = 0; i < value.Length / 23; i++)
                {
                    tmp.Add(new EventJournalItem(new String(longMessage.GetRange(i * 46, 46).ToArray())));
                }
                //tmp.Sort(delegate (EventJournalItem first, EventJournalItem second)
                //{
                //    if ((double)first.JournalDateTime.Ticks / TimeSpan.TicksPerSecond >
                //        (double)second.JournalDateTime.Ticks / TimeSpan.TicksPerSecond)
                //        return -1;
                //    if ((double)first.JournalDateTime.Ticks / TimeSpan.TicksPerSecond <
                //        (double)second.JournalDateTime.Ticks / TimeSpan.TicksPerSecond)
                //        return 1;
                //    else
                //        return 0;
                //});
                foreach (var item in tmp)
                {
                    //если из памяти читает нули - в eventmessage пишется "null", ее мы не выводим
                    if (!item.EventMessage.Equals("null"))
                        this.EventJournal.Add(item);
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

                    this.ReadJournalValue(_token);
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
                    try
                    {
                        await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x4000, new ushort[] { 0 });
                        ClearCompleted.Invoke();
                        MessageBox.Show("Журнал очищен", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch { }
                }
                else if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON_GS)
                {
                    ClearPiconGSJournal();
                }
                else
                {
                    ClearRunoJournal();
                }
            }
            else
            {
                MessageBox.Show("Неверный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
        private async void ClearRunoJournal()
        {

            try
            {
                uiClear.IsEnabled = false;
                uiImport.IsEnabled = false;
                uiClear.Content = "Ожидайте";

                await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x2000, new ushort[] { 1 });

                ushort[] zeros = new ushort[23];
                int i = 0;
                for (ushort j = 0; j < _runoJournalSize; j++)
                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(0x2001 + i), zeros);
                    i += 23;
                }
                ClearCompleted.Invoke();
                ShowMessage("Очистка журнала завершена", "Выполнено", MessageBoxImage.Information);
            }
            catch { }
            finally
            {
                ClearCompleted.Invoke();
                uiClear.IsEnabled = true;
                uiImport.IsEnabled = true;
                uiClear.Content = "Очистить";

            }

        }
        private async void ClearPiconGSJournal()
        {
            try
            {
                uiClear.IsEnabled = false;
                uiImport.IsEnabled = false;
                uiClear.Content = "Ожидайте";
                bool _easyClear = await GetPiconGSSignature();
                if (_easyClear)
                {
                    try
                    {
                        await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x2000, new ushort[] { 0 });
                        return;
                    }
                    catch { }
                }
                await RTUConnectionGlobal.SendDataByAddressAsync(1, 0x2000, new ushort[] { 1 });
                _runoJournalSize = 199;
                ushort[] zeros = new ushort[23];
                int i = 0;
                for (ushort j = 0; j < _runoJournalSize; j++)
                {
                    await RTUConnectionGlobal.SendDataByAddressAsync(1, (ushort)(0x2001 + i), zeros);
                    i += 23;
                }
                ClearCompleted.Invoke();
                ShowMessage("Очистка журнала завершена", "Выполнено", MessageBoxImage.Information);
            }
            catch { }
            finally
            {
                ClearCompleted.Invoke();
                uiClear.IsEnabled = true;
                uiImport.IsEnabled = true;
                uiClear.Content = "Очистить";
            }







        }

        private void uiToggleJournalSize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (uiToggleJournalSize.IsChecked == true) _runoJournalSize = 249;
                else _runoJournalSize = 169;
            }
            catch (Exception ex) { };
        }

        private async Task<bool> GetPiconGSSignature()
        {
            try
            {
                ushort[] res = await RTUConnectionGlobal.GetDataByAddress(1, (ushort)(0x0400), 20);
                return ReadSignatureComplete(res);
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        private bool ReadSignatureComplete(ushort[] value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return GetSignatureString((value).ToList());
            }
        }
        private bool GetSignatureString(List<ushort> value)
        {
            ushort[] deviceName = value.GetRange(0, 5).ToArray();
            //убираем лишний символ
            deviceName[4] = (ushort)(LOBYTE(deviceName[4]));
            ushort[] version = value.GetRange(8, 3).ToArray();

            var devName = "Имя устройства: " + Converter.GetStringFromWords(deviceName) + ";\r\n";

            return CheckVersionUpdateAvailability(
                Converter.GetStringFromWords(deviceName),
                (byte)(version[1] >> 8),
                ((byte)version[1])
                                                );
        }

        private bool CheckVersionUpdateAvailability(string deviceName, byte versionFirstByte, byte versionSecondByte)
        {
            if (!deviceName.ToLower().Contains("gs")) return false;

            if (Convert.ToInt16(versionFirstByte) > 5) return true;
            else if (Convert.ToInt16(versionFirstByte) == 5)
            {
                if (Convert.ToInt16(versionSecondByte) > 0) return true;
                else return false;
            }
            else return false;

        }

        private void uiSaveJournal_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
                SaveJournalExecute(true);
            else SaveJournalExecute(false);
        }

        private void SaveJournalExecute(bool _isPicon2)
        {
            string _deviceName = "";
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON2)
                _deviceName = "Picon2";
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_PICON_GS)
                _deviceName = "PiconGS2";
            if (DeviceSelection.SelectedDevice == (byte)DeviceSelectionEnum.DEVICE_RUNO)
                _deviceName = "Runo";

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.RestoreDirectory = false;
            sfd.FileName = DateTime.Now.ToShortDateString() + " " + _deviceName;
            if (sfd.ShowDialog().Value)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(sfd.FileName, FileMode.OpenOrCreate)))
                    {
                        if (!_isPicon2)
                        {
                            foreach (var item in this.EventJournal)
                            {
                                writer.Write(item.EventDate + " ");
                                writer.Write(item.EventTime + " ");
                                writer.WriteLine(item.EventMessage);
                            }
                        }
                        else
                        {
                            foreach (var item in this.Picon2EventsCollection)
                            {
                                writer.Write(item.Date + " ");
                                writer.Write(item.Time + " ");
                                writer.WriteLine(item.JournalRecord);
                            }
                        }
                        writer.Close();
                    }
                }
                catch (Exception)
                {
                    ShowMessage("При сохранении журнала возникла ошибка.", "", MessageBoxImage.Information);
                }
                finally
                {
                    ShowMessage("Журнал сохранен.", "", MessageBoxImage.Information);
                }
            }
        }

        private void uiCancelImport_Click(object sender, RoutedEventArgs e)
        {
            CancelImport();
        }
    }

}

