using RateLimiter.RateLimitRules;
using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class RuleCollection 
    {
        public string RuleKey { get; private set; }
        public List<IRateLimitRule> RateLimitRules { get; set; }

        public RuleCollection(string ruleKey)
        {
            RuleKey = ruleKey;
            RateLimitRules = new List<IRateLimitRule>();
        }
    }
}
