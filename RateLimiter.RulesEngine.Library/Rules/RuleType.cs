using System;

namespace RateLimiter.RulesEngine.Library.Rules
{
    public enum RuleType {
        RateLimit,
        Resource,
        Region,
        IPRestrict,
        Whitelist,
        Bot
    }
}