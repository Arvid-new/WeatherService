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

namespace WeatherService.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> Logger;
        private readonly WeatherProviderManager WeatherManager;
        private readonly UserService UserService;

        public WeatherController(ILogger<WeatherController> logger, WeatherProviderManager weatherManager, UserService userService)
        {
            Logger = logger;
            WeatherManager = weatherManager;
            UserService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody]User userParam)
        {
            var user = await UserService.AuthenticateAsync(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user.Token);
        }

        [HttpGet]
        public async Task<ActionResult> Get() // Used for testing currently.
        {
            var provider = WeatherManager.GetWeatherProvider(WeatherProvider.OpenWeather);
            if (provider == null)
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, "Access to this provider has been disabled.");

            var weather = await provider.GetWeatherAsync(new Coords(38.0831702, 23.792224));
            if (weather == null)
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, "Failed to get weather from provider.");

            Logger.LogInformation("Acquired weather from [default] " + provider.Name);
            return new JsonResult(weather);
        }

        [HttpGet("{provId},{lat},{lon}")]
        public async Task<ActionResult> Get(int provId, double lat, double lon)
        {
            if (!Coords.ValidateCoords(lat, lon))
                return BadRequest("The coordinates are invalid.");

            if (!Enum.IsDefined(typeof(WeatherProvider), provId))
                return BadRequest("No provider with ID " + provId + " has been defined.");

            var provider = WeatherManager.GetWeatherProvider((WeatherProvider)provId);
            if (provider == null)
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, "Access to this provider has been disabled.");

            var weather = await provider.GetWeatherAsync(new Coords(lat, lon));
            if (weather == null)
                return StatusCode((int)HttpStatusCode.ServiceUnavailable, "Failed to get weather from provider.");

            Logger.LogInformation("Acquired weather from " + provider.Name);
            return new JsonResult(weather);
        }
    }
}
