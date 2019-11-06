using System;
using FluentAssertions;
using Xunit;

namespace RateLimiter.Tests
{
	public class TimeoutLimitingStrategyTest
	{
		[Fact]
		public void TryPass_ShouldPassOnFirstCall()
		{
			var timeOffSet = TimeSpan.MaxValue;
			var target = new TimeoutLimitingStrategy(timeOffSet, () => DateTimeOffset.UtcNow);

			var result = target.TryPass("42");

			result.Should().Be(true);
		}

		[Fact]
		public void TryPass_ShouldPassOnSecondDelayedCall()
		{
			var token = "42";

			var dt1 = DateTimeOffset.UtcNow;
			var timeOffSet = TimeSpan.FromSeconds(1);
			var target = new TimeoutLimitingStrategy(timeOffSet, GetCurrentTime);
			target.TryPass(token).Should().BeTrue();

			dt1 = dt1.AddSeconds(2);

			target.TryPass(token).Should().BeTrue();

			DateTimeOffset GetCurrentTime() => dt1;
		}

		[Fact]
		public void TryPass_ShouldNotPassOnSecondFastCall()
		{
			var token = "42";

			var dt1 = DateTimeOffset.UtcNow;
			var timeOffSet = TimeSpan.FromSeconds(2);
			var target = new TimeoutLimitingStrategy(timeOffSet, GetCurrentTime);
			target.TryPass(token).Should().BeTrue();

			dt1 = dt1.AddSeconds(1);

			target.TryPass(token).Should().BeFalse();

			DateTimeOffset GetCurrentTime() => dt1;
		}
	}
}