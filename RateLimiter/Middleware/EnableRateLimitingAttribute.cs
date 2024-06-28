using System;
using RateLimiter.RateLimiter.Models;

namespace RateLimiter.Middleware
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class EnableRateLimitingAttribute : Attribute
    {
        public EnableRateLimitingAttribute(Region region, params string[] policyNames)
        {
            Policies = policyNames;
            Region = region;
        }

        public string[] Policies { get; set; }

        public Region Region { get; set; }
    }
}
