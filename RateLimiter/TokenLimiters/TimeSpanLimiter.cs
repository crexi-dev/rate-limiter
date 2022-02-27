using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.TokenLimiters
{
	public class TimeSpanLimiter<T> : ITokenLimiter<T>
	{
		private readonly TimeSpan _timeSpan;
		private readonly int _maxRequests;

		public TimeSpanLimiter(TimeSpan timeSpan, int maxRequests)
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
