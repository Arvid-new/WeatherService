using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public WeatherProviderManager()
        {
            // Add weather providers here.
            // Don't add any provider more than once.

            Providers.Add(WeatherProvider.OpenWeather, new OpenWeather("OpenWeather"));
            Providers.Add(WeatherProvider.AccuWeather, new AccuWeather("AccuWeather"));
            Providers.Add(WeatherProvider.DarkSky, new DarkSky("DarkSky"));
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
