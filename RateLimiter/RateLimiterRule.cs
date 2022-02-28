using RateLimiter.HistoryInspectors;
using RateLimiter.RequestFilters;
using RateLimiter.TokenMatchers;

namespace RateLimiter
{
	/// <summary>
	/// Represents a rule for the rate limiter to use to evaluate requests.
	/// </summary>
	public class RateLimiterRule<T>
	{
		/// <summary>
		/// Determines whether this rule applies to an access token.
		/// </summary>
		public ITokenMatcher<T> Matcher { get; }

		/// <summary>
		/// Filters a user's request history to only requests that are applicable to this rule.
		/// </summary>
		public IRequestFilter<T> Filter { get; }

		/// <summary>
		/// Given the filtered request history, this determines whether an incoming request
		/// should be rate limited.
		/// </summary>
		public IHistoryInspector<T> Inspector { get; }

		/// <summary>
		/// Constructs a new rule.
		/// </summary>
		public RateLimiterRule(ITokenMatcher<T> matcher, IRequestFilter<T> filter, IHistoryInspector<T> limiter)
		{
			Matcher = matcher;
			Filter = filter;
			Inspector = limiter;
		}

		/// <summary>
		/// Constructs a new rule with the default token matcher (matches all tokens).
		/// </summary>
		public RateLimiterRule(IRequestFilter<T> filter, IHistoryInspector<T> inspector) :
			this(new AllMatcher<T>(), filter, inspector)
		{
		}

		/// <summary>
		/// Constructs a new rule with the default request history filter (does not filter any history).
		/// </summary>
		public RateLimiterRule(ITokenMatcher<T> matcher, IHistoryInspector<T> inspector) :
			this(matcher, new EmptyFilter<T>(), inspector)
		{
		}

		/// <summary>
		/// Constructs a new rule with the default token matcher (matches all tokens)
		/// and default request history filter (does not filter any history).
		/// </summary>
		public RateLimiterRule(IHistoryInspector<T> inspector) :
			this(new AllMatcher<T>(), new EmptyFilter<T>(), inspector)
		{
		}
	}
}
