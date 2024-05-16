using RateLimiter.Storage;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    /// <summary>
    /// This rule is used to apply rules depending on the token origin country.
    /// </summary>
    public class CountryBasedRule : IRateLimitRule
    {
        private readonly Dictionary<string, IEnumerable<IRateLimitRule>> _rules;

        public CountryBasedRule(Dictionary<string, IEnumerable<IRateLimitRule>> rules)
        {
            _rules = rules;
        }

        public bool IsRequestAllowed(string resource, string token)
        {
            var country = DataStorage.GetTokenOrigin(token);
            if (!_rules.TryGetValue(country, out var rules))
            {
                return true;
            }

            return rules.All(x => x.IsRequestAllowed(resource, token));
        }
    }
}
