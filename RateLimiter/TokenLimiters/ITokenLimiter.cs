namespace RateLimiter.TokenLimiters
{
	public interface ITokenLimiter
	{
		bool IsRateLimited(IAccessToken accessToken);
	}
}
