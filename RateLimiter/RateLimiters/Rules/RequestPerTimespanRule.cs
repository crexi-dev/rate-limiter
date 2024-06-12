using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimiters.Rules;

public sealed class RequestPerTimespanRule : IRule
{
	private readonly TimeSpan _time;
	private readonly int _limit;
	private readonly object _lock = new();
	private readonly IList<DateTime> _requests = new List<DateTime>();

	public RequestPerTimespanRule(TimeSpan time, int limit)
	{
		_time = time;
		_limit = limit;
	}

	public bool Allows(Client client)
	{
		//basic implementation (almost correct)

		lock (_lock)
		{
			for (var ind = 0; ind < _requests.Count;)
			{
				if (_requests[ind] < DateTime.UtcNow - _time)
				{
					_requests.RemoveAt(ind);
				}
			}
		}

		if (_requests.Count >= _limit) return false;

		lock (_lock)
		{
			_requests.Add(DateTime.UtcNow);
		}

		return true;
	}
}
