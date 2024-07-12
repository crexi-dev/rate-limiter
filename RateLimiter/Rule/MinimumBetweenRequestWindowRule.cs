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
            var latestAllowTimestamp = DateTime.UtcNow.Add(-this.DurationFromLastCall);

            return DurationFromLastCall <= TimeSpan.Zero
                || (requestData.LastOrDefault()?.Timestamp ?? DateTime.MinValue) <= latestAllowTimestamp;
        }
    }
}
