using RateLimiter.Models.Configuration;
using System.Collections.Generic;

namespace RateLimiter.Builders.Configuration
{
    public sealed class ResourcePoliciesConfigurationBuilder
    {
        private string resourceName;
        private List<PolicyConfiguration> policies = new List<PolicyConfiguration>();

        public ResourcePoliciesConfigurationBuilder ForResource(string name)
        {
            resourceName = name;
            return this;
        }

        public ResourcePoliciesConfigurationBuilder AddPolicy(PolicyConfiguration policy)
        {
            policies.Add(policy);
            return this;
        }

        public ResourcePoliciesConfiguration Build() => new ResourcePoliciesConfiguration
        {
            ResourceName = resourceName,
            Policies = policies
        };
    }
}
