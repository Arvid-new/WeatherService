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
                Forecasts = new ResponseModel.Forecast[list.Length / 8]
            };

            // The free package returns the weather every 3 hours so we need to calculate the daily weather from that.
            for (int i = 0; i < response.Forecasts.Length; i++)
            {
                int j = i * 8;

                ResponseModel.Forecast forecast = response.Forecasts[i] = new ResponseModel.Forecast
                {
                    Date = list[j].dt,
                    TempMax = float.MinValue,
                    TempMin = float.MaxValue,
                    WeatherType = list[j].weather[0].main,
                    WeatherDescription = list[j].weather[0].description
                };

                float degRads = DegToRad(list[j].wind.deg);
                float degSinSum = MathF.Sin(degRads);
                float degCosSum = MathF.Cos(degRads);

                int humiditySum = list[j].main.humidity;
                int cloudinessSum = list[j].clouds.all;
                float windSpeedSum = list[j].wind.speed;

                int count = 1;
                for (++j; j < i * 8 + 8; j++)
                {
                    count++;

                    if (list[j].main.temp < forecast.TempMin)
                        forecast.TempMin = list[j].main.temp;

                    if (list[j].main.temp > forecast.TempMax)
                        forecast.TempMax = list[j].main.temp;

                    degRads = DegToRad(list[j].wind.deg);
                    degSinSum += MathF.Sin(degRads);
                    degCosSum += MathF.Cos(degRads);

                    humiditySum += list[j].main.humidity;
                    cloudinessSum += list[j].clouds.all;
                    windSpeedSum += list[j].wind.speed;
                }

                forecast.WindDeg = RadToDeg(MathF.Atan2(degSinSum / count, degCosSum / count)); // Average angle.
                if (forecast.WindDeg < 0) // Convert negative angle to positive.
                    forecast.WindDeg += 360;

                forecast.Humidity = humiditySum / count;
                forecast.Cloudiness = cloudinessSum / count;
                forecast.WindSpeed = windSpeedSum / count;
            }

            return response;
        }

        private float DegToRad(float degrees)
        {
            return MathF.PI / 180 * degrees;
        }

        private float RadToDeg(float rads)
        {
            return rads * 180 / MathF.PI;
        }
    }
}
