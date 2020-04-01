using System;

namespace RateLimiter.RulesEngine.Library
{
    [Flags]
    public enum RuleType
    {
        Resource = 0,
        Region = 1,
        Whitelist = 2
    }
}