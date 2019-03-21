using System;
using System.Xml.Serialization;

namespace UniconGS.UI.Schedule
{
    [Serializable]
    public class GraphicDay
    {
        [XmlElement]
        public string Number { get; set; }
        [XmlElement]
        public GraphicTime TurnOnTime { get; set; }
        [XmlElement]
        public GraphicTime TurnOffTime { get; set; }

        public bool isVisible = true;

        public GraphicDay(string number, GraphicTime turnOnTime, GraphicTime turnOffTime)
        {
            this.Number = number;
            this.TurnOnTime = turnOnTime;
            this.TurnOffTime = turnOffTime;
        }

        public GraphicDay(string number)
        {
            this.Number = number;
            this.TurnOffTime = new GraphicTime(0xff, 0xff);
            this.TurnOnTime = new GraphicTime(0xff, 0xff);
        }

        public GraphicDay()
        { }
    }
}
