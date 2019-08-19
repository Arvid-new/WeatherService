using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherService.Security;
using WeatherService.Entities;
using WeatherService.WeatherProviders;
using WeatherService.Error;
using Microsoft.AspNetCore.Http;

namespace WeatherService.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> Logger;
        private readonly WeatherProviderManager WeatherManager;

        public WeatherController(ILogger<WeatherController> logger, WeatherProviderManager weatherManager)
        {
            Logger = logger;
            WeatherManager = weatherManager;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromServices]UserService userService, [FromBody]User userParam)
        {
            var user = await userService.AuthenticateAsync(userParam.Username, userParam.Password);

            if (user == null)
                return ErrorInfo.BadRequest("Username or password is incorrect");

            Logger.LogInformation("User [{0}] has been successfully authenticated.", userParam.Username);
            return Ok(user.Token);
        }


        [HttpGet("providers")]
        public ActionResult Get()
        {
            return new JsonResult(WeatherManager.GetProviders());
        }

        [HttpGet("{provId},{lat},{lon}")]
        [ResponseCache(CacheProfileName = "DefaultWeather")]
        public async Task<ActionResult> Get(int provId, double lat, double lon)
        {
            if (!Coords.ValidateCoords(lat, lon))
                return ErrorInfo.BadRequest($"The coordinates are invalid. Coords: ({lat},{lon})");

            if (!Enum.IsDefined(typeof(WeatherProvider), provId))
                return ErrorInfo.BadRequest("No provider with ID " + provId + " has been defined.");

            var provider = WeatherManager.GetWeatherProvider((WeatherProvider)provId);
            if (provider == null)
                return ErrorInfo.BadRequest("Access to this provider has been disabled.");
            
            var weather = await provider.GetWeatherAsync(new Coords(lat, lon));
            if (weather == null)
                return ErrorInfo.ServiceUnvailable("Failed to get weather from " + provider.Name);

            provider.LogInfo("Acquired weather for coords: " + weather.Coords.ToString());
            return new JsonResult(weather);
        }
    }
}
