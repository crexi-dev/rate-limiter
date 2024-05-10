using RateLimiter.Interfaces;
using System;

namespace RateLimiter.Classes.Rules
{
    public class GeographicConditionalRule : IRateLimitRule
    {
        private readonly IRateLimitRule _usRule;
        private readonly IRateLimitRule _euRule;
        private readonly Func<string, string> _getRegionByToken;

        public GeographicConditionalRule(IRateLimitRule usRule, IRateLimitRule euRule, Func<string, string> getRegionByToken)
        {
            _usRule = usRule;
            _euRule = euRule;
            _getRegionByToken = getRegionByToken;
        }

        public bool IsRequestAllowed(string token, string resource)
        {
            var region = _getRegionByToken(token);
            return region switch
            {
                "US" => _usRule.IsRequestAllowed(token, resource),
                "EU" => _euRule.IsRequestAllowed(token, resource),
                _ => true, // Default to allow if region not recognized
            };
        }
    }
}
