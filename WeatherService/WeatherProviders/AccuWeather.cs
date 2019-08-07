using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherService.Models;

namespace WeatherService.WeatherProviders
{
    public class AccuWeather : AbstractProvider
    {
        private const string Key = "it3AiVlpDhiEmV47sHlv8GW3Xs8vvaAG ";
        private const string LocationAPICall = @"http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?q={0},{1}&apikey={2}";
        private const string WeatherAPICall = @"http://dataservice.accuweather.com/forecasts/v1/daily/5day/{0}?details=true&apikey={1}";
        private const int UpdateMinutes = 90;

        private readonly Dictionary<Coords, AccuWeatherLocationModel> CoordsToLoc = new Dictionary<Coords, AccuWeatherLocationModel>();
        private readonly Dictionary<string, ResponseModel> ResponseCache = new Dictionary<string, ResponseModel>();

        public AccuWeather(string name) : base(name) { }

        public override async Task<ResponseModel> GetWeatherAsync(Coords coords)
        {
            ResponseModel response = GetWeatherFromCache(coords);
            if (response != null) // We have both the location key and the weather.
            {
                if ((DateTime.UtcNow - response.CallTime).Minutes > UpdateMinutes) // Cached response is too old.
                {
                    LogInfo("Cache removed: Too old. | Coords: " + coords);
                    ResponseCache.Remove(CoordsToLoc[coords].Key);
                }
                else
                {
                    LogInfo("Acquired weather from cache | Coords: " + coords);
                    return response;
                }
            }

            if (CoordsToLoc.TryGetValue(coords, out AccuWeatherLocationModel loc)) // We have the location key and we want the weather.
            {
                AccuWeatherModel model = await WeatherCall(loc.Key);
                if (model == null)
                    return null;

                response = model.ToResponseModel(loc);
                response.CallTime = DateTime.UtcNow;
                ResponseCache.Add(loc.Key, response);
                return response;
            }

            // We don't have the location key. Maybe we have the weather.
            AccuWeatherLocationModel locResult = await LocationCall(coords);
            if (locResult == null)
                return null;

            CoordsToLoc.Add(coords, locResult);

            if (ResponseCache.TryGetValue(locResult.Key, out response)) // We have the weather.
            {
                return response;
            }

            // We don't have the weather.
            AccuWeatherModel result = await WeatherCall(locResult.Key);
            if (result == null)
                return null;

            response = result.ToResponseModel(locResult);
            response.CallTime = DateTime.UtcNow;
            ResponseCache.Add(locResult.Key, response);
            return response;
        }

        private ResponseModel GetWeatherFromCache(Coords coords)
        {
            if (CoordsToLoc.TryGetValue(coords, out AccuWeatherLocationModel locKey))
            {
                if (ResponseCache.TryGetValue(locKey.Key, out ResponseModel response))
                {
                    return response;
                }
            }
            return null;
        }

        private async Task<AccuWeatherLocationModel> LocationCall(Coords coords)
        {
            return await CallFormatAsync<AccuWeatherLocationModel>(LocationAPICall, coords.LatText, coords.LonText, Key);
        }

        private async Task<AccuWeatherModel> WeatherCall(string locationID)
        {
            return await CallFormatAsync<AccuWeatherModel>(WeatherAPICall, locationID, Key);
        }
    }
}
