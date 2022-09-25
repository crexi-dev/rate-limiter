using System.Collections.Generic;

namespace RateLimiter.Models
{
    public class RateLimitRule
	{
		public string Resource { get; set; }
		public List<RequestsPerTimeSpanRule> RequestsPerTimeSpanRules { get; set; }
		public TimeSpanSinceLastCallRule TimeSpanSinceLastCallRule { get; set; }
		public List<RegionBasedRule> RegionBasedRules { get; set; }

		// Additional rules that apply to a resource goes here
	}
}
