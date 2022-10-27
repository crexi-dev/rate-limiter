using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Rules
{
    public abstract class RateLimitRule
    {
        public string Resource { get; set; }
        public string Method { get; set; }
        public int RequestCount { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public abstract bool IsValid(List<DateTime> requestHistory);
    }
}
