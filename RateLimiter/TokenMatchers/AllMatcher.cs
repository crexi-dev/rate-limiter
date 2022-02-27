namespace RateLimiter.TokenMatchers
{
	/// <summary>
	/// A default token matcher that matches with every access token.
	/// </summary>
	public class AllMatcher<T> : ITokenMatcher<T>
	{
		public bool MatchesToken(IAccessToken<T> accessToken) => true;
	}
}
