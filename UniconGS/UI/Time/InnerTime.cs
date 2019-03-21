
namespace UniconGS.UI.Time
{
    public class InnerTime
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public int? Second { get; set; }

        public InnerTime(int? year, int? month, int? day, int? hour, int? minute, int? second)
        {
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Month = month;
            this.Second = second;
            this.Year = year;
        }
    }
}
