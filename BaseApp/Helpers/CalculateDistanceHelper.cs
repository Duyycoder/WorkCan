using BaseApp.Constants;

namespace BaseApp.Helpers
{
    public class CalculateDistanceHelper
    {
        // get distance formula
        public double GetDistanceFromLatLonInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = Deg2Rad(lat2 - lat1);
            double dLon = Deg2Rad(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = CommonConstants.EARTH_RADIUS_BY_METER * c; // distance by meters

            return distance;
        }


        private double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

    }
}
