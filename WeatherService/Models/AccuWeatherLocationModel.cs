using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Models
{
    public class AccuWeatherLocationModel
    {
        public int Version;
        public string Key;
        public string Type;
        public int Rank;
        public string LocalizedName;
        public string EnglishName;
        public string PrimaryPostalCode;

        public class Region
        {
            public string ID;
            public string LocalizedName;
            public string EnglishName;
        }

        public Region region;

        public class Country
        {
            public string ID;
            public string LocalizedName;
            public string EnglishName;
        }

        public Country country;

        public class AdministrativeArea
        {
            public string ID;
            public string LocalizedName;
            public string EnglishName;
            public int Level;
            public string LocalizedType;
            public string EnglishType;
            public string CountryID;
        }

        public AdministrativeArea administrativeArea;

        public class TimeZone
        {
            public string Code;
            public string Name;
            public float GmtOffset;
            public bool IsDaylightSaving;
            public string NextOffsetChange;
        }

        public TimeZone timeZone;

        public class GeoPosition
        {
            public double Latitude;
            public double Longitude;

            public class Elevation
            {
                public class Values
                {
                    public double Value;
                    public string Unit;
                    public int UnitType;
                }

                public Values Metric;
                public Values Imperial;
            }

            public Elevation elevation;
            public bool IsAlias;

            public class SupplementalAdminAreas
            {
                public int Level;
                public string LocalizedName;
                public string EnglishName;
            }

            public SupplementalAdminAreas supplementalAdminAreas;
            public string[] DataSets;
        }

        public GeoPosition geoPosition;
    }
}
