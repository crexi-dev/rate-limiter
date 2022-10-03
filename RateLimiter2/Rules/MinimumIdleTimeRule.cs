using RateLimiter.Resources;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules
{
    public class MinimumIdleTimeRule: BaseRule, IRateRule
    {
        public TimeSpan IdleTime { get; set; }

        public override bool Evaluate(Resource resource, string clientToken, List<DateTime> requests, ref bool terminateRuleProcessing)
        {
            var filteredRequests = requests.Where(r => r > DateTime.UtcNow - IdleTime).ToList();

            if (filteredRequests.Count > 0)
                return false;

            return true;
        }
    }
}
