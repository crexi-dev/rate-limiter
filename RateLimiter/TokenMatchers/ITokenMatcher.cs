namespace RateLimiter.TokenMatchers
{
	public interface ITokenMatcher<T>
	{
		bool MatchesToken(IAccessToken<T> accessToken);
	}
}
