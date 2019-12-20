using System;
using Android.Util;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Xml;

namespace Asphalt__Gradcoop
{
    public interface ILocation
    {
        string Name { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        string GetLatLongString();
        bool SetLatLongString(string latLongString);
    }

    public class Location : ILocation
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string GetLatLongString() {
            return Latitude.ToString(CultureInfo.InvariantCulture) + "," + Longitude.ToString(CultureInfo.InvariantCulture);
        }
        public bool SetLatLongString(string latLongString)
        {
            string[] strArr = latLongString.Split(',');
            if (strArr.Length != 2)
                return false;

            Latitude = Constants.GetInvariantCultureDouble(strArr[0]);
            Longitude = Constants.GetInvariantCultureDouble(strArr[1]);

            if (Math.Abs(Latitude) > 90 || Math.Abs(Longitude) > 180 || Latitude == double.NaN || Longitude == double.NaN)
            {
                //Stop user from proceeding with wrong lat, long
                Latitude = double.NaN;
                Longitude = double.NaN;

                return false;
            }

            return true;
        }
    }

    static class LocationProxy
    {
        public static bool IsLocationAvailable()
        {
            if (!CrossGeolocator.IsSupported)
                return false;
            
            return CrossGeolocator.Current.IsGeolocationAvailable;
        }

        public static string GetLatLongString(Position location)
        {
            return location.Latitude.ToString(CultureInfo.InvariantCulture) + "," + location.Longitude.ToString(CultureInfo.InvariantCulture);
        }

        public static async Task<Position> GetCurrentPosition()
        {
            Position position = null;

            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                position = await locator.GetLastKnownLocationAsync().ConfigureAwait(false);

                if (position != null)
                {
                    //got a cahched position, so let's use it.
                    return position;
                }

                //position = await locator.GetPositionAsync(TimeSpan.FromSeconds(2), cts.Token).ConfigureAwait(false);
                return null;
            }
            catch (Exception ex)
            {
                Log.WriteLine(0, "error", "Unable to get location: " + ex);
                return position;
            }
        }

        public static double CalculateRouteDistance(ILocation start, ILocation end) {
            XmlDocument doc = new XmlDocument();
            string url = @"http://dev.virtualearth.net/REST/V1/Routes/Driving?o=xml&wp.0="+ start.GetLatLongString() + "&wp.1=" + end.GetLatLongString() + "&key=AhO54klItEw6LzqrWf7r-DEX_YvJjqmiQWVJCZbyQrsR766itRfBokuW_7OnBs6d";

            doc.Load(url);

            var nodes = doc.SelectNodes("//*");

            foreach (XmlNode node in nodes)
            {
                if (node.Name == "TravelDistance")
                {
                    return Constants.GetInvariantCultureDouble(node.InnerText);
                }
            }

            return double.NaN;
        }
    }
}