using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherService.WeatherProviders;

namespace WeatherService.Models
{
    public class ResponseModel
    {
        public DateTime CallTime;

        public string CityName;
        public Coords Coords;
        public string Country;

        public class Forecast
        {
            public long Date;
            public string DateText;
            public float? Temp;
            //public float Pressure;
            //public float Humidity;
            public string WeatherType;
            public string WeatherDescription;
            public int Cloudiness;
            public float? WindSpeed;
            public float? WindDeg;
        }

        public Forecast[] Forecasts;

        public ResponseModel() { }
    }
}
