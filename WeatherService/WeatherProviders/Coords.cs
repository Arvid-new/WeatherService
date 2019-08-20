using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherService.WeatherProviders
{
    public struct Coords
    {
        public double Lat;
        public double Lon;

        [JsonIgnore]
        public string LatText => Lat.ToString(CultureInfo.InvariantCulture);
        [JsonIgnore]
        public string LonText => Lon.ToString(CultureInfo.InvariantCulture);

        public Coords(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public override string ToString()
        {
            return "Lat=" + LatText + " Lon=" + LonText;
        }

        public bool ValidateCoords()
        {
            return Lat >= -90 && Lat <= 90 && Lon >= -180 && Lon <= 180;
        }

        public static bool ValidateCoords(double lat, double lon)
        {
            return lat >= -90 && lat <= 90 && lon >= -180 && lon <= 180;
        }
    }
}
