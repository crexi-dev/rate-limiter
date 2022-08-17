using RateLimiter.Models.Conditions;
using RateLimiter.Models.Configuration.Conditions;
using System;

namespace RateLimiter.Factories
{
    public sealed class ConditionFactory
    {
        public ICondition Create(ConditionConfiguration conditionConfiguration)
        {
            return conditionConfiguration switch
            {
                RegionConditionConfiguration regionConditionConfiguration => new RegionCondition(regionConditionConfiguration.Region),
                IsClientAuthenticatedConditionConfiguration isClientAuthenticatedConditionConfiguration => 
                    new IsClientAuthenticatedCondition(isClientAuthenticatedConditionConfiguration.IsClientAuthenticated),
                HasRoleConditionConfiguration hasRoleConditionConfiguration => new HasRoleCondition(hasRoleConditionConfiguration.Role),
                HasSubscriptionConditionConfiguration hasSubscriptionConditionConfiguration => 
                    new HasSubscriptionCondition(hasSubscriptionConditionConfiguration.HasSubscription),
                _ => throw new InvalidCastException()
            };
        }
    }
}
