using RateLimit.DTO;

namespace RateLimit.Contracts
{
	public interface IRule
	{
		Task<bool> IsAccessAllowedAsync(RequestDto request);

	}

}
