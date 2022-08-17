using RateLimiter.Models.Configuration.Rules;

namespace RateLimiter.Builders.Configuration
{
    public sealed class TimeSpanSinceLastRequestRuleConfigurationBuilder
    {
        public TimeSpanSinceLastRequestRuleConfiguration Build() => new()
        {
            TimeSpanSinceLastRequestInSeconds = 20
        };
    }
}
