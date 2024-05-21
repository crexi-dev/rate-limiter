using RateLimiter.Nugget.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimitingRules
{
    public class TimespanPassedSinceLastCallRule : IRateLimitRule<TimespanPassedSinceLastCallRule>
    {
        public bool IsRateLimitExceeded(string clientId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetRoutes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TimespanPassedSinceLastCallRule> GetAllRoutes()
        {
            throw new NotImplementedException();
        }
    }
}
