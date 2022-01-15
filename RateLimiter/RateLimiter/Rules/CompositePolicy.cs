using RateLimiter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimiter.Rules
{
    public sealed class CompositePolicy : IRateLimitPolicy
    {
        internal readonly IRateLimitPolicy[] rateLimitPolicies;

        public CompositePolicy(params IRateLimitPolicy[] rateLimitPolicies)
        {
            this.rateLimitPolicies = rateLimitPolicies ?? Array.Empty<IRateLimitPolicy>();
        }

        public bool Check(string accessToken, IEnumerable<UserRequest> userRequests, DateTime currentDate)
            => rateLimitPolicies.All(p => p.Check(accessToken, userRequests, currentDate));
    }
}
