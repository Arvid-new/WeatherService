using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.WeatherProviders
{
    public enum Icon
    {
        Clear,
        Cloudy,
        PartlyCloudy,
        Fog,
        Wind,
        Rain,
        Thunderstorm,
        Sleet,
        Snow,
    }

    public class IconMap
    {
        private const Icon DefaultIcon = Icon.Clear;

        private readonly Dictionary<string, Icon> Map;

        public IconMap(Dictionary<string, Icon> map)
        {
            Map = map;
        }

        public Icon GetIcon(string key)
        {
            if (Map.TryGetValue(key, out Icon icon))
            {
                return icon;
            }

            Log.Warning("No Icon assigned to key {0}. Returning default Icon.", key);
            return DefaultIcon;
        }
    }
}
