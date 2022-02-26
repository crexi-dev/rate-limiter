using RateLimiter.TokenLimiters;
using RateLimiter.TokenSelectors;

namespace RateLimiter
{
	public class RateLimiterRule
	{
		public ITokenSelector Selector { get; }
		public ITokenLimiter Limiter { get; }

		public RateLimiterRule(ITokenSelector selector, ITokenLimiter limiter)
		{
			Selector = selector;
			Limiter = limiter;
		}
	}
}
