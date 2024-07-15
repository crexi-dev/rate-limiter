using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Model;

namespace RateLimiter.Rule
{
    public class MinimumBetweenRequestWindowRule : RateLimitRuleBase
    {
        public MinimumBetweenRequestWindowRule(TimeSpan durationFromLastCall)
        {
            DurationFromLastCall = durationFromLastCall;
        }

        public TimeSpan DurationFromLastCall { get; }

        public override bool CheckRequestAllow(IEnumerable<RateLimitRequest> requestData)
        {
            var lastTwoRequests = (requestData ?? Enumerable.Empty<RateLimitRequest>()).TakeLast(2).ToList();

            if (DurationFromLastCall <= TimeSpan.Zero || lastTwoRequests.Count < 2)
                return true;

            return lastTwoRequests[1].Timestamp - lastTwoRequests[0].Timestamp >= this.DurationFromLastCall;
        }
    }
}
