using System;

namespace RateLimiter.Model
{
    public class Request
    {
        public string Token { get; set; }
        public DateTime LastAccessTime { get; set; }
    }
}
