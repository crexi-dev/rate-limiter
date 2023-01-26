using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Models;
using RateLimiter.Rule.Contracts;

namespace RateLimiter.Rule
{
    public class PrefixReqiestPerTimespanRule : IRule
    {
        public long RequestsLimit { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public string Prefix { get; set; }
        public bool ValidateRuleByStatistic(ClientStatistic statistic)
        {
            if (statistic.PrefixRequest.TryGetValue(Prefix, out List<DateTime>? value))
            {
                var count = value.Count(x => x > DateTime.UtcNow - TimeSpan);
                return RequestsLimit >= count;
            }
            return true;
        }
    }
}