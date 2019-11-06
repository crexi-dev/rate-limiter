using System;
using System.Collections.Generic;

namespace RateLimiter
{
	public interface IRateLimiterStrategy
	{
		bool TryPass(string token);
	}

	public class LimitedPerTimeSpanStrategy : IRateLimiterStrategy
	{
		public bool TryPass(string token)
		{
			throw new NotImplementedException();
		}
	}

	public class TimeoutLimitingStrategy : IRateLimiterStrategy
	{
		private readonly TimeSpan _timeOffset;
		private readonly Func<DateTimeOffset> _getCurrentTime;
		private Dictionary<string, DateTimeOffset> _history = new Dictionary<string, DateTimeOffset>();
		private Object _lock = new Object();

		public TimeoutLimitingStrategy(TimeSpan timeOffset, Func<DateTimeOffset> getCurrentTime)
		{
			_timeOffset = timeOffset;
			_getCurrentTime = getCurrentTime ?? throw new ArgumentNullException(nameof(getCurrentTime));
		}

		public bool TryPass(string token)
		{
			bool success = true;
			var currentCallTime = _getCurrentTime();


			lock (_lock)
			{
				if (_history.TryGetValue(token, out var lastCallTime))
				{
					if (lastCallTime.Add(_timeOffset) > currentCallTime)
						success = false;
				}

				_history[token] = currentCallTime;
			}

			return success;
		}
	}


	public class RateLimiter
	{
		private readonly IRateLimiterStrategy _strategy;

		public RateLimiter(IRateLimiterStrategy strategy)
		{
			_strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
		}

		public bool TryPass(string token) => _strategy.TryPass(token);
	}
}