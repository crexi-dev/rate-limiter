using System.Collections.Generic;

namespace RateLimiter.HistoryFilters
{
	public interface IHistoryFilter<T>
	{
		IEnumerable<IApiRequest<T>> FilterHistory(IEnumerable<IApiRequest<T>> history);
	}
}
