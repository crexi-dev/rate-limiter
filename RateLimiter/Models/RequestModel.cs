using System;

namespace RateLimiter.Models
{
    public class RequestModel
    {
        public string Token { get; set; }
        public DateTime RequestFired { get; set; }
    }
}