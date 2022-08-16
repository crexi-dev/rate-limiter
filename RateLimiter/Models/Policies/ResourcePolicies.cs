using System;
using System.Collections.Generic;

namespace RateLimiter.Models.Policies
{
    public sealed class ResourcePolicies
    {
        public string ResourceName { get; set; }

        public IEnumerable<IPolicy> Policies { get; set; }
    }
}
