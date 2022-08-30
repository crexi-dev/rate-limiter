using System.Diagnostics.CodeAnalysis;
using RateLimiter.Models.Enums;

namespace RateLimiter.Models.Rules
{
    [ExcludeFromCodeCoverage]
    public abstract class RateLimiterRuleBase
    {
        public abstract RateLimiterType RateLimiterType { get; }
    }
}
