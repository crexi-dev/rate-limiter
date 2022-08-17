using RateLimiter.Consts;
using System;

namespace RateLimiter.Models.Rules
{
    public sealed class TimeSpanSinceLastSuccessfulRequestRule : IRule
    {
        private TimeSpan timeSpanSinceLastSuccessfulRequestInMilliseconds;

        public TimeSpanSinceLastSuccessfulRequestRule(int timeSpanSinceLastSuccessfulRequestInSeconds)
        {
            timeSpanSinceLastSuccessfulRequestInMilliseconds = TimeSpan.FromSeconds(timeSpanSinceLastSuccessfulRequestInSeconds);
        }

        public RuleResult Execute(СlientStatistics сlientStatistics)
        {
            var ruleResult = new RuleResult();

            var lastSuccessfulRequestDateTime = сlientStatistics.LastSuccessfulRequest;
            if (lastSuccessfulRequestDateTime.HasValue)
            {
                var dateTime = DateTime.UtcNow;
                var elapsedMilliseconds = (dateTime - lastSuccessfulRequestDateTime.Value).TotalMilliseconds;

                if (elapsedMilliseconds < timeSpanSinceLastSuccessfulRequestInMilliseconds.TotalMilliseconds)
                {
                    ruleResult.Fail(Constants.TimeSpanSinceLastSuccessfulRequestRuleMessage);
                }
            }

            return ruleResult;
        }
    }
}
