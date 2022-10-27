using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Rules
{
    public class NumberOfRequestsPerTimespan : RateLimitRule
    {
        public override bool IsValid(List<DateTime> requestHistory)
        {
            var isValid = true;
            var now = DateTime.UtcNow;

            var actualCount = requestHistory.Count(r => now - r <= TimeSpan);

            if (actualCount > RequestCount)
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
