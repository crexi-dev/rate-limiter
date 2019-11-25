namespace RateLimiter.Interfaces
{
	public interface IRateLimitManager
	{
		bool CheckAccess(long resourceId, long userId);
	}
}
