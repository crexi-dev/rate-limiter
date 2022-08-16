using RateLimiter.Consts;
using System;
using System.Linq;

namespace RateLimiter.Models.Rules
{
    public sealed class RequestsPerTimeSpanRule : IRule
    {
        private int maxRequests;
        private int timeSpanInSeconds;

        public RequestsPerTimeSpanRule(int maxRequests, int timeSpanInSeconds)
        {
            this.maxRequests = maxRequests;
            this.timeSpanInSeconds = timeSpanInSeconds;
        }

        public RuleResult Execute(СlientStatistics сlientStatistics)
        {
            var ruleResult = new RuleResult();

            var dateTime = DateTime.UtcNow;
            var startDateTime = dateTime.AddSeconds(-timeSpanInSeconds);

            var requestsCountInThePast = сlientStatistics.RequestsHistory.Count(requestDateTime => requestDateTime > startDateTime);

            if (requestsCountInThePast + 1 > maxRequests)
            {
                ruleResult.Fail(Constants.RequestsPerTimeSpanRuleMessage);
            }

            return ruleResult;
        }
    }
}
