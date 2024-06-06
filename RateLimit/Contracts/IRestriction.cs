using RateLimit.DTO;

namespace RateLimit.Contracts
{
	public interface IRestriction
	{
		Task<bool> IsPassedAsync(RequestDto request);
	}
}
