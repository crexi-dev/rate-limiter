using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.HistoryFilters
{
	public class ResourceHistory<T> : IHistoryFilter<T>
	{
		private readonly HashSet<string> _resources;

		public ResourceHistory(params string[] resources)
		{
			_resources = resources.ToHashSet();
		}

		public IEnumerable<IApiRequest<T>> FilterHistory(IEnumerable<IApiRequest<T>> history)
		{
			return history.Where(request => _resources.Contains(request.Resource));
		}
	}
}
