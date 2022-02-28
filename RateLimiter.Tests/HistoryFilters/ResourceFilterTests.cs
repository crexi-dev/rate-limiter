using NUnit.Framework;
using RateLimiter.RequestFilters;
using RateLimiter.Tests.Utilities;
using System;

namespace RateLimiter.Tests.HistoryFilters
{
	public class ResourceFilterTests
	{
		[TestFixture]
		public class IsRequestIncluded
		{
			[TestCase("")]
			[TestCase("Products")]
			public void EmptyResourcesNeverMatches(string resource)
			{
				var request = new ApiRequest(1, DateTimeOffset.UtcNow, resource);

				var filter = new ResourceFilter<int>();

				var result = filter.IsRequestIncluded(request);

				Assert.False(result);
			}

			[Test]
			public void EmptyInputNeverMatches()
			{
				var request = new ApiRequest(1, DateTimeOffset.UtcNow, string.Empty);

				var filter = new ResourceFilter<int>("Products", "Orders", "Users");

				var result = filter.IsRequestIncluded(request);

				Assert.False(result);
			}

			[Test]
			public void MissingResourceDoesNotMatch()
			{
				var request = new ApiRequest(1, DateTimeOffset.UtcNow, "Orders");

				var filter = new ResourceFilter<int>("Products", "Users");

				var result = filter.IsRequestIncluded(request);

				Assert.False(result);
			}

			[Test]
			public void SameCaseMatches()
			{
				var resource = "Orders";
				var request = new ApiRequest(1, DateTimeOffset.UtcNow, resource);

				var filter = new ResourceFilter<int>(resource);

				var result = filter.IsRequestIncluded(request);

				Assert.True(result);
			}

			[Test]
			public void DifferentCaseMatches()
			{
				var request = new ApiRequest(1, DateTimeOffset.UtcNow, "orders");

				var filter = new ResourceFilter<int>("Orders");

				var result = filter.IsRequestIncluded(request);

				Assert.True(result);
			}

			[Test]
			public void ResourceAmongListMatches()
			{
				var request = new ApiRequest(1, DateTimeOffset.UtcNow, "Users");

				var filter = new ResourceFilter<int>("Products", "Orders", "Users");

				var result = filter.IsRequestIncluded(request);

				Assert.True(result);
			}
		}
	}
}
