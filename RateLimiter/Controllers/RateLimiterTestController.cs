using Microsoft.AspNetCore.Mvc;
using RateLimiter.Middleware;

namespace RateLimiter.Controllers;

[ApiController]
[Route("[controller]")]
public class RateLimiterTestController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild"
    };

    private readonly ILogger<RateLimiterTestController> _logger;

    public RateLimiterTestController(ILogger<RateLimiterTestController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [RateLimit(RuleType = RuleTypeEnum.IpAddress, MaxRequests = 20)]

    // Here we are checking the rule associated with the ip address of a specific region. Similarly specific rules can be written for client tokens, client id's etc
    public IEnumerable<RateLimitModel> Get()
    {
        
        return Enumerable.Range(1, 5).Select(index => new RateLimitModel
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

}

