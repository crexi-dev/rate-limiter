using RateLimiter.Models.Configuration.Conditions;
using RateLimiter.Models.Configuration.Rules;
using System.Collections.Generic;

namespace RateLimiter.Models.Configuration
{
    public sealed class PolicyConfiguration
    {
        public RuleConfiguration Rule { get; set; }

        public IEnumerable<ConditionConfiguration> Conditions { get; set; }
    }
}
