using System;

namespace RateLimiter.Tests.Utilities
{
	internal record ApiRequest(int UserId, DateTimeOffset Timestamp, string Resource) : IApiRequest<int>;
}