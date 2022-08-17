using RateLimiter.Models.Configuration;
using System.Collections.Generic;

namespace RateLimiter.Builders.Configuration
{
    public sealed class RateLimiterConfigurationBuilder
    {
        private List<ResourcePoliciesConfiguration> resourceConfigurations = new List<ResourcePoliciesConfiguration>();

        public RateLimiterConfigurationBuilder AddResourcePoliciesConfiguration(ResourcePoliciesConfiguration resourceConfiguration)
        {
            resourceConfigurations.Add(resourceConfiguration);
            return this;
        }

        public RateLimiterConfiguration Build() => new RateLimiterConfiguration
        {
            ResourceConfigurations = resourceConfigurations
        };
    }
}
