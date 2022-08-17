using RateLimiter.Models.Rules;

namespace RateLimiter.Models.Configuration.Rules
{
    public sealed class RequestsPerTimeSpanRuleConfiguration : RuleConfiguration
    {
        public RequestsPerTimeSpanRuleConfiguration()
        {
            Type = RuleType.RequestsPerTimeSpanRule;
        }

        public int MaxRequests { get; set; }

        public int TimeSpanInSeconds { get; set; }
    }
}
