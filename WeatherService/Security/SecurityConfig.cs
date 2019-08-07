using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherService.Entities;

namespace WeatherService.Security
{
    public class SecurityConfig
    {
        public string Secret { get; set; }
        public User[] Users { get; set; }
    }
}
