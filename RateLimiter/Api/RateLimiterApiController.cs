using Microsoft.AspNetCore.Mvc;

namespace RateLimiter.Api
{
    public class RateLimiterApiController : ControllerBase
    {
        private readonly RateLimiter _rateLimiter;

        public const string TooManyRequestsMessage = "Too many requests";

        public RateLimiterApiController()
        {
            _rateLimiter = RateLimiter.Instance;
        }

        [HttpGet("fixedLimit")]
        public IActionResult GetFixedLimit(string clientId)
        {
            if (!_rateLimiter.IsRequestAllowed(clientId, "fixedLimit"))
            {
                return StatusCode(429, TooManyRequestsMessage);
            }

            return Ok();
        }

        [HttpGet("slidingLimit")]
        public IActionResult GetSlidingLimit(string clientId)
        {
            if (!_rateLimiter.IsRequestAllowed(clientId, "slidingLimit"))
            {
                return StatusCode(429, TooManyRequestsMessage);
            }

            return Ok();
        }

        [HttpGet("regionBasedLimit")]
        public IActionResult GetRegionBasedLimit(string clientId)
        {
            if (!_rateLimiter.IsRequestAllowed(clientId, "regionLimit"))
            {
                return StatusCode(429, TooManyRequestsMessage);
            }

            return Ok();
        }
    }
}
