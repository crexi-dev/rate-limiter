using RateLimiter.Models.Configuration.Rules;

namespace RateLimiter.Builders.Configuration
{
    public sealed class TimeSpanSinceLastRequestRuleConfigurationBuilder
    {
        private int timeSpan = 20;

        public TimeSpanSinceLastRequestRuleConfigurationBuilder WithTimeSpanInSeconds(int timeSpan)
        {
            this.timeSpan = timeSpan;
            return this;
        }

        public TimeSpanSinceLastRequestRuleConfiguration Build() => new()
        {
            TimeSpanSinceLastRequestInSeconds = timeSpan
        };
    }
}
