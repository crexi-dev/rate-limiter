using Microsoft.AspNetCore.Mvc;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Common.Attributes;

namespace RateLimiter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[FixedWindowRateLimit("AdminApiLimit", 10, 60)]
public class AdminController : ControllerBase
{
    private readonly IRateLimiterService _rateLimiterService;
    private readonly IRateLimitCounter _counter;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IRateLimiterService rateLimiterService,
        IRateLimitCounter counter,
        ILogger<AdminController> logger)
    {
        _rateLimiterService = rateLimiterService;
        _counter = counter;
        _logger = logger;
    }

    [HttpPost("reset/{clientId}")]
    public async Task<IActionResult> ResetLimits(string clientId)
    {
        _logger.LogInformation("Resetting rate limits for client {ClientId}", clientId);
        
        // Call the service to reset limits
        await _rateLimiterService.ResetLimitsAsync(clientId);
        
        // Also directly reset with the counter as a fallback
        await _counter.ResetAsync(clientId);
        
        return Ok(new { message = $"Rate limits reset for client {clientId}" });
    }
}
