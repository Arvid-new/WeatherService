using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherService.Security;

namespace WeatherService.WeatherProviders
{
    public enum WeatherProvider
    {
        OpenWeather,
        AccuWeather,
        DarkSky
    }

    public class WeatherProviderManager
    {
        private readonly Dictionary<WeatherProvider, AbstractProvider> Providers = new Dictionary<WeatherProvider, AbstractProvider>();
        private readonly ApiKeys Keys;

        public WeatherProviderManager(IOptions<ApiKeys> keys)
        {
            Keys = keys.Value;

            // Add weather providers here.
            // Don't add any provider more than once.

            Providers.Add(WeatherProvider.OpenWeather, new OpenWeather("OpenWeather", Keys.OpenWeather));
            Providers.Add(WeatherProvider.AccuWeather, new AccuWeather("AccuWeather", Keys.AccuWeather));
            Providers.Add(WeatherProvider.DarkSky, new DarkSky("DarkSky", Keys.DarkSky));
        }

        public AbstractProvider GetWeatherProvider(WeatherProvider provider)
        {
            return Providers.TryGetValue(provider, out AbstractProvider value) ? value : null;
        }

        public bool IsProviderEnabled(WeatherProvider provider)
        {
            return Providers.ContainsKey(provider);
        }
    }
}
