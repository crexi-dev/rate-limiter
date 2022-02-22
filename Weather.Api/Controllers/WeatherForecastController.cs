using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimits.RateLimits;
using RateLimits.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weather.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IRateLimiter _rateLimiter;

        public WeatherForecastController(IRateLimiter rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        [HttpGet]
        public ActionResult Get([FromQuery] string token, [FromQuery] string region)
        {
            if (!_rateLimiter.HasAccess(token, "Weather/Get", region,
                new RequestsPerTimespanRule(TimeSpan.FromSeconds(30), 2, "Georgia"),
                new TimespanSinceLastCallRule(TimeSpan.FromSeconds(10), "USA")))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            var rng = new Random();
            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray());
        }
    }
}
