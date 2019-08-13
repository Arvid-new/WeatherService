using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherService.Models;
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

        public WeatherProviderManager(IOptions<ApiKeys> keys, IMemoryCache cache)
        {
            Keys = keys.Value;

            // Add weather providers here.
            // Don't add any provider more than once.
            Providers.Add(WeatherProvider.OpenWeather, new OpenWeather("OpenWeather", Keys.OpenWeather, cache));
            //Providers.Add(WeatherProvider.AccuWeather, new AccuWeather("AccuWeather", Keys.AccuWeather, cache));
            Providers.Add(WeatherProvider.DarkSky, new DarkSky("DarkSky", Keys.DarkSky, cache));
        }

        public AbstractProvider GetWeatherProvider(WeatherProvider provider)
        {
            return Providers.TryGetValue(provider, out AbstractProvider value) ? value : null;
        }

        public bool IsProviderEnabled(WeatherProvider provider)
        {
            return Providers.ContainsKey(provider);
        }

        public List<ProviderView> GetProviders()
        {
            List<ProviderView> result = new List<ProviderView>();

            foreach (var prov in Providers)
            {
                result.Add(new ProviderView((int)prov.Key, prov.Value.Name));
            }

            return result;
        }
    }

    public class ProviderView
    {
        public int Id;
        public string Name;

        public ProviderView(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
