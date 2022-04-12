using System;

namespace RateLimiter.Models
{
    public class RequestDetails
    {
        public int count { get; set; }

        public TimeSpan recordedTimeInterval { get; set; }

        public DateTime? TimeRequestMade { get; set; }
    }
}
