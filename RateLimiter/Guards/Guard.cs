using RateLimiter.Rules;
using System;

namespace RateLimiter.Guards;

internal static class Guard
{
    public static void RequestInfoType<T>(RuleRequestInfo info) where T : RuleRequestInfo
    {
        if (info is not T) throw new InvalidRuleRequestInfoType();
    }

}
