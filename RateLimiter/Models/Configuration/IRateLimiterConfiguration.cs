using System.Collections.Generic;

namespace RateLimiter.Models.Configuration
{
    public interface IRateLimiterConfiguration
    {
        public IEnumerable<ResourcePoliciesConfiguration> ResourceConfigurations { get; set; }
    }
}
