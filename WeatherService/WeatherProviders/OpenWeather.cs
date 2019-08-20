using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WeatherService.Models;

namespace WeatherService.WeatherProviders
{
    public class OpenWeather : AbstractProvider
    {
        private const string HourlyAPICall = @"http://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&APPID={2}&units={3}";
        private const string CurrentAPICall = @"http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&APPID={2}&units={3}";
        private const int UpdateMinutes = 30;
        private const int CallsPerMinute = 60;
        private const int BlockMinutes = 10;

        private readonly object Lock = new object();

        private bool Blocked = false;
        private DateTime LastBlocked;

        private int Calls = 0;
        private DateTime LastCallsReset = DateTime.UtcNow;

        public OpenWeather(string name, string key, IMemoryCache cache) : base(name, key, cache) { }

        public override async Task<ResponseModel> GetWeatherAsync(Coords coords, Units units)
        {
            // Try to get weather from cache.
            if (Cache.TryGetValue(coords, out ResponseModel response))
            {
                LogInfo("Acquired response from cache for coords: " + coords);
                return response;
            }

            // Can we call the API right now?
            if (!CheckAvailability())
                return null;

            string unitsTxt = units.ToString();

            var hourly = await CallFormatAsync<OpenWeatherModel>(HourlyAPICall, coords.LatText, coords.LonText, Key, unitsTxt);
            if (hourly == null)
                return null;

            Interlocked.Increment(ref Calls);

            var current = await CallFormatAsync<OpenWeatherCurrentModel>(CurrentAPICall, coords.LatText, coords.LonText, Key, unitsTxt);
            if (current == null)
                return null;

            Interlocked.Increment(ref Calls);

            if (hourly.cod == "429" || current.cod == 429) // 429 means we are blocked.
            {
                Block();
                return null;
            }

            response = hourly.ToResponseModel(current);
            response.Units = unitsTxt;
            response.Expiration = DateTime.UtcNow.AddMinutes(UpdateMinutes);
            Cache.Set(coords, response, response.Expiration);
            return response;
        }

        private void Block()
        {
            lock (Lock)
            {
                Blocked = true;
                LastBlocked = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Returns true if it's possible to make another API call.
        /// </summary>
        /// <returns></returns>
        private bool CheckAvailability()
        {
            lock (Lock)
            {
                DateTime now = DateTime.UtcNow;

                if ((now - LastCallsReset).TotalMinutes >= 1) // Reset calls.
                {
                    Calls = 0;
                    LastCallsReset = now;
                }

                if (Blocked)
                {
                    if ((now - LastBlocked).TotalMinutes > BlockMinutes) // Block is over.
                    {
                        Blocked = false;
                    }
                    else // Block is still active.
                    {
                        return false;
                    }
                }

                if (Calls >= CallsPerMinute - 1)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
