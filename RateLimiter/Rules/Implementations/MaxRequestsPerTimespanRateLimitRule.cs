using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RateLimiter.Rules
{
    public class MaxRequestsPerTimespanRateLimitRule : IRateLimitRule
    {
        protected int RequestTimespanInMinutes => int.Parse(ConfigurationManager.AppSettings["MaxRequestsPerTimespanRateLimitRule.RequestTimespanInMinutes"]);

        protected int MaxRequestsInTimespan => int.Parse(ConfigurationManager.AppSettings["MaxRequestsPerTimespanRateLimitRule.MaxRequestsInTimespan"]);

        public virtual bool Validate(string token, string resourceName, List<ApiRequest> tokenRequestLog)
        {
            if (tokenRequestLog == null)
                return true;

            var requestTimespan = DateTime.UtcNow.AddMinutes(RequestTimespanInMinutes * -1);

            var requestsWithinTimespan = tokenRequestLog
                .Where(x => x.ResourceName.ToLower() == resourceName.ToLower())
                .Where(x => x.DateRequested >= requestTimespan);

            return requestsWithinTimespan.Count() < MaxRequestsInTimespan;
        }
    }
}
