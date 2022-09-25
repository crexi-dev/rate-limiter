using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class RateLimiterLocaleRequirement : RateLimiterHandler<RateLimiterLocaleRequirement>, IRateLimiterRequirement
    {
        public string LocaleName { get; }
        protected override Task HandleRequirementAsync(RateLimiterHandlerContext context, RateLimiterLocaleRequirement requirement)
        {
            if (context.User != null)
            {
                var found = false;

                found = context.User.Locale == LocaleName;

                if (found)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }

        public RateLimiterLocaleRequirement(string localeName)
        {
            LocaleName = localeName;
        }
    }
}
