using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    public class SlidingWindowRateLimiterRule : IRateLimiterRule
    {
        public bool IsRequestAllowed(RequestModel currentRequest, List<RequestModel> requests, RuleModel rule)
        {
            double windowInSeconds = rule.Window == null ? 0 : ((TimeSpan)rule.Window).TotalSeconds;
            DateTime windowEnd = DateTime.UtcNow;
            DateTime windowStart = windowEnd.AddSeconds(-windowInSeconds);

            // Remove expired requests
            requests = requests.Where(r => r.TimeRequested >= windowStart && (rule.Locations == null ? true : rule.Locations.Contains(r.Location))).ToList();

            return requests.Count < rule.MaxRequests;
        }
    }
}
