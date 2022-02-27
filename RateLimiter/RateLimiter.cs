using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RateLimiter
{
	public class RateLimiter<T>
	{
		private readonly List<RateLimiterRule<T>> _rules;
		private readonly IApiRequestHistoryProvider<T> _historyProvider;

		public IEnumerable<RateLimiterRule<T>> Rules => _rules.AsReadOnly();

		public RateLimiter(IApiRequestHistoryProvider<T> historyProvider, IEnumerable<RateLimiterRule<T>> rules)
		{
			_rules = rules.ToList();
			_historyProvider = historyProvider;
		}

		public async Task<bool> IsRateLimited(IAccessToken<T> accessToken)
		{
			var matchingRules = _rules.Where(rule => rule.Matcher.MatchesToken(accessToken));
			if (!matchingRules.Any())
			{
				return false;
			}

			var history = await _historyProvider.GetApiRequestHistory(accessToken);
			return matchingRules.Any(rule => rule.Inspector.IsRateLimited(history));
		}
	}
}
