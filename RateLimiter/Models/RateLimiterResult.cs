using RateLimiter.Models.Policies;
using System.Collections.Generic;

namespace RateLimiter.Models
{
    public sealed class RateLimiterResult
    {
        private List<PolicyResult> policyResults = new List<PolicyResult>();
        
        public bool IsRateLimited { get; private set; }

        public string LimitationReason { get; private set; }

        public void Record(PolicyResult policyResult)
        {
            policyResults.Add(policyResult);

            IsRateLimited = policyResult.IsFailed;
            LimitationReason = policyResult.Message;
        }
    }
}
