using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RateLimiter.Rules
{
    public class TimespanPassedRateLimitRule : IRateLimitRule
    {
        protected int TimespanPassedInMinutes => int.Parse(ConfigurationManager.AppSettings["TimespanPassedRateLimitRule.TimespanPassedInMinutes"]);

        public virtual bool Validate(string token, string resourceName, List<ApiRequest> tokenRequestLog)
        {
            if (tokenRequestLog == null)
                return true;

            var minTimeSinceLastRequest = DateTime.UtcNow.AddMinutes(TimespanPassedInMinutes * -1);

            var requestsWithinTimespan = tokenRequestLog
                .Where(x => x.ResourceName.ToLower() == resourceName.ToLower())
                .Where(x => x.DateRequested >= minTimeSinceLastRequest);

            return !requestsWithinTimespan.Any();
        }
    }
}
