using System;

namespace RateLimiter.RulesEngine.Library
{
    [Flags]
    public enum Region
    {
        All = 0,
        US = 1,
        EU = 2,
        AF = 4,
        AU = 8
    }
}