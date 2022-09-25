using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class RateLimitRule
	{
		public string Resource { get; set; }
		public RequestsPerTimeSpanRule RequestsPerTimeSpanRule { get; set; }
		public TimeSpanSinceLastCallRule TimeSpanSinceLastCallRule { get; set; }
		public List<RegionBasedRule> RegionBasedRules { get; set; }

		// Additional rules that apply to a resource goes here
	}
}
