using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class RateLimiterClaimsRequirement : RateLimiterHandler<RateLimiterClaimsRequirement>, IRateLimiterRequirement
    {
        public string ClaimType { get; }

        protected override Task HandleRequirementAsync(RateLimiterHandlerContext context, RateLimiterClaimsRequirement requirement)
        {
            if (context.User != null)
            {
                var found = false;

                found = context.User.Claims.Any(c => string.Equals(c, requirement.ClaimType, StringComparison.OrdinalIgnoreCase));

                if (found)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }

        public RateLimiterClaimsRequirement(string claimType)
        {
            if (claimType == null)
            {
                throw new ArgumentNullException(nameof(claimType));
            }

            ClaimType = claimType;
        }
    }
}
