using Microsoft.AspNetCore.Mvc;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Attributes;

namespace RateLimiter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnhancedDemoController : ControllerBase
{
    private readonly ILogger<EnhancedDemoController> _logger;
    private readonly IRateLimitCounter _counter;

    public EnhancedDemoController(ILogger<EnhancedDemoController> logger, IRateLimitCounter counter)
    {
        _logger = logger;
        _counter = counter;
    }

    [HttpGet("authenticated")]
    [FixedWindowRateLimit("AuthenticatedUserLimit", 50, 60)]
    public IActionResult GetAuthenticated()
    {
        var userId = HttpContext.User.Identity?.Name ?? "anonymous";
        _logger.LogInformation("Authenticated endpoint called by user: {UserId}", userId);
        
        return Ok(new 
        { 
            message = "Authenticated user endpoint",
            user = userId,
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("premium")]
    [FixedWindowRateLimit("PremiumUserLimit", 100, 60)]
    public IActionResult GetPremium()
    {
        var tier = HttpContext.User.FindFirst("tier")?.Value ?? "standard";
        _logger.LogInformation("Premium endpoint called by tier: {Tier}", tier);
        
        return Ok(new 
        { 
            message = "Premium endpoint with higher limits",
            tier = tier,
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("region-aware")]
    [RegionBasedRateLimit("RegionAwareLimit", "US", 30, 60)]
    [RegionBasedRateLimit("RegionAwareLimit", "EU", 20, 60)]
    public IActionResult GetRegionAware()
    {
        var region = HttpContext.User.FindFirst("region")?.Value ?? "UNKNOWN";
        _logger.LogInformation("Region-aware endpoint called from region: {Region}", region);
        
        return Ok(new 
        { 
            message = "Region-aware endpoint with different limits per region",
            region = region,
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("client-info")]
    public IActionResult GetClientInfo()
    {
        var clientInfo = new
        {
            IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false,
            UserId = HttpContext.User.Identity?.Name,
            Claims = HttpContext.User.Claims.ToDictionary(c => c.Type, c => c.Value),
            Headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        return Ok(clientInfo);
    }

    [HttpPost("simulate-load")]
    [FixedWindowRateLimit("LoadTestLimit", 10, 60)]
    public async Task<IActionResult> SimulateLoad([FromBody] LoadTestRequest request)
    {
        _logger.LogInformation("Load test endpoint called with delay: {DelayMs}ms", request.DelayMs);
        
        // Simulate some processing time
        if (request.DelayMs > 0)
        {
            await Task.Delay(request.DelayMs);
        }
        
        return Ok(new 
        { 
            message = "Load test completed",
            delayMs = request.DelayMs,
            timestamp = DateTime.UtcNow
        });
    }
}

public class LoadTestRequest
{
    public int DelayMs { get; set; } = 0;
}
