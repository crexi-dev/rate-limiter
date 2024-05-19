using GuardNet;
using RateLimiter.Rules;
using System;

namespace RateLimiter.CustomGuards
{
    public static class CustomGuard
    {
        public static void IsValidRuleRequestInfoType<T>(Type current)
        { 
            Guard.For(() => current.IsAssignableTo(typeof(T)), new InvalidRuleRequestInfoTypeException());
        }
    }
}
