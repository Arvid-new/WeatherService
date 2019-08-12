using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherService.WeatherProviders;

namespace WeatherService.Models
{
    public class ResponseModel
    {
        public DateTime Expiration;
        public string Provider;

        public string CityName;
        public string Country;
        public Coords Coords;

        public class Current
        {
            public long Date;
            public float? Temp;
            public float? Humidity;
            public string WeatherType;
            public string WeatherDescription;
            public int? Cloudiness;
            public float? WindSpeed;
            public float? WindDeg;
        }

        public Current Now;

        public class Forecast
        {
            public long Date;
            public float? TempMin;
            public float? TempMax;
            public float? Humidity;
            public string WeatherType;
            public string WeatherDescription;
            public int? Cloudiness;
            public float? WindSpeed;
            public float? WindDeg;
        }

        public Forecast[] Forecasts;

        public ResponseModel() { }
    }
}
