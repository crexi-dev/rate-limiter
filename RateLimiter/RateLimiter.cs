using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
	public class RateLimiter
	{
		private readonly List<RateLimiterRule> _rules;
		public IEnumerable<RateLimiterRule> Rules => _rules.AsReadOnly();

		public RateLimiter(IEnumerable<RateLimiterRule> rules)
		{
			_rules = rules.ToList();
		}

		bool IsRateLimited(IAccessToken accessToken) =>
			_rules
				.Where(rule => rule.Selector.IsSelected(accessToken))
				.Any(rule => rule.Limiter.IsRateLimited(accessToken));
	}
}
