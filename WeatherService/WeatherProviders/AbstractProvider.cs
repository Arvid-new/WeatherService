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
    public struct Coords
    {
        public double Lat;
        public double Lon;

        public string LatText => Lat.ToString(CultureInfo.InvariantCulture);
        public string LonText => Lon.ToString(CultureInfo.InvariantCulture);

        public Coords(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public override string ToString()
        {
            return "Lat=" + Lat + " Lon=" + Lon;
        }

        public bool ValidateCoords()
        {
            return Lat >= -90 && Lat <= 90 && Lon >= -180 && Lon <= 180;
        }

        public static bool ValidateCoords(double lat, double lon)
        {
            return lat >= -90 && lat <= 90 && lon >= -180 && lon <= 180;
        }
    }

    public abstract class AbstractProvider
    {
        protected static readonly HttpClient HttpClient = new HttpClient();

        public readonly string Name;
        public readonly string Key;

        public AbstractProvider(string name, string key)
        {
            Name = name;
            Key = key;
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
