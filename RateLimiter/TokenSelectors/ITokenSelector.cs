namespace RateLimiter.TokenSelectors
{
	public interface ITokenSelector
	{
		bool IsSelected(IAccessToken accessToken);
	}
}
