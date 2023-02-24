using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Rules
{
    public class RequestsPerTimeSpanRule
    {
        public int RequestsCount { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
