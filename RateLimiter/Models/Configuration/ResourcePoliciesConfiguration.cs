using System.Collections.Generic;

namespace RateLimiter.Models.Configuration
{
    public sealed class ResourcePoliciesConfiguration
    {
        public string ResourceName { get; set; }

        public IEnumerable<PolicyConfiguration> Policies { get; set; }
    }
}
