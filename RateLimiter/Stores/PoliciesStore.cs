using RateLimiter.Models.Policies;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Stores
{
    public sealed class PoliciesStore
    {
        private readonly IEnumerable<ResourcePolicies> resourcePolicies;

        public PoliciesStore(IEnumerable<ResourcePolicies> resourcePolicies)
        {
            this.resourcePolicies = resourcePolicies;
        }

        public IEnumerable<IPolicy> GetPolicies(string resourceName)
        {
            return resourcePolicies
                .Where(resourcePolicy => resourcePolicy.ResourceName == resourceName)
                .SelectMany(resourcePolicy => resourcePolicy.Policies)
                .ToList();
        }
    }
}
