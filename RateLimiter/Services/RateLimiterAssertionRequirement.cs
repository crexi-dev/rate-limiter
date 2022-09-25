using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class RateLimiterAssertionRequirement : RateLimiterHandler<RateLimiterAssertionRequirement>, IRateLimiterRequirement
    {
        /// <summary>
        /// Function that is called to handle this requirement.
        /// </summary>
        public Func<RateLimiterHandlerContext, Task<bool>> Handler { get; }

        protected override async Task HandleRequirementAsync(RateLimiterHandlerContext context, RateLimiterAssertionRequirement requirement)
        {
            if (await Handler(context).ConfigureAwait(false))
            {
                context.Succeed(this);
            }
        }

        public RateLimiterAssertionRequirement(Func<RateLimiterHandlerContext, bool> handler)
        {
            Handler = context => Task.FromResult(handler(context));
        }
    }
}
