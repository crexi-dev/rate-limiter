using RateLimiter.Consts;
using System;

namespace RateLimiter.Models.Rules
{
    public sealed class TimeSpanSinceLastRequestRule : IRule
    {
        private const string message = "Enough time has not elapsed since the last request!";

        private TimeSpan timeSpanSinceLastRequestInMilliseconds;

        public TimeSpanSinceLastRequestRule(int timeSpanSinceLastRequestInSeconds)
        {
            timeSpanSinceLastRequestInMilliseconds = TimeSpan.FromMilliseconds(timeSpanSinceLastRequestInSeconds);
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
