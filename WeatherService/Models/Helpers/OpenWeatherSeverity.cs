using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Models.Helpers
{
    public static class OpenWeatherSeverity
    {
        private static readonly Dictionary<int, int> WeatherIdToSeverity;
        static OpenWeatherSeverity()
        {
            // Assign severity levels to each weather type here. This is used to find which weather type represents each day
            // since we get an hourly forecast from the API.
            // More info: https://openweathermap.org/weather-conditions

            WeatherIdToSeverity = new Dictionary<int, int>()
            {
                // Clear.
                { 800, 0 },

                // Clouds.
                { 801, 1 },
                { 802, 3 },
                { 803, 4 },
                { 804, 5 },

                // Atmosphere.
                { 701, 10 },
                { 711, 10 },
                { 721, 10 },
                { 731, 10 },
                { 741, 10 },
                { 751, 10 },
                { 761, 10 },
                { 762, 17 }, // Volcanic ash. Sounds really bad for the lungs.
                { 771, 10 }, 
                { 781, 1000 }, // Tornado... Sounds dangerous.

                // Drizzle.
                { 300, 30 },
                { 301, 31 },
                { 302, 32 },
                { 310, 33 },
                { 311, 34 },
                { 312, 35 },
                { 313, 36 },
                { 314, 37 },
                { 321, 29 },

                // Rain.
                { 500, 50 },
                { 501, 51 },
                { 502, 52 },
                { 503, 53 },
                { 504, 54 },
                { 511, 55 },
                { 520, 47 },
                { 521, 48 },
                { 522, 49 },
                { 531, 47 },

                // Thunderstorm.
                { 200, 71 },
                { 201, 72 },
                { 202, 73 },
                { 210, 64 },
                { 211, 65 },
                { 212, 66 },
                { 221, 65 },
                { 230, 67 },
                { 231, 68 },
                { 232, 69 },

                // Snow.
                { 600, 90 },
                { 601, 91 },
                { 602, 92 },
                { 611, 93 },
                { 612, 94 },
                { 613, 95 },
                { 615, 96 },
                { 616, 97 },
                { 620, 98 },
                { 621, 99 },
                { 622, 100 }
            };
        }

        public static int GetSeverity(int weatherId)
        {
            if (WeatherIdToSeverity.TryGetValue(weatherId, out int severity))
            {
                return severity;
            }

            Log.Warning("OpenWeatherSeverity: No severity assigned to weatherId {0}. Returning -1.", weatherId);
            return -1;
        }
    }
}
