using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.HistoryInspectors
{
	public class LastRequestInspector<T> : IHistoryInspector<T>
	{
		private readonly TimeSpan _minimumInterval;

		public LastRequestInspector(TimeSpan minimumInterval)
		{
			_minimumInterval = minimumInterval;
		}

		public bool IsRateLimited(IEnumerable<IApiRequest<T>> history)
		{
			var intervalStart = DateTimeOffset.UtcNow.Subtract(_minimumInterval);

			return history.Any(request => request.Timestamp > intervalStart);
		}
	}
}
