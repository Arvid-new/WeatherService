using Newtonsoft.Json;
using Serilog;
using System;
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
        private const string Key = "ea2a68cad3fd31088bc61197be2944bd";
        private const string APICall = @"https://api.darksky.net/forecast/{0}/{1},{2}?exclude=minutely,alerts";
        private const int UpdateMinutes = 70;

        private readonly Dictionary<Coords, ResponseModel> ResponseCache = new Dictionary<Coords, ResponseModel>();

        public DarkSky(string name) : base(name) { }

        public override ResponseModel GetWeather(Coords coords)
        {
            // Try to get weather from cache.
            ResponseModel response = GetWeatherFromCache(coords);
            if (response != null)
            {
                if ((DateTime.UtcNow - response.CallTime).Minutes > UpdateMinutes) // Cached response is too old.
                {
                    LogInfo("Cache removed: Too old. | Coords: " + coords);
                    ResponseCache.Remove(coords);
                }
                else
                {
                    LogInfo("Acquired weather from cache | Coords: " + coords);
                    return response;
                }
            }

            DarkSkyModel result = CallFormatAsync<DarkSkyModel>(APICall, Key, coords.LatText, coords.LonText).Result;
            if (result == null)
                return null;

            response = result.ToResponseModel();
            response.CallTime = DateTime.UtcNow;
            ResponseCache.Add(coords, response);
            return response;
        }

        private ResponseModel GetWeatherFromCache(Coords coords)
        {
            return ResponseCache.TryGetValue(coords, out ResponseModel response) ? response : null;
        }
    }
}
