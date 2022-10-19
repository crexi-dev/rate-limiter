using System;

namespace RateLimiter.Model
{
    public class ClientRequestStorage
    {
        public DateTime LastRequest { get; set; }
        public int RequestCount { get; set; }
    }
}
