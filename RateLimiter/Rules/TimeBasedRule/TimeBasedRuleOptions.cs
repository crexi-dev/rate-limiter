using System;

namespace RateLimiter.Rules.TimeBasedRule
{
    public class TimeBasedRuleOptions: RateLimitRuleOptions
    {
        public string Name { get; set; } = "TimeBasedRule";
        public TimeSpan MinTimeBetweenRequests { get; set; } = TimeSpan.FromSeconds(10);
    }
}
