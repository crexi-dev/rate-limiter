using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class RateLimitRuleOptions
	{
		public Dictionary<string, RateLimitRule>? Rules { get; set; }
		// additional rules that are global goes here
	}
}
