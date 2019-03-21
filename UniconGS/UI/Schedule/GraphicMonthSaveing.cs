using System;
using System.Xml.Serialization;

namespace UniconGS.UI.Schedule
{
    [Serializable]
    public class GraphicMonthSaveing
    {
        private GraphicTime _turnOnTime ;
        private GraphicTime _turnOffTime ;

        [XmlElement]
        public GraphicTime TurnOnTime
        {
            get
            {
                return this._turnOnTime;
            }
            set
            {
                this._turnOnTime = value;
            }
        }
        [XmlElement]
        public GraphicTime TurnOffTime
        {

            get
            {
                return this._turnOffTime;
            }

            set
            {
                this._turnOffTime = value;
            }

        }
        public GraphicMonthSaveing()
        {
            this.TurnOffTime = new GraphicTime(0xff,0xff);
            this.TurnOnTime = new GraphicTime(0xff,0xff);
        }
        public GraphicMonthSaveing(GraphicTime turnOnTime, GraphicTime turnOffTime)
        {
            this.TurnOffTime = turnOffTime;
            this.TurnOnTime = turnOnTime;
        }
    }
}
