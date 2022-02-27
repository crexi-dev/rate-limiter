using RateLimiter.HistoryFilters;
using RateLimiter.HistoryInspectors;
using RateLimiter.TokenMatchers;

namespace RateLimiter
{
	public class RateLimiterRule<T>
	{
		public ITokenMatcher<T> Matcher { get; }
		public IHistoryFilter<T> Filter { get; set; }
		public IHistoryInspector<T> Inspector { get; }

		public RateLimiterRule(ITokenMatcher<T> matcher, IHistoryFilter<T> filter, IHistoryInspector<T> limiter)
		{
			Matcher = matcher;
			Filter = filter;
			Inspector = limiter;
		}

		public RateLimiterRule(IHistoryFilter<T> filter, IHistoryInspector<T> inspector) :
			this(new AllMatcher<T>(), filter, inspector)
		{
		}

		public RateLimiterRule(ITokenMatcher<T> matcher, IHistoryInspector<T> inspector) :
			this(matcher, new EmptyFilter<T>(), inspector)
		{
		}

		public RateLimiterRule(IHistoryInspector<T> inspector) :
			this(new AllMatcher<T>(), new EmptyFilter<T>(), inspector)
		{
		}
	}
}
