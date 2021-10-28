using RateLimiter.Model.Enum;
using RateLimiter.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.ApiRule.RuleImplementations
{
    public class TimespanPassedRateLimitRule : IRuleValidation
    {

        protected int TimespanPassedInMinutes => 1;

        public virtual bool Validate(string token, ResourceEnum resourceName, List<ApiRequest> tokenRequestLog)
        {

            if (tokenRequestLog == null)
                return true;
            var minTimeSinceLastRequest = DateTime.UtcNow.AddMinutes(TimespanPassedInMinutes * -1);

            var requestsWithinTimespan = tokenRequestLog
                .Where(x => x.ResourceName == resourceName)
                .Where(x => x.DateRequested >= minTimeSinceLastRequest);

            return !requestsWithinTimespan.Any();
        }
    }
}
