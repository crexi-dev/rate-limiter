using System;

namespace RateLimiter.Repositories
{
    internal class RequestRecord
    {
        public DateTime Timestamp { get; set; }
        public bool Result { get; set; }
    }
}