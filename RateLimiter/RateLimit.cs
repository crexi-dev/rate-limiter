namespace RateLimiter
{
    using System;

    internal class RateLimit
    {
        public DateTime FirstCall { get; set; } = DateTime.Now.ToUniversalTime();
        public int CallCount { get; set; }        
    }
}
 