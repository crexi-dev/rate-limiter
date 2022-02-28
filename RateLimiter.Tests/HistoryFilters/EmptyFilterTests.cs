using NUnit.Framework;
using RateLimiter.RequestFilters;
using RateLimiter.Tests.Utilities;
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
				var request = new ApiRequest(1, DateTimeOffset.Now, "Products");

				var filter = new EmptyFilter<int>();

				var result = filter.IsRequestIncluded(request);

				Assert.True(result);
			}
		}
	}
}
