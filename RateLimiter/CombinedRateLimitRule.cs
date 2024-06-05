using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class CombinedRateLimitRule : RateLimitRule
    {
        private readonly List<RateLimitRule> _rules;

        public CombinedRateLimitRule(List<RateLimitRule> rules)
        {
            _rules = rules;
        }

        public override bool AllowRequest(string clientId)
        {
             return _rules.All(rule => rule.AllowRequest(clientId));
        }
    }

}
