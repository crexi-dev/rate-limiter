using RateLimiter.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public abstract class Rule
    {
        public TimeSpan Period { get; set; }

        public int Limit { get; set; }

        public Location Location { get; set; }

        public abstract Task<bool> ValidateAsync(IEnumerable<DateTime> requestDates);
    }
}
