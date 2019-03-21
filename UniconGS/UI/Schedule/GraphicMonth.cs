using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace UniconGS.UI.Schedule
{
    [Serializable]
    public class GraphicMonth
    {
        private string _name = string.Empty;

        public string MonthName
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }
        [XmlElement]
        public int Number { get; set; }
        [XmlArray("Day")]
        public List<GraphicDay> Days { get; set; }
        [XmlElement]
        public GraphicMonthSaveing MonthSaving { get; set; }

        public GraphicMonth(string name, int number, List<GraphicDay> days, GraphicMonthSaveing monthSaving)
        {
            this.MonthName = name;
            this.Number = number;
            this.Days = days;
            this.MonthSaving = monthSaving;
        }

        public GraphicMonth(string name, int number, bool? is31DayMonth)
        {
            this.MonthName = name;
            this.Number = number;
            this.Days = new List<GraphicDay>(32);
            if (is31DayMonth.HasValue)
            {

                if (is31DayMonth.Value)
                {
                    for (int i = 0; i < 31; i++)
                    {
                        this.Days.Add(new GraphicDay((i + 1).ToString() + " Число"));
                    }
                }
                else
                {
                    for (int i = 0; i < 30; i++)
                    {
                        this.Days.Add(new GraphicDay((i + 1).ToString() + " Число"));
                    }
                    this.Days.Add(new GraphicDay((30 + 1).ToString() + " Число") { isVisible = false });
                }
            }
            else
            {
                for (int i = 0; i < 29; i++)
                {
                    this.Days.Add(new GraphicDay((i + 1).ToString() + " Число"));
                }
                this.Days.Add(new GraphicDay((29 + 1).ToString() + " Число") { isVisible = false });
                this.Days.Add(new GraphicDay((30 + 1).ToString() + " Число") { isVisible = false });
            }
            this.MonthSaving = new GraphicMonthSaveing(new GraphicTime(0, 0), new GraphicTime(0, 0));
        }

        public GraphicMonth()
        { }
    }
}
