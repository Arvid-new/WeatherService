﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherService.Models;

namespace WeatherService.WeatherProviders
{
    public class OpenWeather : AbstractProvider
    {
        private const string Key = "423bfce432ab364947047977e369fabe";
        private const string APICall = @"http://api.openweathermap.org/data/2.5/forecast?lat={0}&lon={1}&APPID={2}";
        private const int UpdateMinutes = 120;
        private const int CallsPerMinute = 60;
        private const int BlockMinutes = 10;

        private bool Blocked = false;
        private DateTime LastBlocked;

        private int Calls = 0;
        private DateTime LastCallsReset = DateTime.UtcNow;

        private readonly Dictionary<Coords, ResponseModel> ResponseCache = new Dictionary<Coords, ResponseModel>();

        public OpenWeather(string name) : base(name) { }

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

            // Can we call the API right now?
            if (!CheckAvailability())
            {
                return null;
            }

            OpenWeatherModel result = CallFormatAsync<OpenWeatherModel>(APICall, coords.LatText, coords.LonText, Key).Result;
            if (result == null)
                return null;

            Calls++;
            if (result.cod == "429") // 429 means we are blocked.
            {
                Blocked = true;
                LastBlocked = DateTime.UtcNow;
                return null;
            }

            response = result.ToResponseModel();
            response.CallTime = DateTime.UtcNow;
            ResponseCache.Add(coords, response);
            return response;
        }

        private ResponseModel GetWeatherFromCache(Coords coords)
        {
            return ResponseCache.TryGetValue(coords, out ResponseModel response) ? response : null;
        }

        /// <summary>
        /// Returns true if it's possible to make another API call.
        /// </summary>
        /// <returns></returns>
        private bool CheckAvailability()
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
