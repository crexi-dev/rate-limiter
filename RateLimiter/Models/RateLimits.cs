using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public class RateLimits
    {
        public int PerSecond { get; set; }
        public int PerMinute { get; set; }
        public int PerHour { get; set; }
        public int PerDay { get; set; }
        public TimeSpan MinimumSpan { get; set; }


        public RateLimits(int perSecond, int perMinute, int perHour, int perDay)
        {
            PerSecond = perSecond;
            PerMinute = perMinute;
            PerHour = perHour;
            PerDay = perDay;
        }

        public RateLimits()
        {

        }
    }
}
