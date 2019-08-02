using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Models
{
    public class DarkSkyModel
    {
        public double latitude;
        public double longitude;
        public string timezone;
        
        public class Status
        {
            public long time;
            public string summary;
            public string icon;
            public float precipIntensity;
            public float precipIntensityError;
            public float precipProbability;
            public string precipType;
            public float dewPoint;
            public float humidity;
            public float pressure;
            public float windSpeed;
            public float windGust;
            public int windBearing;
            public float cloudCover;
            public int uvIndex;
            public float visibility;
            public float ozone;
        }

        public class DataBlock
        {
            public string summary;
            public string icon;
        }

        public class Currently : Status
        {
            public float apparentTemperature;
            public int nearestStormDistance;
            public float nearestStormBearing;
            public float temperature;
        }

        public Currently currently;

        public class Minutely : DataBlock
        {
            public Status[] data;
        }

        public Minutely minutely;

        public class HourlyData : Status
        {
            public float apparentTemperature;
            public float precipAccumulation;
            public float temperature;
        }

        public class Hourly : DataBlock
        {
            public HourlyData[] data;
        }

        public Hourly hourly;

        public class DailyData : Status
        {
            public float apparentTemperatureHigh;
            public long apparentTemperatureHighTime;
            public float apparentTemperatureLow;
            public long apparentTemperatureLowTime;

            public float apparentTemperatureMax;
            public long apparentTemperatureMaxTime;
            public float apparentTemperatureMin;
            public long apparentTemperatureMinTime;

            public float moonPhase;
            public float precipAccumulation;
            public float precipIntensityMax;
            public long precipIntensityMaxTime;
            public long sunriseTime;
            public long sunsetTime;

            public float temperatureHigh;
            public long temperatureHighTime;
            public float temperatureLow;
            public long temperatureLowTime;

            public float temperatureMax;
            public long temperatureMaxTime;
            public float temperatureMin;
            public long temperatureMinTime;

            public long uvIndexTime;
            public long windGustTime;
        }

        public class Daily : DataBlock
        {
            public DailyData[] data;
        }

        public Daily daily;

        public class Flags
        {
            public string[] sources;
            public float nearest_station;
            public string units;
        }

        public Flags flags;

        public ResponseModel ToResponseModel()
        {
            ResponseModel response = new ResponseModel()
            {
                CityName = "",
                Coords = new WeatherProviders.Coords(latitude, longitude),
                Country = "",
                Forecasts = new ResponseModel.Forecast[daily.data.Length]
            };

            for (int i = 0; i < daily.data.Length; i++)
            {
                response.Forecasts[i] = new ResponseModel.Forecast
                {
                    Date = daily.data[i].time,
                    DateText = "",
                    Temp = (daily.data[i].temperatureHigh + daily.data[i].temperatureLow) / 2,
                    //Pressure = list[i].main.pressure,
                    //Humidity = list[i].main.humidity,
                    WeatherType = daily.data[i].icon,
                    WeatherDescription = daily.data[i].summary,
                    Cloudiness = (int)(daily.data[i].cloudCover * 100),
                    WindSpeed = daily.data[i].windSpeed,
                    WindDeg = daily.data[i].windBearing
                };
            }

            return response;
        }
    }
}
