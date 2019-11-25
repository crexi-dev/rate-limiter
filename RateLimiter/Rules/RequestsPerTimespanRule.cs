using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Interfaces;

namespace RateLimiter.Rules
{
	public class RequestsPerTimespanRule : IRule
	{
		private const int TimeSpan = 60; // in seconds
		private const int RequestsCount = 10;

		public bool CheckAccess(IEnumerable<ResourceAccess> data, DateTime date)
		{
			if (data == null || !data.Any())
			{
				return true;
			}

			var count = data.Count(item => item.AccessOn >= date.AddSeconds(-TimeSpan));
			return count < RequestsCount;
		}
	}
}
