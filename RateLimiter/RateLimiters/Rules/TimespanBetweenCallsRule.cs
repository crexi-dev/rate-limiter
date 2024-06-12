using System;

namespace RateLimiter.RateLimiters.Rules;

public sealed class TimespanBetweenCallsRule : IRule
{
	private readonly TimeSpan _time;
	private DateTime _lastCall;

	public TimespanBetweenCallsRule(TimeSpan time)
	{
		_time = time;
	}

	public bool Allows(Client client)
	{
		//basic implementation (almost correct)

		if (DateTime.UtcNow - _time > _lastCall) return false;

		_lastCall = DateTime.UtcNow;
		return true;
	}
}
