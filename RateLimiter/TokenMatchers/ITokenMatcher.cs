namespace RateLimiter.TokenMatchers
{
	/// <summary>
	/// Matches a rule against an access token to see if that rule applies.
	/// </summary>
	public interface ITokenMatcher<T>
	{
		/// <summary>
		/// Determines if the given access token triggers the associated rule.
		/// </summary>
		/// <returns>True if the rule should be run for this access token, false otherwise.</returns>
		bool MatchesToken(IAccessToken<T> accessToken);
	}
}
