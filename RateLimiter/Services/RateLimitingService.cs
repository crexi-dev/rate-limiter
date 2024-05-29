using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.Services
{
    public class RateLimitingService
    {
        private readonly IEnumerable<IRule> _rules;

        public RateLimitingService(IEnumerable<IRule> rules)
        {
            _rules = rules;
        }

        public bool IsRequestAllowed(string token)
        {
            foreach (var rule in _rules)
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
