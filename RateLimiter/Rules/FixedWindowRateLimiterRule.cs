using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    public class FixedWindowRateLimiterRule : IRateLimiterRule
    {
        public bool IsRequestAllowed(RequestModel currentRequest, List<RequestModel> requests, RuleModel rule)
        {
            double windowInSeconds = rule.Window == null ? 0 : ((TimeSpan)rule.Window).TotalSeconds;
            DateTime windowStart = DateTime.UtcNow.AddSeconds(-windowInSeconds);

            int requestCount = requests.Count(r => r.TimeRequested >= windowStart && (rule.Locations == null ? true : rule.Locations.Contains(r.Location)));

            return requestCount < rule.MaxRequests;
        }
    }
}
