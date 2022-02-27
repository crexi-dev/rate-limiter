using System;

namespace RateLimiter
{
	public interface IApiRequest<T>
	{
		public T Id { get; }
		public DateTimeOffset Timestamp { get; }
	}
}
