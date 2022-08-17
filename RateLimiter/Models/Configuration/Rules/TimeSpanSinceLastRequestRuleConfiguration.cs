using RateLimiter.Models.Rules;

namespace RateLimiter.Models.Configuration.Rules
{
    public sealed class TimeSpanSinceLastRequestRuleConfiguration : RuleConfiguration
    {
        public TimeSpanSinceLastRequestRuleConfiguration()
        {
            Type = RuleType.TimeSpanSinceLastRequestRule;
        }

        public int TimeSpanSinceLastRequestInSeconds { get; set; }
    }
}
