using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Rules
{
    public class TimeSpanBetweenTwoRequests : RateLimitRule
    {
        public override bool IsValid(List<DateTime> requestHistory)
        {
            var lastRequest = requestHistory.OrderByDescending(r => r).Skip(1).FirstOrDefault();

            if (DateTime.UtcNow - lastRequest <= TimeSpan)
            {
                return false;
            }

            return true;
        }
    }
}
