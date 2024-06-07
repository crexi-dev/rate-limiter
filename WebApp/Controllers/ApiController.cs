using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimiter.Services.Interfaces;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController(
    ILogger<ApiController> logger,
    IRateLimitingManager rateLimitingManager)
    : ControllerBase
{
    private readonly ILogger<ApiController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> TestRateLimit(string resource, string accessToken)
    {
        try
        {
            var result = await rateLimitingManager.IsRequestAllowedAsync(resource, accessToken);

            if (result.StatusCode is 200 or 201)
            {
                return Ok();
            }

            return StatusCode(result.StatusCode??429, result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}