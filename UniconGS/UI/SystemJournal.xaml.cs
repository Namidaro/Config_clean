using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using UniconGS.Source;
using System.Text.RegularExpressions;

namespace UniconGS.UI
{
    public class EventJournalItem
    {
        public string EventMessage { get; set; }
        public string EventDate { get; set; }
        public string EventTime { get; set; }

        public DateTime JournalDateTime { get; set; }

        public EventJournalItem(string message)
        {
            /*Разбор регулярным выражением*/
            Regex regex = new Regex(@"\<(?<Date>.+)\>\<(?<Time>.+)\>\<(?<Message>.+)\>");
            Match m = regex.Match(message);
            if (m.Success)
            {
                this.EventDate = m.Groups["Date"].Value;
                this.EventTime = m.Groups["Time"].Value;
                this.EventMessage = m.Groups["Message"].Value;

                try
                {
                    IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
                    DateTime f1 = DateTime.ParseExact(EventDate, "dd/MM/yyyy", culture);
                    DateTime f2 = DateTime.Parse(EventTime, culture, System.Globalization.DateTimeStyles.AssumeLocal);
                    this.JournalDateTime = new DateTime(f1.Year, f1.Month, f1.Day, f2.Hour, f2.Minute, f2.Second);
                }
                catch (Exception)
                {
                    this.EventDate = string.Empty;
                    this.EventTime = string.Empty;
                    this.JournalDateTime = new DateTime();
                    this.EventMessage = "Ошибка преобразования сообщения";
                }
            }
            else
            {
                this.EventMessage = "Ошибка преобразования сообщения";
            }
        }
    }

    /// <summary>
    /// Interaction logic for SystemJournal.xaml
    /// </summary>
    public partial class SystemJournal : UserControl, IBaseControl
    {
        #region Events
        public delegate void ShowMessageEventHandler(string message, string caption);
        public event ShowMessageEventHandler ShowMessage;

        public delegate void StartWorkEventHandler();
        public delegate void StopWorkEventHandler();

        public event StartWorkEventHandler StartWork;
        public event StopWorkEventHandler StopWork;
        #endregion

        #region Globals
        private ObservableCollection<EventJournalItem> _eventJournal = new ObservableCollection<EventJournalItem>();
        private Slot _querer = null;
        #endregion

        public void SetAutonomous()
        {
            this.uiImport.IsEnabled = false;
        }

        public void DisableAutonomous()
        {
            this.uiImport.IsEnabled = true;
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

        public SystemJournal()
        {
            InitializeComponent();
            this.uiImport.Click += new RoutedEventHandler(uiImport_Click);
        }

        private void uiImport_Click(object sender, RoutedEventArgs e)
        {
            if (this.StopUpdate != null && this.StartUpdate != null)
            {
                if (this.StartWork != null)
                    this.StartWork();
                this.StopUpdate();
                var result = this.SetJournalValue();
                this.StartUpdate();
                if (this.StopWork != null)
                    this.StopWork();
                if (result == null)
                {
                    if (this.ShowMessage != null)
                    {
                        this.ShowMessage("Журнал системы пуст.", "Чтение журнала системы");
                    }
                }
                else
                {
                    if (Convert.ToBoolean(result))
                    {
                        if (this.ShowMessage != null)
                        {
                            this.ShowMessage("Чтение журнала системы прошло успешно", "Чтение журнала системы");
                        }
                    }
                    else
                    {
                        if (this.ShowMessage != null)
                        {
                            this.ShowMessage("Во время чтения журнала системы произошла ошибка", "Чтение журнала системы");
                        }
                    }
                }
            }
        }

        #region Privtaes

        private bool? SetJournalValue()
        {
            if (this._querer != null)
            {
                var result = DataTransfer.ReadWords(ref this._querer);
                if (result)
                {
                    return this.SetJournalValue((ushort[])this._querer.Value, this._querer.Size);

                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        private bool SetJournalValue(ushort[] value, int count)
        {
            try
            {
                this.EventJournal.Clear();
                var longMessage = Converter.GetStringWithNullsFromWords(value).ToCharArray().ToList();
                /*Разделить строку на подстроки и составить список*/
                List<EventJournalItem> tmp = new List<EventJournalItem>();
                //var str = string.Empty;
                for (int i = 0; i < 170; i++)
                {
                    tmp.Add(new EventJournalItem(new String(longMessage.GetRange(i * 46, 46).ToArray())));
                }
                tmp.Sort(delegate(EventJournalItem first, EventJournalItem second)
                {
                    if ((double)first.JournalDateTime.Ticks / TimeSpan.TicksPerSecond > (double)second.JournalDateTime.Ticks / TimeSpan.TicksPerSecond)
                        return -1;
                    if ((double)first.JournalDateTime.Ticks / TimeSpan.TicksPerSecond < (double)second.JournalDateTime.Ticks / TimeSpan.TicksPerSecond)
                        return 1;
                    else
                        return 0;
                });
                foreach (var item in tmp)
                {
                    this.EventJournal.Add(item);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region IBaseControl Members

        public event Commands.StartUpdateEventHandler StartUpdate;

        public event Commands.StopUpdateEventHandler StopUpdate;

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

        public object Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public bool WriteContext()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBaseControl Members


        public void CancelUpdate()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
