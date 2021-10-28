using RateLimiter.Model.Enum;
using RateLimiter.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.ApiRule.RuleImplementations
{
    public class MaxRequestsPerTimespanRateLimitRule : IRuleValidation
    {
        protected int RequestTimespanInMinutes => 60;

        protected int MaxRequestsInTimespan => 2;

        public virtual bool Validate(string token, ResourceEnum resourceName, List<ApiRequest> tokenRequestLog)
        {

            if (tokenRequestLog == null)
                return true;
            var requestTimespan = DateTime.UtcNow.AddMinutes(RequestTimespanInMinutes * -1);

            var requestsWithinTimespan = tokenRequestLog
                .Where(x => x.ResourceName == resourceName)
                .Where(x => x.DateRequested >= requestTimespan);

            return requestsWithinTimespan.Count() < MaxRequestsInTimespan;
        }
    }
}


