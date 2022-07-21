using System;

namespace RateLimiter.Model
{
    public class UserRequestCache
    {
        public DateTime LastRequest { get; set; }
        public int RequestCount { get; set; }
    }
}
