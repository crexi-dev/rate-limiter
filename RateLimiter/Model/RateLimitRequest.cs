using System;

namespace RateLimiter.Model
{
    public class RateLimitRequest
    {
        public string Token { get; set; }
        public Uri Url { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
