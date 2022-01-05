using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimiter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiterOwn.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
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

        [HttpGet("check")]
        public IActionResult Get(int userId)
        {
            var rateLimiter = new RateLimiters();
            if (!rateLimiter.CheckRateLimit(userId, Rules.AccessCounter | Rules.LastAccessTime))
            {
                return new StatusCodeResult(429);
            }
            return Ok("Nice to Meet You");
        }
    }
}
