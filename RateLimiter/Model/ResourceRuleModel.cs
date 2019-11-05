using System;

namespace RateLimiter.Model
{
    public class ResourceRuleModel
    {
        public string ResourceName { get; set; }

        public int LimitRequests { get; set; }

        public TimeSpan TimeLimit { get; set; }
        
        public TimeSpan? DifferenceBetweenRequests { get; set; }
    }
}
