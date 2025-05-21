using Microsoft.AspNetCore.Mvc;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Attributes;

namespace RateLimiter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;
    private readonly IRateLimitCounter _counter;

    public DemoController(ILogger<DemoController> logger, IRateLimitCounter counter)
    {
        _logger = logger;
        _counter = counter;
    }

    [HttpGet]
    [FixedWindowRateLimit("GlobalLimit", 5, 60)]
    public IActionResult Get()
    {
        _logger.LogInformation("Demo endpoint called");
        return Ok(new { message = "Demo endpoint - subject to global rate limit" });
    }

    [HttpGet("users")]
    [SlidingWindowRateLimit("ApiUserEndpoint", 3, 60)]
    public IActionResult GetUsers()
    {
        _logger.LogInformation("Users endpoint called");
        return Ok(new List<object>
        {
            new { id = 1, name = "User 1" },
            new { id = 2, name = "User 2" },
            new { id = 3, name = "User 3" }
        });
    }

    [HttpGet("users/{id}")]
    [SlidingWindowRateLimit("ApiUserDetailsEndpoint", 20, 60)]
    public IActionResult GetUser(int id)
    {
        _logger.LogInformation("User details endpoint called for ID {Id}", id);
        return Ok(new { id = id, name = $"User {id}" });
    }

    [HttpGet("burst")]
    [TokenBucketRateLimit("BurstLimit", 50, 1.0, 60)]
    public IActionResult GetBurst()
    {
        _logger.LogInformation("Burst endpoint called");
        return Ok(new { message = "Burst endpoint - subject to token bucket rate limit" });
    }

    [HttpGet("region/us")]
    [RegionBasedRateLimit("UsRegionLimit", "US", 20, 60)]
    public IActionResult GetUsRegion()
    {
        _logger.LogInformation("US Region endpoint called");
        return Ok(new { message = "US region endpoint - higher limits for US region" });
    }

    [HttpGet("region/eu")]
    [RegionBasedRateLimit("EuRegionLimit", "EU", 10, 60, 1000)]
    public async Task<IActionResult> GetEuRegion()
    {
        _logger.LogInformation("EU Region endpoint called");
        
        string key = $"EuRegionLimit:EU:get:/api/demo/region/eu";
        long count = await _counter.GetCountAsync(key);
        
        return Ok(new { 
            message = "EU region endpoint - lower limits and minimum time between requests for EU region",
            currentCount = count
        });
    }

    [HttpGet("headers")]
    public IActionResult GetHeaders()
    {
        // Return all request headers for debugging
        var headers = new Dictionary<string, string>();
        foreach (var header in Request.Headers)
        {
            headers[header.Key] = header.Value.ToString();
        }
        
        _logger.LogInformation("Headers endpoint called");
        return Ok(new { headers });
    }
    
    [HttpGet("debug")]
    public async Task<IActionResult> GetDebugInfo([FromQuery] string? clientId = null)
    {
        _logger.LogInformation("Debug endpoint called");
        
        // Return debug info
        var debugInfo = new Dictionary<string, object>();
        
        // Include client ID
        debugInfo["clientId"] = clientId ?? "unknown";
        
        // Get all headers
        var headers = new Dictionary<string, string>();
        foreach (var header in Request.Headers)
        {
            headers[header.Key] = header.Value.ToString();
        }
        debugInfo["headers"] = headers;
        
        // Get counter values for some keys
        if (!string.IsNullOrEmpty(clientId))
        {
            var globalLimitKey = $"GlobalLimit:{clientId}:/api/demo";
            var usersLimitKey = $"ApiUserEndpoint:{clientId}:/api/demo/users";
            
            debugInfo["globalLimitCount"] = await _counter.GetCountAsync(globalLimitKey);
            debugInfo["usersLimitCount"] = await _counter.GetCountAsync(usersLimitKey);
        }
        
        return Ok(debugInfo);
    }

    [HttpGet("debug/services")]
    public IActionResult GetRegisteredServices([FromServices] IServiceProvider serviceProvider)
    {
        var ruleProvider = serviceProvider.GetRequiredService<IRateLimitRuleProvider>();
        var clientProvider = serviceProvider.GetRequiredService<IRateLimitClientIdentifierProvider>();
        var authService = serviceProvider.GetService<IAuthenticationService>();
        var geoService = serviceProvider.GetService<IGeoIPService>();
        
        return Ok(new
        {
            RuleProvider = ruleProvider.GetType().Name,
            ClientProvider = clientProvider.GetType().Name,
            AuthService = authService?.GetType().Name ?? "Not registered",
            GeoService = geoService?.GetType().Name ?? "Not registered"
        });
    }
}
