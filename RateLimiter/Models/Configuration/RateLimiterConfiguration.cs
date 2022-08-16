using System.Collections.Generic;

namespace RateLimiter.Models.Configuration
{
    public sealed class RateLimiterConfiguration
    {
        public IEnumerable<ResourcePoliciesConfiguration> ResourceConfigurations { get; set; }
    }
}
