using RateLimiter.Models.Configuration.Rules;

namespace RateLimiter.Builders.Configuration
{
    public sealed class RequestsPerTimeSpanRuleConfigurationBuilder
    {
        public RequestsPerTimeSpanRuleConfiguration Build() => new()
        {
            MaxRequests = 3,
            TimeSpanInSeconds = 60
        };
    }
}
