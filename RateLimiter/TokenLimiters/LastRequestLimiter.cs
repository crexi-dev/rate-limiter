using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.TokenLimiters
{
	public class LastRequestLimiter<T> : ITokenLimiter<T>
	{
		private readonly TimeSpan _minimumInterval;

		public LastRequestLimiter(TimeSpan minimumInterval)
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
