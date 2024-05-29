using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.Services
{
    public class RateLimitingService
    {
        private readonly Dictionary<string, List<IRule>> _rulesByRegion;

        public RateLimitingService(Dictionary<string, List<IRule>> dictionary)
        {
            this._rulesByRegion = dictionary;
        }

        public bool IsRequestAllowed(string token)
        {
            string regionPrefix = token.Split('-')[0];
            var regionRules = _rulesByRegion[regionPrefix];
            foreach (var rule in regionRules)
            {
                if (!rule.IsAllowed(token))
                {
                    return false;
                }
            }

            return true;
        }
    }

}
