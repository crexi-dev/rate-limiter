using RateLimiter.Models;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class GeographicBlockRateLimiterRule : IRateLimiterRule
    {
        public bool IsRequestAllowed(RequestModel currentRequest, List<RequestModel> requests, RuleModel rule)
        {
            if(string.IsNullOrEmpty(currentRequest.Location) || rule.Locations == null)
            {
                return true;
            }

            return !rule.Locations.Contains(currentRequest.Location);
        }
    }
}
