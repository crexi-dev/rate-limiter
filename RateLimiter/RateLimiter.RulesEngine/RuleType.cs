using System;

namespace RateLimiter.RulesEngine
{
    public enum RuleType {
        Resource,
        Region,
        IPRestrict,
        Whitelist,
        Bot
    }
}