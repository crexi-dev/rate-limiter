using Microsoft.AspNetCore.Mvc;
using RateLimiter.Enums;

namespace RateLimiter.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController()
        {
            
        }

        [HttpGet("/RestricByIp")]
        [RateLimitingAttribute(Restriction = RestrictionTypeEnum.IpAddress, MaxNumberRequest = 2)]
        public IEnumerable<WeatherForecast> RestricByIp()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("/RestricByApiKey")]
        [RateLimitingAttribute(Restriction = RestrictionTypeEnum.PerApiKey, MaxNumberRequest = 2)]
        public ActionResult<string> RestricByApiKey()
        {
            return "Restriction by User";
        }


        [HttpGet("/RestricByPerUser")]  
        [RateLimitingAttribute(Restriction = RestrictionTypeEnum.PerUser, MaxNumberRequest = 2)]
        public ActionResult<string> RestricByPerUser()
        {
            return "Restriction by User ";
        }
    }
}