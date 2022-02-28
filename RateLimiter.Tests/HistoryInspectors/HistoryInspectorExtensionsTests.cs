using NUnit.Framework;
using RateLimiter.HistoryInspectors;
using RateLimiter.Tests.Utilities;
using System;

namespace RateLimiter.Tests.HistoryInspectors
{
	public class HistoryInspectorExtensionsTests
	{
		private static ApiRequest GetRequest(TimeSpan timeBeforeNow) =>
			new(1, DateTimeOffset.UtcNow.Subtract(timeBeforeNow), "Orders");

		[TestFixture]
		public class And
		{
			[Test]
			public void TransformedInspectorReturnsFalseIfFirstInspectorReturnsFalse()
			{
				var a = new LastRequestInspector<int>(TimeSpan.FromMinutes(2));
				var b = new AlwaysLimitHistoryInspector<int>();

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(5)),
				};

				var inspector = a.And(b);

				var result = inspector.IsRateLimited(history);

				Assert.False(result);
			}

			[Test]
			public void TransformedInspectorReturnsFalseIfSecondInspectorReturnsFalse()
			{
				var a = new AlwaysLimitHistoryInspector<int>();
				var b = new LastRequestInspector<int>(TimeSpan.FromMinutes(2));

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(5)),
				};

				var inspector = a.And(b);

				var result = inspector.IsRateLimited(history);

				Assert.False(result);
			}

			[Test]
			public void TransformedInspectorReturnsTrueIfBothInspectorsReturnTrue()
			{
				var a = new LastRequestInspector<int>(TimeSpan.FromMinutes(6));
				var b = new RollingIntervalInspector<int>(TimeSpan.FromMinutes(10), 2);

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(5)),
				};

				var inspector = a.And(b);

				var result = inspector.IsRateLimited(history);

				Assert.True(result);
			}

			[Test]
			public void TransformedInspectorShortCircuts()
			{
				var a = new LastRequestInspector<int>(TimeSpan.FromMinutes(2));
				var b = new ThrowExceptionInspector<int>();

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(5)),
				};

				var inspector = a.And(b);

				var result = inspector.IsRateLimited(history);

				Assert.False(result);
			}
		}

		[TestFixture]
		public class Or
		{
			[Test]
			public void TransformedInspectorReturnsTrueIfFirstInspectorReturnsTrue()
			{
				var a = new LastRequestInspector<int>(TimeSpan.FromMinutes(6));
				var b = new NeverLimitHistoryInspector<int>();

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(5)),
				};

				var inspector = a.Or(b);

				var result = inspector.IsRateLimited(history);

				Assert.True(result);
			}

			[Test]
			public void TransformedInspectorReturnsTrueIfSecondInspectorReturnsTrue()
			{
				var a = new NeverLimitHistoryInspector<int>();
				var b = new LastRequestInspector<int>(TimeSpan.FromMinutes(2));

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(1)),
				};

				var inspector = a.Or(b);

				var result = inspector.IsRateLimited(history);

				Assert.True(result);
			}

			[Test]
			public void TransformedInspectorReturnsFalseIfBothInspectorsReturnFalse()
			{
				var a = new LastRequestInspector<int>(TimeSpan.FromMinutes(4));
				var b = new RollingIntervalInspector<int>(TimeSpan.FromMinutes(10), 3);

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(5)),
				};

				var inspector = a.Or(b);

				var result = inspector.IsRateLimited(history);

				Assert.False(result);
			}

			[Test]
			public void TransformedInspectorShortCircuts()
			{
				var a = new LastRequestInspector<int>(TimeSpan.FromMinutes(4));
				var b = new ThrowExceptionInspector<int>();

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(2)),
				};

				var inspector = a.Or(b);

				var result = inspector.IsRateLimited(history);

				Assert.True(result);
			}
		}

		[TestFixture]
		public class Inverse
		{
			[Test]
			public void ReturnsTrueIfGivenInspectReturnsFalse()
			{
				var original = new LastRequestInspector<int>(TimeSpan.FromMinutes(2));

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(5)),
				};

				var inspector = original.Inverse();

				var result = inspector.IsRateLimited(history);

				Assert.True(result);
			}

			[Test]
			public void ReturnsFalseIfGivenInspectorReturnsTrue()
			{
				var original = new LastRequestInspector<int>(TimeSpan.FromMinutes(6));

				var history = new[]
				{
					GetRequest(TimeSpan.FromMinutes(8)),
					GetRequest(TimeSpan.FromMinutes(5)),
				};

				var inspector = original.Inverse();

				var result = inspector.IsRateLimited(history);

				Assert.False(result);
			}
		}
	}
}
