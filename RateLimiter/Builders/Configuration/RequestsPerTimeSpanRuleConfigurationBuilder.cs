using RateLimiter.Models.Configuration.Rules;

namespace RateLimiter.Builders.Configuration
{
    public sealed class RequestsPerTimeSpanRuleConfigurationBuilder
    {
        private int maxRequests = 5;

        public RequestsPerTimeSpanRuleConfigurationBuilder WithMaxRequests(int requests)
        {
            maxRequests = requests;
            return this;
        }

        public RequestsPerTimeSpanRuleConfiguration Build() => new()
        {
            MaxRequests = maxRequests,
            TimeSpanInSeconds = 20
        };
    }
}
