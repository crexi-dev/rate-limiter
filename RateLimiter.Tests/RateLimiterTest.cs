using System.Collections.Generic;
using NUnit.Framework;

namespace RateLimiter.Tests
{
	[TestFixture]
	public class RateLimiterTest
	{
		private ResourceRules _defaultRules;
		private Dictionary<string, ResourceRules> _resourceRules;
		private const string ResourceId1 = "Resource1";
		private const string ResourceId2 = "Resource2";

		[SetUp]
		public void TestInit()
		{
			_defaultRules = new ResourceRules
			{
				MinCallTimespan = 1000,
				RequestPerTimespan = 10,
				Timespan = 1000 * 30 // 30 seconds
			};

			_resourceRules = new Dictionary<string, ResourceRules>
			{
				{ResourceId1, new ResourceRules {MinCallTimespan = 500, RequestPerTimespan = 5, Timespan = 1000 * 60}},
				{ResourceId2, new ResourceRules {MinCallTimespan = 250, RequestPerTimespan = 4, Timespan = 1000 * 1}}
			};
		}

		[Test]
		public void Should_Allow_Request()
		{
			var rateLimiter = new RateLimiter(_defaultRules, _resourceRules);

			var userToken = "TestToken1";
			var result = rateLimiter.AskForResource(userToken, ResourceId1);

			Assert.IsTrue(result);
		}

		[Test]
		public void Should_Not_Allow_Fast_Requests()
		{
			var rateLimiter = new RateLimiter(_defaultRules, _resourceRules);

			var userToken = "TestToken1";

			var result1 = rateLimiter.AskForResource(userToken, ResourceId1);
			Assert.IsTrue(result1);

			var result2 = rateLimiter.AskForResource(userToken, ResourceId1);
			Assert.IsFalse(result2);
		}

		[Test]
		public void Should_Allow_Multiple_Tokens_Request()
		{
			var rateLimiter = new RateLimiter(_defaultRules, _resourceRules);

			var result1 = rateLimiter.AskForResource("TestToken1", ResourceId1);
			Assert.IsTrue(result1);

			var result2 = rateLimiter.AskForResource("TestToken2", ResourceId1);
			Assert.IsTrue(result2);
		}

		[Test]
		public void Should_Not_Allow_Max_Requests()
		{
			var rateLimiter = new RateLimiter(_defaultRules, _resourceRules);

			var userToken = "TestToken1";

			for (int i = 0; i < 5; i++)
			{
				var result = rateLimiter.AskForResource(userToken, ResourceId1);
				Assert.IsTrue(result);
				System.Threading.Thread.Sleep(500);
			}

			var result2 = rateLimiter.AskForResource(userToken, ResourceId1);
			Assert.IsFalse(result2);
		}

		[Test]
		public void Should_Allow_Multiple_Slow_Requests()
		{
			var rateLimiter = new RateLimiter(_defaultRules, _resourceRules);

			var userToken = "TestToken1";

			for (int i = 0; i < 6; i++)
			{
				var result = rateLimiter.AskForResource(userToken, ResourceId2);
				Assert.IsTrue(result);
				System.Threading.Thread.Sleep(300);
			}
		}

		[Test]
		[TestCase(5, 0)]
		[TestCase(0, 500)]
		[TestCase(0, 0)]
		public void Should_Allow_Without_Max_Count_rule(int requestPerTimespan, int timespan)
		{
			var resourceRules = new Dictionary<string, ResourceRules>
			{
				{ResourceId1, new ResourceRules {MinCallTimespan = 100, RequestPerTimespan = requestPerTimespan, Timespan = timespan}}
			};

			var rateLimiter = new RateLimiter(_defaultRules, resourceRules);

			var userToken = "TestToken1";

			for (int i = 0; i < 5; i++)
			{
				var result = rateLimiter.AskForResource(userToken, ResourceId1);
				Assert.IsTrue(result);
				System.Threading.Thread.Sleep(100);
			}
			rateLimiter.AskForResource(userToken, ResourceId1);

			var result2 = rateLimiter.AskForResource(userToken, ResourceId1);
			Assert.IsFalse(result2);
		}

		[Test]
		public void Should_Allow_Without_Min_Timespan_rule()
		{
			var resourceRules = new Dictionary<string, ResourceRules>
			{
				{ResourceId1, new ResourceRules {MinCallTimespan = 0, RequestPerTimespan = 5, Timespan = 1000 * 60}}
			};

			var rateLimiter = new RateLimiter(_defaultRules, resourceRules);

			var userToken = "TestToken1";

			for (int i = 0; i < 5; i++)
			{
				var result = rateLimiter.AskForResource(userToken, ResourceId1);
				Assert.IsTrue(result);
			}

			var result2 = rateLimiter.AskForResource(userToken, ResourceId1);
			Assert.IsFalse(result2);
		}

		[Test]
		public void Should_Allow_Without_rules()
		{
			var resourceRules = new Dictionary<string, ResourceRules>
			{
				{ResourceId1, new ResourceRules {MinCallTimespan = 0, RequestPerTimespan = 0, Timespan = 0}}
			};

			var rateLimiter = new RateLimiter(_defaultRules, resourceRules);

			var userToken = "TestToken1";

			var result = rateLimiter.AskForResource(userToken, ResourceId1);
			Assert.IsTrue(result);
		}

		[Test]
		public void Should_Allow_Default_rules()
		{
			var rateLimiter = new RateLimiter(_defaultRules, _resourceRules);

			var userToken = "TestToken1";

			var result = rateLimiter.AskForResource(userToken, "NewResource");
			Assert.IsTrue(result);
		}
	}
}
