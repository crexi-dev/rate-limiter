using RateLimiter.Resources;
using RateLimiter.Rules.Extensions;
using RateLimiter.Rules.Interfaces;
using System.Net;

namespace RateLimiter.Rules
{
    public class TokenInRegionRule : BaseRule, IRateRule
    {
        // Region where this rule applies
        public string Region = String.Empty;

        // Rule to evalute in the region
        public IRateRule? RegionalRule { get; set; }

        public override bool Evaluate(Resource resource, string clientToken, List<DateTime> requests, ref bool terminateRuleProcessing)
        {
            // Is the token in our region
            if (clientToken.TokenInRegion(Region))
            {
                if (RegionalRule == null)
                    throw new Exception($"No regional rule defined for region {Region}");

                // Evaluate the regional rule
                var result = RegionalRule.Evaluate(resource, clientToken, requests, ref terminateRuleProcessing);

                // Dont process any more rules after this one
                terminateRuleProcessing = true;
                return result;
            }

            // Not in region so allow it
            return true;
        }
    }
}