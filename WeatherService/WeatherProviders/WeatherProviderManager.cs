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
        DarkSky,
        AccuWeather
    }

    public class WeatherProviderManager
    {
        private readonly Dictionary<WeatherProvider, AbstractProvider> Providers = new Dictionary<WeatherProvider, AbstractProvider>();

        public WeatherProviderManager(IOptions<ApiKeys> keysOpts, IMemoryCache cache)
        {
            ApiKeys keys = keysOpts.Value;

            // Add weather providers here. Comment them out to disable them.

            Providers.Add(WeatherProvider.OpenWeather, new OpenWeather("OpenWeather", keys.OpenWeather, cache));
            Providers.Add(WeatherProvider.DarkSky, new DarkSky("DarkSky", keys.DarkSky, cache));
            //Providers.Add(WeatherProvider.AccuWeather, new AccuWeather("AccuWeather", Keys.AccuWeather, cache));
        }

        public AbstractProvider GetWeatherProvider(WeatherProvider provider)
        {
            return Providers.TryGetValue(provider, out AbstractProvider value) ? value : null;
        }

        public bool IsProviderEnabled(WeatherProvider provider)
        {
            return Providers.ContainsKey(provider);
        }

        public List<ProviderViewModel> GetProviders()
        {
            List<ProviderViewModel> result = new List<ProviderViewModel>();

            foreach (var prov in Providers)
            {
                result.Add(new ProviderViewModel((int)prov.Key, prov.Value.Name));
            }

            return result;
        }
    }

    public class ProviderViewModel
    {
        public int Id;
        public string Name;

        public ProviderViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
