using RateLimiter.Consts;
using System;

namespace RateLimiter.Models.Rules
{
    public sealed class TimeSpanSinceLastRequestRule : IRule
    {
        private TimeSpan timeSpanSinceLastRequestInMilliseconds;

        public TimeSpanSinceLastRequestRule(int timeSpanSinceLastRequestInSeconds)
        {
            timeSpanSinceLastRequestInMilliseconds = TimeSpan.FromSeconds(timeSpanSinceLastRequestInSeconds);
        }

        public RuleResult Execute(СlientStatistics сlientStatistics)
        {
            var ruleResult = new RuleResult();

            var lastRequestDateTime = сlientStatistics.LastRequest;
            if (lastRequestDateTime.HasValue)
            {
                var dateTime = DateTime.UtcNow;
                var elapsedMilliseconds = (dateTime - lastRequestDateTime.Value).TotalMilliseconds;

                if (elapsedMilliseconds < timeSpanSinceLastRequestInMilliseconds.TotalMilliseconds)
                {
                    ruleResult.Fail(Constants.TimeSpanSinceLastRequestRuleMessage);
                }
            }

            return ruleResult;
        }
    }
}
