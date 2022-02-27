using NUnit.Framework;
using RateLimiter.HistoryInspectors;
using System;

namespace RateLimiter.Tests.HistoryInspectors
{
	public class LastRequestInspectorTests
	{
		// Note: These tests are technically not deterministic because the timestamp for now
		// is being calculated in different spots. The tests run fast enough and the intervals
		// are large enough that this isn't going to manifest as a problem. However, in real
		// production code, to make this inspector truly testable, we would need to
		// inject the value for now (or a delegate function for calculating now).
		[TestFixture]
		public class IsRateLimited
		{
			private static ApiRequest GetRequest(TimeSpan timeBeforeNow) => new()
			{
				UserId = 1,
				Resource = "Orders",
				Timestamp = DateTimeOffset.UtcNow.Subtract(timeBeforeNow)
			};

			[Test]
			public void EmptyHistoryIsNotRateLimited()
			{
				var history = Array.Empty<ApiRequest>();

				var interval = TimeSpan.FromMinutes(5);
				var inspector = new LastRequestInspector<int>(interval);

				var result = inspector.IsRateLimited(history);

				Assert.False(result);
			}

			[Test]
			public void NoRequestsInIntervalIsNotRateLimited()
			{
				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(10)),
					GetRequest(TimeSpan.FromMinutes(7))
				};

				var interval = TimeSpan.FromMinutes(5);
				var inspector = new LastRequestInspector<int>(interval);

				var result = inspector.IsRateLimited(history);

				Assert.False(result);
			}

			[Test]
			public void OneRequestInIntervalIsRateLimited()
			{
				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(10)),
					GetRequest(TimeSpan.FromMinutes(7)),
					GetRequest(TimeSpan.FromMinutes(3)),
				};

				var interval = TimeSpan.FromMinutes(5);
				var inspector = new LastRequestInspector<int>(interval);

				var result = inspector.IsRateLimited(history);

				Assert.True(result);
			}

			[Test]
			public void MultipleRequestsInIntervalIsRateLimited()
			{
				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(10)),
					GetRequest(TimeSpan.FromMinutes(7)),
					GetRequest(TimeSpan.FromMinutes(3)),
					GetRequest(TimeSpan.FromMinutes(2)),
				};

				var interval = TimeSpan.FromMinutes(5);
				var inspector = new LastRequestInspector<int>(interval);

				var result = inspector.IsRateLimited(history);

				Assert.True(result);
			}
		}
	}
}
