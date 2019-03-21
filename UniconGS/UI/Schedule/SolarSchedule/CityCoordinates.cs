using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniconGS.UI.Schedule.SolarSchedule
{
    public struct CityCoordinates
    {
        public double Latitude;
        public double Longitude;

        public CityCoordinates(double _lat, double _long)
        {
            Latitude = _lat;
            Longitude = _long;
        }
    }

    
}
