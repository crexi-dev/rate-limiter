namespace RateLimiter.TokenSelectors
{
	public interface ITokenSelector<T>
	{
		bool IsSelected(IAccessToken<T> accessToken);
	}
}
