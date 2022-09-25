using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Services
{
    public class RateLimiterPolicyBuilder : IRateLimiterPolicyBuilder
    {
        public IList<IRateLimiterRequirement> Requirements { get; set; } = new List<IRateLimiterRequirement>();

        public RateLimits RateLimits { get; set; } = new RateLimits();

        public IRateLimiterPolicyBuilder RequireClaim(string claimName)
        {
            Requirements.Add(new RateLimiterClaimsRequirement(claimName));

            return this;
        }

        public IRateLimiterPolicyBuilder RequireAssertion(Func<RateLimiterHandlerContext, bool> handler)
        {
            Requirements.Add(new RateLimiterAssertionRequirement(handler));
            return this;
        }

        public IRateLimiterPolicyBuilder SetRateLimits(RateLimits limits)
        {
            this.RateLimits = limits;
            return this;
        }

        public IRateLimiterPolicyBuilder RequireLocale(string localeName)
        {
            Requirements.Add(new RateLimiterLocaleRequirement(localeName));
            return this;
        }

        public RateLimiterPolicy Build()
        {
            //return a policy according to the builder configuration
            var policy = new RateLimiterPolicy();
            policy.Requirements = Requirements;
            policy.RateLimits = RateLimits;
            return policy;
        }

        public RateLimiterPolicyBuilder()
        {

        }

    }
}
