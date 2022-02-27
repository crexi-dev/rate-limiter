using System.Collections.Generic;

namespace RateLimiter.HistoryFilters
{
	public class EmptyFilter<T> : IHistoryFilter<T>
	{
		public IEnumerable<IApiRequest<T>> FilterHistory(IEnumerable<IApiRequest<T>> history) => history;
	}
}
