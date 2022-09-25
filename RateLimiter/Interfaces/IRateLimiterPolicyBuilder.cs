using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiterPolicyBuilder
    {
        IRateLimiterPolicyBuilder RequireClaim(string claimName);

        IRateLimiterPolicyBuilder RequireLocale(string cultureName);

        IRateLimiterPolicyBuilder RequireAssertion(Func<RateLimiterHandlerContext, bool> assertion);
    }
}
