using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter
{
	public interface IApiRequestHistoryProvider<T>
	{
		Task<IEnumerable<IApiRequest<T>>> GetApiRequestHistory(IAccessToken<T> accessToken);
	}
}
