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

        public ResponseModel ToResponseModel(OpenWeatherCurrentModel current)
        {
            ResponseModel response = new ResponseModel()
            {
                Provider = "OpenWeather",
                CityName = city.name,
                Country = city.country,
                Coords = new WeatherProviders.Coords(city.coord.lat, city.coord.lon),
                Now = new ResponseModel.Current()
                {
                    Date = current.dt,
                    Temp = current.main.temp,
                    Humidity = current.main.humidity,
                    WeatherType = current.weather[0].main,
                    WeatherDescription = current.weather[0].description,
                    Cloudiness = current.clouds.all,
                    WindSpeed = current.wind.speed,
                    WindDeg = current.wind.deg
                },
                Forecasts = new ResponseModel.Forecast[5]
            };

            for (int i = 0; i < response.Forecasts.Length; i++)
            {
                int j = i * 8;

                ResponseModel.Forecast forecast = response.Forecasts[i] = new ResponseModel.Forecast
                {
                    Date = list[j].dt,
                    TempMax = float.MinValue,
                    TempMin = float.MaxValue,
                    Humidity = list[j].main.humidity,
                    WeatherType = list[j].weather[0].main,
                    WeatherDescription = list[j].weather[0].description,
                    Cloudiness = list[j].clouds.all,
                    WindSpeed = list[j].wind.speed,
                    WindDeg = list[j].wind.deg
                };

                j++;
                int count = 1;
                for (; j < i * 8 + 8; j++)
                {
                    if (list[j].main.temp < forecast.TempMin)
                        forecast.TempMin = list[j].main.temp;

                    if (list[j].main.temp > forecast.TempMax)
                        forecast.TempMax = list[j].main.temp;

                    forecast.Humidity += (list[j].main.humidity - forecast.Humidity) / count;
                    forecast.Cloudiness += (list[j].clouds.all - forecast.Cloudiness) / count;
                    forecast.WindSpeed += (list[j].wind.speed - forecast.WindSpeed) / count;

                    count++;
                }
            }

            return response;
        }
    }
}
