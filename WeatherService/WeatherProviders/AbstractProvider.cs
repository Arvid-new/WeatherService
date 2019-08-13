using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherService.Models;

namespace WeatherService.WeatherProviders
{
    public abstract class AbstractProvider
    {
        protected static readonly HttpClient HttpClient = new HttpClient();

        protected readonly IMemoryCache Cache;

        public readonly string Name;
        public readonly string Key;

        public AbstractProvider(string name, string key, IMemoryCache cache)
        {
            Name = name;
            Key = key;
            Cache = cache;
        }

        public abstract Task<ResponseModel> GetWeatherAsync(Coords coords);

        public void LogInfo(string message)
        {
            Log.Information($"{Name}: {message}");
        }

        protected Task<T> CallFormatAsync<T>(string requestUri, params object[] args) where T : class
        {
            return CallAsync<T>(string.Format(requestUri, args));
        }

        protected async Task<T> CallAsync<T>(string requestUri) where T : class
        {
            T result = null;

            try
            {
                using (var response = await HttpClient.GetAsync(requestUri))
                {
                    // TODO: Check status code and special headers.
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<T>(apiResponse);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception while consuming API: [{0}]", requestUri);
            }

            return result;
        }
    }
}
