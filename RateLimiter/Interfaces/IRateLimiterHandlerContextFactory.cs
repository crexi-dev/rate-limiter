using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Interfaces
{
    public interface IRateLimiterHandlerContextFactory
    {
        RateLimiterHandlerContext CreateContext(RateLimiterPolicy policy, IEnumerable<IRateLimiterRequirement> requirements, User user, IDateTimeWrapper requestDate, string requestPath);
    }
}
