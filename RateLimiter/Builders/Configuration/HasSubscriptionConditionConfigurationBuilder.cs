using RateLimiter.Models.Configuration.Conditions;

namespace RateLimiter.Builders.Configuration
{
    public sealed class HasSubscriptionConditionConfigurationBuilder
    {
        private bool hasSubscription = true;

        public HasSubscriptionConditionConfigurationBuilder WithSubscription(bool hasSubscription)
        {
            this.hasSubscription = hasSubscription;
            return this;
        }

        public HasSubscriptionConditionConfiguration Build() => new()
        {
            HasSubscription = hasSubscription
        };
    }
}
