using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherService.Models;

namespace WeatherService.WeatherProviders
{
    public class AccuWeather : AbstractProvider
    {
        private const string LocationAPICall = @"http://dataservice.accuweather.com/locations/v1/cities/geoposition/search?q={0},{1}&apikey={2}";
        private const string WeatherAPICall = @"http://dataservice.accuweather.com/forecasts/v1/daily/5day/{0}?details=true&apikey={1}";
        private const string CurrentAPICall = @"http://dataservice.accuweather.com/currentconditions/v1/{0}?details=true&apikey={1}";
        private const int UpdateMinutes = 90;

        private readonly ConcurrentDictionary<Coords, AccuWeatherLocationModel> CoordsToLoc = new ConcurrentDictionary<Coords, AccuWeatherLocationModel>();
        private readonly ConcurrentDictionary<string, ResponseModel> ResponseCache = new ConcurrentDictionary<string, ResponseModel>();

        public AccuWeather(string name, string key, IMemoryCache cache) : base(name, key, cache) { }

        public override async Task<ResponseModel> GetWeatherAsync(Coords coords)
        {
            DateTime now = DateTime.UtcNow;

            ResponseModel response = GetWeatherFromCache(coords);
            if (response != null) // We have both the location key and the weather.
            {
                if (now > response.Expiration) // Cached response is too old.
                {
                    LogInfo("Cache removed: Too old. | Coords: " + coords);
                    ResponseCache.Remove(CoordsToLoc[coords].Key, out _);
                }
                else
                {
                    LogInfo("Acquired weather from cache | Coords: " + coords);
                    return response;
                }
            }

            if (CoordsToLoc.TryGetValue(coords, out AccuWeatherLocationModel loc)) // We have the location key and we want the weather.
            {
                AccuWeatherModel daily = await WeatherCall(loc.Key);
                if (daily == null)
                    return null;

                AccuWeatherCurrentModel[] current = await CurrentWeatherCall(loc.Key);
                if (current == null)
                    return null;

                response = daily.ToResponseModel(loc, current[0]);
                response.Expiration = now.AddMinutes(UpdateMinutes);
                ResponseCache.TryAdd(loc.Key, response);
                return response;
            }

            // We don't have the location key. Maybe we have the weather.
            AccuWeatherLocationModel locResult = await LocationCall(coords);
            if (locResult == null)
                return null;

            CoordsToLoc.TryAdd(coords, locResult);

            if (ResponseCache.TryGetValue(locResult.Key, out response)) // We have the weather.
            {
                return response;
            }

            // We don't have the weather.
            AccuWeatherModel daily2 = await WeatherCall(locResult.Key);
            if (daily2 == null)
                return null;

            AccuWeatherCurrentModel[] current2 = await CurrentWeatherCall(locResult.Key);
            if (current2 == null)
                return null;

            response = daily2.ToResponseModel(locResult, current2[0]);
            response.Expiration = now.AddMinutes(UpdateMinutes);
            ResponseCache.TryAdd(locResult.Key, response);
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

        private Task<AccuWeatherLocationModel> LocationCall(Coords coords)
        {
            return CallFormatAsync<AccuWeatherLocationModel>(LocationAPICall, coords.LatText, coords.LonText, Key);
        }

        private Task<AccuWeatherModel> WeatherCall(string locationID)
        {
            return CallFormatAsync<AccuWeatherModel>(WeatherAPICall, locationID, Key);
        }

        private Task<AccuWeatherCurrentModel[]> CurrentWeatherCall(string locationID)
        {
            return CallFormatAsync<AccuWeatherCurrentModel[]>(CurrentAPICall, locationID, Key);
        }
    }
}
