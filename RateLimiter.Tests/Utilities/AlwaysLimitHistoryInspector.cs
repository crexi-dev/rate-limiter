using RateLimiter.HistoryInspectors;
using System.Collections.Generic;

namespace RateLimiter.Tests.Utilities
{
	internal class AlwaysLimitHistoryInspector<T> : IHistoryInspector<T>
	{
		public bool IsRateLimited(IEnumerable<IApiRequest<T>> history) => true;
	}
}
