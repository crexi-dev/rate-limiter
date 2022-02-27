namespace RateLimiter.TokenMatchers
{
	public class AllMatcher<T> : ITokenMatcher<T>
	{
		public bool MatchesToken(IAccessToken<T> accessToken) => true;
	}
}
