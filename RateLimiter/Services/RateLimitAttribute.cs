using System;
namespace RateLimiter.Middleware
{
	public class RateLimitAttribute : Attribute
	{
        public RuleTypeEnum RuleType { get; set; }
        public int MaxRequests { get; set; }
    }
}

