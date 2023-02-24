using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Configuration;

namespace RateLimiter.Rules
{
    public class RequestsPerTimeSpanRule : IRule
    {
        public int RequestsCount { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public bool Check(ICollection<DateTime> accessAttempts)
        {
            if (!accessAttempts.Any())
            {
                throw new InvalidOperationException("No access attempts were registered, nothing to check");
            }

            var lastAttempt = accessAttempts.Last();

            return accessAttempts.Reverse().Count(x => x > lastAttempt - TimeSpan) <= RequestsCount;
        }
    }
}
