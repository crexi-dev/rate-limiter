using System;
using System.Diagnostics.CodeAnalysis;
using RateLimiter.Models.Enums;

namespace RateLimiter.Models.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RateLimiterAttribute : Attribute
    {
        public RateLimiterType RateLimiterType { get; set; }
    }
}
