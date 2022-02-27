using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.HistoryInspectors
{
	/// <summary>
	/// Sets a maximum number of requests that can be made over a
	/// given timespan.
	/// 
	/// For example, if a user can only make 100 requests per hour,
	/// then the inspector examines the history and filters out
	/// any requests made before one hour ago. If the count of the
	/// remaining requests is 100 or more, then the user is rate limited.
	/// Otherwise, they are not.
	/// </summary>
	public class TimeSpanInspector<T> : IHistoryInspector<T>
	{
		private readonly TimeSpan _timeSpan;
		private readonly int _maxRequests;

		/// <summary>
		/// Create a new inspector.
		/// </summary>
		/// <param name="timeSpan">The interval of time to check for request history.</param>
		/// <param name="maxRequests">The maximum number of requests allowed for that time interval.</param>
		public TimeSpanInspector(TimeSpan timeSpan, int maxRequests)
		{
			_timeSpan = timeSpan;
			_maxRequests = maxRequests;
		}

		public bool IsRateLimited(IEnumerable<IApiRequest<T>> history)
		{
			var intervalStart = DateTimeOffset.UtcNow.Subtract(_timeSpan);
			var count = history.Count(request => request.Timestamp >= intervalStart);

			return count >= _maxRequests;
		}
	}
}
