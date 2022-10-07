using System.Collections.Generic;

namespace RateLimiter.Configuration.Options
{
    public class LimiterOptions
    {
        public IList<LocationLimiterOptions>? LocationLimiters { get; set; }
    }
}