using RateLimiter.Models.Configuration.Rules;

namespace RateLimiter.Builders.Configuration
{
    public sealed class TimeSpanSinceLastSuccessfulRequestRuleConfigurationBuilder
    {
        private int timeSpan = 20;

        public TimeSpanSinceLastSuccessfulRequestRuleConfigurationBuilder WithTimeSpanInSeconds(int timeSpan)
        {
            this.timeSpan = timeSpan;
            return this;
        }

        public TimeSpanSinceLastSuccessfulRequestRuleConfiguration Build() => new()
        {
            TimeSpanSinceLastSuccessfulRequestInSeconds = timeSpan
        };
    }
}
