namespace RateLimiter.Models.Configuration.Rules
{
    public sealed class RequestsPerTimeSpanRuleConfiguration : RuleConfiguration
    {
        public int MaxRequests { get; set; }

        public int TimeSpanInSeconds { get; set; }
    }
}
