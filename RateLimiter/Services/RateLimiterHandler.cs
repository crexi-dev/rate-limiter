using System;
using System.Collections.Generic;
using System.Text;
using RateLimiter.Interfaces;

namespace RateLimiter.Services
{
    using RateLimiter.Interfaces;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class RateLimiterHandler<TRequirement> : IRateLimiterHandler
            where TRequirement : IRateLimiterRequirement
    {
        public virtual async Task HandleAsync(RateLimiterHandlerContext context)
        {
            foreach (var req in context.Requirements.OfType<TRequirement>())
            {
                await HandleRequirementAsync(context, req).ConfigureAwait(false);
            }
        }

        protected abstract Task HandleRequirementAsync(RateLimiterHandlerContext context, TRequirement requirement);
    }

    public abstract class RateLimiterHandler<TRequirement, TResource> : IRateLimiterHandler
        where TRequirement : IRateLimiterRequirement
    {
        public virtual async Task HandleAsync(RateLimiterHandlerContext context)
        {
            if (context.Resource is TResource)
            {
                foreach (var req in context.Requirements.OfType<TRequirement>())
                {
                    await HandleRequirementAsync(context, req, (TResource)context.Resource).ConfigureAwait(false);
                }
            }
        }

        protected abstract Task HandleRequirementAsync(RateLimiterHandlerContext context, TRequirement requirement, TResource resource);
    }
}
