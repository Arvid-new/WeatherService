using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Models
{
    public class OpenWeatherModel
    {
        public string cod;
        public float message;

        public class City
        {
            public int id;
            public string name;

            public class Coord
            {
                public double lon;
                public double lat;
            }
            public Coord coord;

            public string country;
            public int timezone;
        }
        public City city;

        public int cnt;

        public class Forecast
        {
            public long dt;

            public class Main
            {
                public float temp;
                public float temp_min;
                public float temp_max;
                public float pressure;
                public float sea_level;
                public float grnd_level;
                public int humidity;
                public float temp_kf;
            }
            public Main main;

            public class Weather
            {
                public int id;
                public string main;
                public string description;
                public string icon;
            }
            public Weather[] weather;

            public class Clouds
            {
                public int all;
            }
            public Clouds clouds;

            public class Wind
            {
                public float speed;
                public float deg;
            }
            public Wind wind;

            public class Sys
            {
                public string pod;
            }
            public Sys sys;

            public string dt_txt;
        }
        public Forecast[] list;

        public OpenWeatherModel() { }

        public ResponseModel ToResponseModel()
        {
            ResponseModel response = new ResponseModel()
            {
                CityName = city.name,
                Coords = new WeatherProviders.Coords(city.coord.lat, city.coord.lon),
                Country = city.country,
                Forecasts = new ResponseModel.Forecast[list.Length]
            };

            for (int i = 0; i < list.Length; i++)
            {
                response.Forecasts[i] = new ResponseModel.Forecast
                {
                    Date = list[i].dt,
                    DateText = list[i].dt_txt,
                    Temp = list[i].main.temp,
                    //Pressure = list[i].main.pressure,
                    //Humidity = list[i].main.humidity,
                    WeatherType = list[i].weather[0].main,
                    WeatherDescription = list[i].weather[0].description,
                    Cloudiness = list[i].clouds.all,
                    WindSpeed = list[i].wind.speed,
                    WindDeg = list[i].wind.deg
                };
            }

            return response;
        }
    }
}
