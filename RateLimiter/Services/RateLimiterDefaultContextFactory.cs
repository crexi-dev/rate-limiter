using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Services
{
    public class RateLimiterDefaultContextFactory : IRateLimiterHandlerContextFactory
    {
        public virtual RateLimiterHandlerContext CreateContext(RateLimiterPolicy policy, IEnumerable<IRateLimiterRequirement> requirements, User user, IDateTimeWrapper requestDate, string requestPath)
        {
            return new RateLimiterHandlerContext(policy, requirements, user, requestDate, requestPath);
        }
    }
}
