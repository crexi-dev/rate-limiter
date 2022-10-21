using Microsoft.AspNetCore.Mvc;

namespace RateLimiter2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RateLimiterController : ControllerBase
    {
        private readonly ILogger<RateLimiterController> _logger;

        public RateLimiterController(ILogger<RateLimiterController> logger)
        {
            _logger = logger;
        }

        [RateLimitAttribute.RequestRateLimit]
        public string Get()
        {
            return $"Requests are limited to 100 per user per hour";
        }
    }
}