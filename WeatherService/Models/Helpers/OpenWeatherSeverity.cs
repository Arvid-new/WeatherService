using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Models.Helpers
{
    public static class OpenWeatherSeverity
    {
        private static readonly Dictionary<int, int> WeatherIdToSeverity = new Dictionary<int, int>();
        static OpenWeatherSeverity()
        {
            // Assign severity levels to each weather type here. This is used to find which weather type represents each day
            // since we get an hourly forecast from the API.
            // More info: https://openweathermap.org/weather-conditions

            // Clear.
            Add(800, 0);

            // Clouds.
            Add(801, 1);
            Add(802, 3);
            Add(803, 4);
            Add(804, 5);

            // Atmosphere.
            Add(701, 10);
            Add(711, 10);
            Add(721, 10);
            Add(731, 10);
            Add(741, 10);
            Add(751, 10);
            Add(761, 10);
            Add(762, 17); // Volcanic ash. Sounds really bad for the lungs.
            Add(771, 10);
            Add(781, 1000); // Tornado... Sounds dangerous.

            // Drizzle.
            Add(300, 30);
            Add(301, 31);
            Add(302, 32);
            Add(310, 33);
            Add(311, 34);
            Add(312, 35);
            Add(313, 36);
            Add(314, 37);
            Add(321, 29);

            // Rain.
            Add(500, 50);
            Add(501, 51);
            Add(502, 52);
            Add(503, 53);
            Add(504, 54);
            Add(511, 55);
            Add(520, 47);
            Add(521, 48);
            Add(522, 49);
            Add(531, 47);

            // Thunderstorm.
            Add(200, 71);
            Add(201, 72);
            Add(202, 73);
            Add(210, 64);
            Add(211, 65);
            Add(212, 66);
            Add(221, 65);
            Add(230, 67);
            Add(231, 68);
            Add(232, 69);

            // Snow.
            Add(600, 90);
            Add(601, 91);
            Add(602, 92);
            Add(611, 93);
            Add(612, 94);
            Add(613, 95);
            Add(615, 96);
            Add(616, 97);
            Add(620, 98);
            Add(621, 99);
            Add(622, 100);
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

        private static void Add(int weatherId, int severity)
        {
            WeatherIdToSeverity.Add(weatherId, severity);
        }
    }
}
