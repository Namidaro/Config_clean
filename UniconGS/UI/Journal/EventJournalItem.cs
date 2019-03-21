using System;
using System.Text.RegularExpressions;

namespace UniconGS.UI.Journal
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
                this.EventMessage = "null";
            }
        }
    }
}
