using System;

namespace Novatek.Core
{
    public class GPSData
    {
        public DateTime DateTime;
        public float Speed;
        public double Latitude;
        public double Longitude;
        public double Distance;


        private double _heading;
        public double Heading
        {
            get { return _heading; }
            set { _heading = value; }
        }
        public string HeadingString
        {
            get
            {
                var directions = new string[]
                {
                    "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N"
                };
                var index = (_heading + 23) / 45;
                return Math.Round(_heading, 1) + "° " + directions[Convert.ToInt32(index)];
            }
        }

        private bool _mph = false;
        public double SpeedAmount
        {
            get
            {
                double kph = (Speed * 3.6);
                double mph = (kph / 1.609344);

                if (_mph == true)
                {
                    if (mph < 10)
                        return Math.Round(mph, 1);
                    else
                        return Math.Round(mph, 0);
                }
                else
                {
                    if (kph < 10)
                        return Math.Round(kph, 1);
                    else
                        return Math.Round(kph, 0);
                }
            }
        }
        public String SpeedUnit
        {
            get
            {
                if (_mph == true)
                    return "MPH";
                else
                    return "KM/H";
            }
        }


        public GPSData(bool mph = false)
        {
            _mph = mph;
        }
    }
}
