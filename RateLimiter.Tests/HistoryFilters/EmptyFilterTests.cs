using NUnit.Framework;
using RateLimiter.RequestFilters;
using System;

namespace RateLimiter.Tests.HistoryFilters
{
	public class EmptyFilterTests
	{
		[TestFixture]
		public class IsRequestIncluded
		{
			[Test]
			public void AlwaysReturnsTrue()
			{
				var request = new ApiRequest
				{
					UserId = 1,
					Resource = "Products",
					Timestamp = DateTimeOffset.Now
				};

				var filter = new EmptyFilter<int>();

				var result = filter.IsRequestIncluded(request);

				Assert.True(result);
			}
		}
	}
}
