namespace RateLimit.Contracts
{
	public interface ILimitService
	{
		Task<bool> IsAccessAllowedAsync(int limit, TimeSpan lastCallPeriod, string clientId);
	}
}