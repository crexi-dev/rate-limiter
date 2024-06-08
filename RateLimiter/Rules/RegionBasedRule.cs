using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Rules
{
    public class RegionBasedRule : ILimitRule
    {
        private readonly ILimitRule _usRule;
        private readonly ILimitRule _euRule;

        public RegionBasedRule(ILimitRule usRule, ILimitRule euRule)
        {
            _usRule = usRule;
            _euRule = euRule;
        }

        public bool IsRequestAllowed(string clientId, string ruleName)
        {
            string region = GetRegionFromClientId(clientId);

            return region switch
            {
                "US" => _usRule.IsRequestAllowed(clientId, ruleName),
                "EU" => _euRule.IsRequestAllowed(clientId, ruleName),
                _ => true
            };
        }

        private static string GetRegionFromClientId(string clientId) => clientId.StartsWith("US") ? "US" : "EU";
    }
}
