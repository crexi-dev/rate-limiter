using RateLimiter.Models.Rules;

namespace RateLimiter.Models.Configuration.Rules
{
    public sealed class TimeSpanSinceLastSuccessfulRequestRuleConfiguration : RuleConfiguration
    {
        public TimeSpanSinceLastSuccessfulRequestRuleConfiguration()
        {
            Type = RuleType.TimeSpanSinceLastSuccessfulRequestRule;
        }

        public int TimeSpanSinceLastSuccessfulRequestInSeconds { get; set; }
    }
}
