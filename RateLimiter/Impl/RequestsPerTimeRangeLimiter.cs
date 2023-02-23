using System;
using System.Linq;
using RateLimiter.Interfaces;
using RateLimiter.Models;

namespace RateLimiter.Impl
{
	public class RequestsPerTimeLimiter: ILimiterRule
	{
		public int RequestLimit { get; set; }
		public TimeSpan TimeRange { get; set; }

		public bool Validate(RequestsData requestsData)
		{
			if (requestsData.Requests.Count <= 1) return true;

			var timeLimit = DateTime.UtcNow - TimeRange;
			var requestCount = requestsData.Requests.Count(o => o > timeLimit);
			return requestCount <= RequestLimit;
		}
	}
}