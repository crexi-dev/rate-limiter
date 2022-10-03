using RateLimiter.Resources;
using RateLimiter.Rules.Interfaces;
using System.Net;

namespace RateLimiter.Rules
{
    public class MaximumRequestRateRule : BaseRule, IRateRule
    {
        public int RequestCount { get; set; }
        public TimeSpan ElapsedTime { get; set; }  

        public override bool Evaluate(Resource resource, string clientToken, List<DateTime> requests, ref bool terminateRuleProcessing)
        {
            var filteredRequests = requests.Where(r => r > DateTime.UtcNow - ElapsedTime).ToList();

            if (filteredRequests.Count >= RequestCount)
                return false;
            return true;
        }
    }
}
