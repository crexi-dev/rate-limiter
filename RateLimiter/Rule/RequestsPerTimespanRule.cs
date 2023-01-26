using System;
using System.Linq;
using RateLimiter.Models;
using RateLimiter.Rule.Contracts;

namespace RateLimiter.Rule
{
    public class RequestsPerTimespanRule : IRule
    {
        public long RequestsLimit { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public bool ValidateRuleByStatistic(ClientStatistic statistic)
        {
            var count = statistic.Requests.Count(x => x > DateTime.UtcNow - TimeSpan);
            return RequestsLimit >= count;
        }
    }
}