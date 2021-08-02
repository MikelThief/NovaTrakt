using System;

namespace Novatek.Helpers
{
    class GPSHelpers
    {

        // Change the format of the coordinates found in the video file
        public static float FixCoordinates(string hemisphere, float coordinate)
        {
            // Novatek stores coordinates in DDDmm.mmmm format
            // This function converts them to tradition lat/long decimals
            string coordinates = coordinate.ToString("R");

            string[] parts = coordinates.Split('.');

            if (parts.Length == 2)
                coordinates = parts[0].PadLeft(5, '0') + "." + parts[1].PadRight(4, '0');
            else
                coordinates = parts[0].PadLeft(5, '0') + ".0000";

            int degrees = Convert.ToInt16(coordinates.Substring(0, 3));
            float minutes = Convert.ToSingle(coordinates.Substring(3));

            float t = degrees + (minutes / 60);

            if (hemisphere == "S" || hemisphere == "W")
            {
                return -1 * t;
            }
            else
            {
                return t;
            }
        }

        // Get the distance between two coordinates (in Metres)
        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) + Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) * Math.Cos(Deg2rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2deg(dist);
            dist = dist * 60 * 1.1515;
            return dist;
        }

        // Get the heading between two coordinates (in Degrees)
        public static double Heading(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = Deg2rad(lon2 - lon1);
            var dPhi = Math.Log(
                Math.Tan(Deg2rad(lat2) / 2 + Math.PI / 4) / Math.Tan(Deg2rad(lat1) / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
            return ToBearing(Math.Atan2(dLon, dPhi));
        }


        //  This function converts decimal degrees to radians
        private static double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //  This function converts radians to decimal degrees
        private static double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        //  This function converts radians to degrees (as bearing: 0...360)
        private static double ToBearing(double radians)
        {
            return (Rad2deg(radians) + 360) % 360;
        }
    }
}
