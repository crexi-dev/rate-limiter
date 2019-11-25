using System;
using RateLimiter.Interfaces;

namespace RateLimiter
{
	public class DateTimeProvider: IDateTimeProvider
	{
		public DateTime Now => DateTime.UtcNow;
	}
}
