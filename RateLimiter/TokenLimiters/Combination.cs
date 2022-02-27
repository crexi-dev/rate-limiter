using System.Collections.Generic;

namespace RateLimiter.TokenLimiters
{
	public static class Combination
	{
		public static ITokenLimiter<T> And<T>(ITokenLimiter<T> a, ITokenLimiter<T> b) =>
			new TokenLimiterBuilder<T>(history => a.IsRateLimited(history) && b.IsRateLimited(history));

		public static ITokenLimiter<T> Or<T>(ITokenLimiter<T> a, ITokenLimiter<T> b) =>
			new TokenLimiterBuilder<T>(history => a.IsRateLimited(history) || b.IsRateLimited(history));

		public static ITokenLimiter<T> Not<T>(ITokenLimiter<T> tokenLimiter) =>
			new TokenLimiterBuilder<T>(history => !tokenLimiter.IsRateLimited(history));


		#region TokenLimiterBuilder

		private delegate bool LimitChecker<T>(IEnumerable<IApiRequest<T>> history);

		private class TokenLimiterBuilder<T> : ITokenLimiter<T>
		{
			private readonly LimitChecker<T> _limitChecker;

			public TokenLimiterBuilder(LimitChecker<T> limitChecker)
			{
				_limitChecker = limitChecker;
			}

			public bool IsRateLimited(IEnumerable<IApiRequest<T>> history) =>
				_limitChecker.Invoke(history);
		}

		#endregion TokenLimiterBuilder
	}
}
