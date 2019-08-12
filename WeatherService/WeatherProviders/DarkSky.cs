using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherService.Models;

namespace WeatherService.WeatherProviders
{
    public class DarkSky : AbstractProvider
    {
        private const string APICall = @"https://api.darksky.net/forecast/{0}/{1},{2}?exclude=minutely,hourly,alerts";
        private const int UpdateMinutes = 70;

        public DarkSky(string name, string key, IMemoryCache cache) : base(name, key, cache) { }

        public override async Task<ResponseModel> GetWeatherAsync(Coords coords)
        {
            DateTime now = DateTime.UtcNow;

            // Try to get weather from cache.
            ResponseModel response = GetWeatherFromCache(coords);
            if (response != null)
            {
                if (now > response.Expiration) // Cached response is too old.
                {
                    LogInfo("Cache removed: Too old. | Coords: " + coords);
                    Cache.Remove(coords);
                }
                else
                {
                    LogInfo("Acquired weather from cache | Coords: " + coords);
                    return response;
                }
            }

            DarkSkyModel result = await CallFormatAsync<DarkSkyModel>(APICall, Key, coords.LatText, coords.LonText);
            if (result == null)
                return null;

            response = result.ToResponseModel();
            response.Expiration = now.AddMinutes(UpdateMinutes);
            Cache.Set(coords, response);
            return response;
        }

        private ResponseModel GetWeatherFromCache(Coords coords)
        {
            return Cache.TryGetValue(coords, out ResponseModel response) ? response : null;
        }
    }
}
