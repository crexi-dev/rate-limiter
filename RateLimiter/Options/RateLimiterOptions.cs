using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RateLimiter.Models.Enums;
using RateLimiter.Models.Rules;

namespace RateLimiter.Options
{
    [ExcludeFromCodeCoverage]
    public class RateLimiterOptions
    {
        public Dictionary<RateLimiterType, RateLimiterRuleBase> RateLimiterRules { get; set; }
    }
}
