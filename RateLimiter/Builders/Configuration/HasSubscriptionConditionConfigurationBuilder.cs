using RateLimiter.Models.Configuration.Conditions;

namespace RateLimiter.Builders.Configuration
{
    public sealed class HasSubscriptionConditionConfigurationBuilder
    {
        public HasSubscriptionConditionConfiguration Build() => new()
        {
            HasSubscription = true
        };
    }
}
