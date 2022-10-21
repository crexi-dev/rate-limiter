using Microsoft.AspNetCore.Mvc;

namespace RateLimiter.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RateLimitController : ControllerBase
    {

        private readonly ILogger<RateLimitController> _logger;

        public RateLimitController(ILogger<RateLimitController> logger)
        {
            _logger = logger;
        }

        [RateLimitAttribute.RequestRateLimit]
        public string Get()
        {
            return $"Client Rate limiting is enabled";
        }
    }
}