using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Models
{
    public class AccuWeatherCurrentModel
    {
        public string LocalObservationDateTime;
        public long EpochTime;
        public string WeatherText;
        public int? WeatherIcon;
        public bool HasPrecipitation;
        public string PrecipitationType;
        public bool IsDayTime;
        
        public class Values
        {
            public class Val
            {
                public double? value;
                public string Unit;
                public int UnitType;
            }

            public Val Metric;
            public Val Imperial;
        }

        public Values Temperature;
        public Values RealFeelTemperature;
        public Values RealFeelTemperatureShade;

        public int? RelativeHumidity;

        public Values DewPoint;

        public class Wind
        {
            public class Direction
            {
                public double? Degrees;
                public string Localized;
                public string English;
            }

            public Direction direction;
            public Values Speed;
        }

        public Wind wind;

        public class WindGust
        {
            public Values Speed;
        }

        public WindGust windGust;

        public int? UVIndex;
        public string UVIndexText;
        public Values Visibility;
        public int? CloudCover;
        public Values ApparentTemperature;

        public string Link;
        public string MobileLink;

        public ResponseModel ToResponseModel(AccuWeatherLocationModel locModel, AccuWeatherModel daily)
        {
            return daily.ToResponseModel(locModel, this);
        }
    }
}
