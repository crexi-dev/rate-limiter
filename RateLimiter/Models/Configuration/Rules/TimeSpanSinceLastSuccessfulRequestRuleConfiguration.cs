namespace RateLimiter.Models.Configuration.Rules
{
    public sealed class TimeSpanSinceLastSuccessfulRequestRuleConfiguration : RuleConfiguration
    {
        public int TimeSpanSinceLastSuccessfulRequestInSeconds { get; set; }
    }
}
