using RateLimiter.RateLimiter.Rules;
using System.Collections.Generic;

namespace RateLimiter.RateLimiter.Builders
{   
    public sealed class PolicyBuilder
    {
        private readonly List<IRateLimitPolicy> policies = new();

        public PolicyBuilder AddPolicy(IRateLimitPolicy rateLimitPolicy)
        {
            policies.Add(rateLimitPolicy);

            return this;
        }

        public IRateLimitPolicy Build()
        {
            return new CompositePolicy(policies.ToArray());
        }
    }
}
