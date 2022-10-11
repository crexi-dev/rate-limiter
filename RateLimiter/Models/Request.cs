using System;

namespace RateLimiter.Models
{
    internal class Request
    {
        public User User { get; set; }
        public string Api { get; set; }
        public DateTimeOffset DateTime { get; set; }
    }
}