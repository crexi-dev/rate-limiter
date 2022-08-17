using RateLimiter.Models.Configuration.Rules;

namespace RateLimiter.Builders.Configuration
{
    public sealed class TimeSpanSinceLastSuccessfulRequestRuleConfigurationBuilder
    {
        public TimeSpanSinceLastSuccessfulRequestRuleConfiguration Build() => new()
        {
            TimeSpanSinceLastSuccessfulRequestInSeconds = 20
        };
    }
}
