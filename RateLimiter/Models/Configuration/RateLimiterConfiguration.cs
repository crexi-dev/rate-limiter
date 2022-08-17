using System.Collections.Generic;

namespace RateLimiter.Models.Configuration
{
    public sealed class RateLimiterConfiguration : IRateLimiterConfiguration
    {
        public IEnumerable<ResourcePoliciesConfiguration> ResourceConfigurations { get; set; }
    }
}
