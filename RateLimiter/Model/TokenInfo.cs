using System;

namespace RateLimiter.Model
{
    public class TokenInfo
    {
        public int NoOfTimesCalledInLastHour { get; set; }

        public DateTime LastRequestTime { get; set; }

        public string Location { get; set; }

    }
}
