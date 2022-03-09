using System;

namespace RateLimiter.Models.Domain
{

    public class Request
    {
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
        public double Cost { get; set; }

        public Request(DateTime timestamp, long size, double cost)
        {
            Timestamp = timestamp;
            Size = size;
            Cost = cost;
        }
    }
}
