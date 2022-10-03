

using RateLimiter.Resources;

namespace RateLimiter.Rules.Interfaces
{
    public interface IRateRule
    {
        string Name { get; }

        // Evaluate a rate rule for the current request.  A rule can terminate
        // further processing of rules by setting terminateRuleProcessing = true
        public bool Evaluate(Resource resource, string clientToken, List<DateTime> requests, ref bool terminateRuleProcessing);
    }
}
