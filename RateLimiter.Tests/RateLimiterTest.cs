using NUnit.Framework;
using RateLimiter.HistoryInspectors;
using RateLimiter.RequestFilters;
using RateLimiter.Tests.Utilities;
using RateLimiter.TokenMatchers;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
	public class RateLimiterTests
	{
		private static AccessToken GetToken(int userId, string countryCode) =>
			new(userId, "127.0.0.1", countryCode);
		private static ApiRequest GetRequest(int userId, TimeSpan timeBeforeNow, string resource) =>
			new(userId, DateTimeOffset.UtcNow.Subtract(timeBeforeNow), resource);

		// Note: These tests are technically not deterministic because the timestamp for now
		// is being calculated in different spots. The tests run fast enough and the intervals
		// are large enough that this isn't going to manifest as a problem. However, in real
		// production code, to make this inspector truly testable, we would need to
		// inject the value for now (or a delegate function for calculating now).
		[TestFixture]
		public class IsRateLimited
		{
			[Test]
			public async Task WithEmptyRulesItIsNotRateLimited()
			{
				var rules = Array.Empty<RateLimiterRule<int>>();
				var historyProvider = new InMemoryHistoryProvider<int>();

				historyProvider.Record(
					GetRequest(1, TimeSpan.FromMinutes(10), "Users"),
					GetRequest(1, TimeSpan.FromMinutes(7), "Orders"),
					GetRequest(2, TimeSpan.FromMinutes(5), "Users")
				);

				var rateLimiter = new RateLimiter<int>(historyProvider, rules);

				var accessToken = new AccessToken(1, "127.0.0.1", "US");

				var result = await rateLimiter.IsRateLimited(accessToken);

				Assert.False(result);
			}

			[Test]
			public async Task WhenNoRulesApplyToTokenItIsNotRateLimited()
			{
				var rules = new[]
				{
					new RateLimiterRule<int>(
						new PlaceOfOriginMatcher<int>("GB", "IE"),
						new AlwaysLimitHistoryInspector<int>()
					),
					new RateLimiterRule<int>(
						new PlaceOfOriginMatcher<int>("US", "MX", "CA"),
						new AlwaysLimitHistoryInspector<int>()
					)
				};

				var historyProvider = new InMemoryHistoryProvider<int>();
				historyProvider.Record(
					GetRequest(1, TimeSpan.FromSeconds(1), "Users"),
					GetRequest(1, TimeSpan.FromSeconds(2), "Users"),
					GetRequest(1, TimeSpan.FromSeconds(3), "Users"),
					GetRequest(1, TimeSpan.FromSeconds(4), "Users")
				);

				var accessToken = GetToken(1, "DK");
				var rateLimiter = new RateLimiter<int>(historyProvider, rules);

				var result = await rateLimiter.IsRateLimited(accessToken);

				Assert.False(result);
			}

			[Test]
			public async Task WhenHistoryIsFilteredBelowRateLimitItIsNotRateLimited()
			{
				var rules = new[]
				{
					new RateLimiterRule<int>(
						new ResourceFilter<int>("Users"),
						new LastRequestInspector<int>(TimeSpan.FromMinutes(5))
					)
				};

				var historyProvider = new InMemoryHistoryProvider<int>();
				historyProvider.Record(
					GetRequest(1, TimeSpan.FromMinutes(10), "Users"),
					GetRequest(1, TimeSpan.FromMinutes(4), "Orders"),
					GetRequest(1, TimeSpan.FromMinutes(2), "Products")
				);

				var accessToken = GetToken(1, "US");
				var rateLimiter = new RateLimiter<int>(historyProvider, rules);

				var result = await rateLimiter.IsRateLimited(accessToken);

				Assert.False(result);
			}

			[Test]
			public async Task WhenBelowRateLimiterLimitsItIsNotRateLimited()
			{
				var rules = new[]
				{
					new RateLimiterRule<int>(
						new RollingIntervalInspector<int>(TimeSpan.FromMinutes(5), 3)
					),
					new RateLimiterRule<int>(
						new LastRequestInspector<int>(TimeSpan.FromMinutes(1))
					)
				};

				var historyProvider = new InMemoryHistoryProvider<int>();
				historyProvider.Record(
					GetRequest(1, TimeSpan.FromMinutes(10), "Users"),
					GetRequest(1, TimeSpan.FromMinutes(4), "Orders"),
					GetRequest(1, TimeSpan.FromMinutes(2), "Products")
				);

				var accessToken = GetToken(1, "US");
				var rateLimiter = new RateLimiter<int>(historyProvider, rules);

				var result = await rateLimiter.IsRateLimited(accessToken);

				Assert.False(result);
			}

			[Test]
			public async Task WhenASingleRuleLimitsItIsRateLimited()
			{
				var rules = new[]
				{
					new RateLimiterRule<int>(
						new RollingIntervalInspector<int>(TimeSpan.FromMinutes(5), 3)
					),
					new RateLimiterRule<int>(
						new LastRequestInspector<int>(TimeSpan.FromMinutes(1))
					)
				};

				var historyProvider = new InMemoryHistoryProvider<int>();
				historyProvider.Record(
					GetRequest(1, TimeSpan.FromMinutes(10), "Users"),
					GetRequest(1, TimeSpan.FromMinutes(4), "Orders"),
					GetRequest(1, TimeSpan.FromSeconds(30), "Products")
				);

				var accessToken = GetToken(1, "US");
				var rateLimiter = new RateLimiter<int>(historyProvider, rules);

				var result = await rateLimiter.IsRateLimited(accessToken);

				Assert.True(result);
			}

			[Test]
			public async Task WhenMultipleRulesLimitItIsRateLimited()
			{
				var rules = new[]
				{
					new RateLimiterRule<int>(
						new RollingIntervalInspector<int>(TimeSpan.FromMinutes(5), 3)
					),
					new RateLimiterRule<int>(
						new LastRequestInspector<int>(TimeSpan.FromMinutes(1))
					)
				};

				var historyProvider = new InMemoryHistoryProvider<int>();
				historyProvider.Record(
					GetRequest(1, TimeSpan.FromMinutes(4), "Users"),
					GetRequest(1, TimeSpan.FromMinutes(2), "Orders"),
					GetRequest(1, TimeSpan.FromSeconds(30), "Products")
				);

				var accessToken = GetToken(1, "US");
				var rateLimiter = new RateLimiter<int>(historyProvider, rules);

				var result = await rateLimiter.IsRateLimited(accessToken);

				Assert.True(result);
			}
		}
	}
}
