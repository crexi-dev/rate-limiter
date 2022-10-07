using System;

namespace RateLimiter.Models
{
    public class LocationLimit
    {
        public string? LocationName { get; set; }

        public int ProceedRequestsCount { get; set; }

        public DateTime FirstRequestTime { get; set; }
    }
}