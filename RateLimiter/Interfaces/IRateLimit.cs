using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Models
{
    public interface IRateLimit
    {
        public void SetDateTime(DateTime dateTime);
        public void SetRules(List<RateLimitRule> rules);
        public void SetRegions(List<RateLimitRegion> regions);        
        public void SetResources(List<RateLimitResource> resources);

        public void SetExistingRequests(List<RateLimitRequest> requests);
        public bool ValidateToken(RateLimitToken token);
    }
}
