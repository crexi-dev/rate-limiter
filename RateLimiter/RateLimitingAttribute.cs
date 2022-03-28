using RateLimiter.Enums;
using System;

namespace RateLimiter
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RateLimitingAttribute : Attribute
    {
        public int MaxNumberRequest { get; set; }
        public RestrictionTypeEnum Restriction { get; set; }
        
    }
}
