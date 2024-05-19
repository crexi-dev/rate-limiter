using GuardNet;
using RateLimiter.Rules;
using System;

namespace RateLimiter.CustomGuards
{
    public static class CustomGuard
    {
        public static void IsValidRuleRequestInfoType<T>(object current)
        { 
            Guard.For(() => typeof(T).IsInstanceOfType(current), new InvalidRuleRequestInfoTypeException());
        }
    }
}
