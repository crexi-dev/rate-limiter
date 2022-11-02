using System;

namespace RateLimiter.Rules
{
    public abstract class Rule
    {
        public string? Endpoint { get; set; }

        public TimeSpan Period { get; set; }

        public int Limit { get; set; }
    }
}
