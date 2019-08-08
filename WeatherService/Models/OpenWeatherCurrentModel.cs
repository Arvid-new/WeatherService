using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Models
{
    public class OpenWeatherCurrentModel
    {
        public class Coord
        {
            public double lon;
            public double lat;
        }
        public Coord coord;

        public class Weather
        {
            public int id;
            public string main;
            public string description;
            public string icon;
        }
        public Weather[] weather;

        public string Base;

        public class Main
        {
            public float temp;
            public float pressure;
            public int humidity;
            public float temp_min;
            public float temp_max;
        }
        public Main main;

        public class Wind
        {
            public float speed;
            public float deg;
        }
        public Wind wind;

        public class Clouds
        {
            public int all;
        }
        public Clouds clouds;

        public long dt;
        
        public class Sys
        {
            public int type;
            public int id;
            public float message;
            public string country;
            public long sunrise;
            public long sunset;
        }
        public Sys sys;

        public int timezone;
        public int id;
        public string name;
        public int cod;

        public ResponseModel ToResponseModel(OpenWeatherModel hourly)
        {
            return hourly.ToResponseModel(this);
        }
    }
}
