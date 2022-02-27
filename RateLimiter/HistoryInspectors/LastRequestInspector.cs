using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.HistoryInspectors
{
	/// <summary>
	/// Uses the most recent request to determine whether a user should be rate limited.
	/// 
	/// For example, if the rule is that a user can only send one request per second,
	/// then the inspector examines the history to see if any requests were made within
	/// the last second. If so, then the user is rate limited. Otherwise, they are not.
	/// </summary>
	public class LastRequestInspector<T> : IHistoryInspector<T>
	{
		private readonly TimeSpan _minimumInterval;

		/// <summary>
		/// Create a new inspector.
		/// </summary>
		/// <param name="minimumInterval">
		/// The minimum amount of time a user must wait before sending a
		/// subsequent API request.
		/// </param>
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
