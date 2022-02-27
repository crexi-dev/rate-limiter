using RateLimiter.TokenLimiters;
using RateLimiter.TokenSelectors;

namespace RateLimiter
{
	public class RateLimiterRule<T>
	{
		public ITokenSelector<T> Selector { get; }
		public ITokenLimiter<T> Limiter { get; }

		public RateLimiterRule(ITokenSelector<T> selector, ITokenLimiter<T> limiter)
		{
			Selector = selector;
			Limiter = limiter;
		}
	}
}
