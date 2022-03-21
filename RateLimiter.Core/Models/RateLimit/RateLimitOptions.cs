using System.Collections.Generic;

namespace RateLimiter.Core.Models.RateLimit
{
    public class RateLimitOptions
    {
        public List<RateLimitRule> GeneralRules { get; set; }

        public List<LocationRateLimitRule?> LocationRules { get; set; }
        /// <summary>
        /// Gets or sets the HTTP Status code returned when rate limiting occurs, by default value is set to 429 (Too Many Requests)
        /// </summary>
        public int HttpStatusCode { get; set; } = 429;

        public string ContentType { get; set; } = "text/plain";
        
        /// <summary>
        /// Gets or sets the counter prefix, used to compose the rate limit counter cache key
        /// </summary>
        public string RateLimitCounterPrefix { get; set; } = "crlc";
    }
}