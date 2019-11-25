using System;

namespace RateLimiter.Interfaces
{
	public interface IDateTimeProvider
	{
		DateTime Now { get; }
	}
}
