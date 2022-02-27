using RateLimiter.HistoryInspectors;
using RateLimiter.TokenMatchers;

namespace RateLimiter
{
	public class RateLimiterRule<T>
	{
		public ITokenMatcher<T> Matcher { get; }
		public IHistoryInspector<T> Inspector { get; }

		public RateLimiterRule(ITokenMatcher<T> matcher, IHistoryInspector<T> limiter)
		{
			Matcher = matcher;
			Inspector = limiter;
		}
	}
}
