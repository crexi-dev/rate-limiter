using System.Collections.Generic;

namespace RateLimiter.HistoryInspectors
{
	public interface IHistoryInspector<T>
	{
		bool IsRateLimited(IEnumerable<IApiRequest<T>> history);
	}
}
