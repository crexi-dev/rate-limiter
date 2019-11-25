using System;
using System.Collections.Generic;
using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests
{
	[TestFixture]
	public class RequestsPerTimespanRuleTests
	{
		[Test]
		public void NullResourceAccessesTest()
		{
			// Arrange
			var rule = new RequestsPerTimespanRule();
			var date = new DateTime(2010, 1, 1, 10, 10, 0);
			IEnumerable<ResourceAccess> data = null;

			// Act
			var result = rule.CheckAccess(data, date);

			//Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void EmptyResourceAccessesTest()
		{
			// Arrange
			var rule = new RequestsPerTimespanRule();
			var date = new DateTime(2010, 1, 1, 10, 10, 0);
			var data = new List<ResourceAccess>();

			// Act
			var result = rule.CheckAccess(data, date);

			//Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void BeforeRequestsCountLimitTest()
		{
			// Arrange
			var rule = new RequestsPerTimespanRule();
			var date = new DateTime(2010, 1, 1, 10, 10, 0);
			var data = new List<ResourceAccess>
			{
				new ResourceAccess
				{
					AccessOn = new DateTime(2010, 1, 1, 10, 9, 30)
				}
			};

			// Act
			var result = rule.CheckAccess(data, date);

			//Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void EqualsRequestsCountLimitTest()
		{
			// Arrange
			var rule = new RequestsPerTimespanRule();
			var date = new DateTime(2010, 1, 1, 10, 10, 0);
			var data = new List<ResourceAccess>
			{
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
			};

			// Act
			var result = rule.CheckAccess(data, date);

			//Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void AfterRequestsCountLimitTest()
		{
			// Arrange
			var rule = new RequestsPerTimespanRule();
			var date = new DateTime(2010, 1, 1, 10, 10, 0);
			var data = new List<ResourceAccess>
			{
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
				new ResourceAccess { AccessOn = new DateTime(2010, 1, 1, 10, 9, 30) },
			};

			// Act
			var result = rule.CheckAccess(data, date);

			//Assert
			Assert.IsFalse(result);
		}
	}
}
