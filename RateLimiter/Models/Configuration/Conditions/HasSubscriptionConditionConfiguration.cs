using RateLimiter.Models.Conditions;

namespace RateLimiter.Models.Configuration.Conditions
{
    public sealed class HasSubscriptionConditionConfiguration : ConditionConfiguration
    {
        public HasSubscriptionConditionConfiguration()
        {
            Type = ConditionType.HasSubscriptionCondition;
        }

        public bool HasSubscription { get; set; }
    }
}
