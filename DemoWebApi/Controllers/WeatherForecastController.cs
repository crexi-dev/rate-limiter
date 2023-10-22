using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RateLimiter;
using RateLimiter.Attributes;

namespace DemoWebApi.Controllers;

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

    [HttpGet(Name = "GetWeatherForecast")]
    [RateRules(RateRulesEnum.RequestsPerTimeSpanRules, RateRulesEnum.CertainTimeSpanSinceLastCall)]
    public IEnumerable<WeatherForecast> Get([FromHeader(Name="x-api-token")][Required] string token)
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}