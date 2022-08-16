namespace RateLimiter.Models.Configuration.Rules
{
    public sealed class TimeSpanSinceLastRequestRuleConfiguration : RuleConfiguration
    {
        public int TimeSpanSinceLastRequestInSeconds { get; set; }
    }
}
