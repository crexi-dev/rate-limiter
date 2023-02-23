using RateLimiter.Models;

namespace RateLimiter.Interfaces
{
	public interface ILimiterRule
	{
		bool Validate(RequestsData requestsData);
	}
}