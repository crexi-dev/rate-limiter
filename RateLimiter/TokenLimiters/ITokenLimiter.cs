using System.Collections.Generic;

namespace RateLimiter.TokenLimiters
{
	public interface ITokenLimiter<T>
	{
		bool IsRateLimited(IEnumerable<IApiRequest<T>> history);
	}
}
