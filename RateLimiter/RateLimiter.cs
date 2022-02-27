using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter
{
	/// <summary>
	/// This is the entry point into the class library. It determines if a given request
	/// should be rate limited according to the rules assigned to the limiter. The user's
	/// request history is fetching using an injected data provider.
	/// </summary>
	/// <typeparam name="T">The type of the user identifier property of the access token.</typeparam>
	public class RateLimiter<T>
	{
		private readonly List<RateLimiterRule<T>> _rules;
		private readonly IApiRequestHistoryProvider<T> _historyProvider;

		/// <summary>
		/// A read-only collection of the rules used by the rate limiter.
		/// </summary>
		public IEnumerable<RateLimiterRule<T>> Rules => _rules.AsReadOnly();

		/// <summary>
		/// Constructs a new rate limiter.
		/// </summary>
		/// <param name="historyProvider">Provides the user's request history given their access token.</param>
		/// <param name="rules">The rules by which the rate limiter will evaluate whether a request should be rate limited.</param>
		public RateLimiter(IApiRequestHistoryProvider<T> historyProvider, IEnumerable<RateLimiterRule<T>> rules)
		{
			_rules = rules.ToList();
			_historyProvider = historyProvider;
		}

		/// <summary>
		/// Determines whether a request with the given access token should be rate limited.
		/// </summary>
		/// <returns>
		/// True if the user with the given access token has hit their rate limit.
		/// Generally this means they should be throttled, though exactly what to do
		/// with the result is up to the caller. False indicates that the user has not
		/// hit their rate limit and should be able to access the desired resource as normal.
		/// </returns>
		public async Task<bool> IsRateLimited(IAccessToken<T> accessToken, CancellationToken cancellationToken = default)
		{
			var matchingRules = _rules.Where(rule => rule.Matcher.MatchesToken(accessToken));
			if (!matchingRules.Any())
			{
				return false;
			}

			var history = await _historyProvider.GetApiRequestHistory(accessToken, cancellationToken);

			return matchingRules.Any(rule =>
			{
				var matchingHistory = history.Where(rule.Filter.IsRequestIncluded);
				return rule.Inspector.IsRateLimited(matchingHistory);
			});
		}
	}
}