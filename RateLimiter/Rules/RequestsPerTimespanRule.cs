using System;
using System.Collections.Generic;
using System.Text;
using RateLimiter.Configuration;

namespace RateLimiter.Rules
{
    public class RequestsPerTimeSpanRule : IRule
    {
        public int RequestsCount { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}
