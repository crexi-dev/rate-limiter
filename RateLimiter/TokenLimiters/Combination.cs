namespace RateLimiter.TokenLimiters
{
	public static class Combination
	{
		public static ITokenLimiter And(ITokenLimiter a, ITokenLimiter b) =>
			new TokenLimiterBuilder(accessToken => a.IsRateLimited(accessToken) && b.IsRateLimited(accessToken));

		public static ITokenLimiter Or(ITokenLimiter a, ITokenLimiter b) =>
			new TokenLimiterBuilder(accessToken => a.IsRateLimited(accessToken) || b.IsRateLimited(accessToken));

		public static ITokenLimiter Not(ITokenLimiter tokenLimiter) =>
			new TokenLimiterBuilder(accessToken => !tokenLimiter.IsRateLimited(accessToken));


		#region TokenLimiterBuilder

		private delegate bool LimitChecker(IAccessToken accessToken);

		private class TokenLimiterBuilder : ITokenLimiter
		{
			private readonly LimitChecker _limitChecker;

			public TokenLimiterBuilder(LimitChecker limitChecker)
			{
				_limitChecker = limitChecker;
			}

			public bool IsRateLimited(IAccessToken accessToken) => _limitChecker.Invoke(accessToken);
		}

		#endregion TokenLimiterBuilder
	}
}
