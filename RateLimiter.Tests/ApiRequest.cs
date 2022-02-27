using System;

namespace RateLimiter.Tests
{
	internal class ApiRequest : IApiRequest<int>
	{
		public int UserId { get; init; }

		public DateTimeOffset Timestamp { get; init; }

		public string Resource { get; init; } = string.Empty;
	}
}
