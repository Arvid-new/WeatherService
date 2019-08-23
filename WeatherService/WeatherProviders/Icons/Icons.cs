using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.WeatherProviders.Icons
{
    public static class Icons
    {
        public static IconMap OpenWeather;
        public static IconMap DarkSky;

        static Icons()
        {
            OpenWeather = new IconMap(new Dictionary<string, Icon>()
            {
                { "01d", Icon.Clear },
                { "01n", Icon.Clear },
                { "02d", Icon.PartlyCloudy },
                { "02n", Icon.PartlyCloudy },
                { "03d", Icon.Cloudy },
                { "03n", Icon.Cloudy },
                { "04d", Icon.Cloudy },
                { "04n", Icon.Cloudy },
                { "09d", Icon.Rain },
                { "09n", Icon.Rain },
                { "10d", Icon.Rain },
                { "10n", Icon.Rain },
                { "11d", Icon.Thunderstorm },
                { "11n", Icon.Thunderstorm },
                { "13d", Icon.Snow },
                { "13n", Icon.Snow },
                { "50d", Icon.Fog },
                { "50n", Icon.Fog }
            });

            DarkSky = new IconMap(new Dictionary<string, Icon>()
            {
                { "clear-day", Icon.Clear },
                { "clear-night", Icon.Clear },
                { "rain", Icon.Rain },
                { "snow", Icon.Snow },
                { "sleet", Icon.Sleet },
                { "wind", Icon.Wind },
                { "fog", Icon.Fog },
                { "cloudy", Icon.Cloudy },
                { "partly-cloudy-day", Icon.PartlyCloudy },
                { "partly-cloudy-night", Icon.PartlyCloudy },
            });
        }
    }
}
