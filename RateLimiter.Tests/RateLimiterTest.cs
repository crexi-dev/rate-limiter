using System;
using Moq;
using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
	    private Mock<IStorage> storage;
	    private IRateLimiter target;
	    private ResourceRequestLimit rule;

	    private const string ResourceId = "qwe";
	    private const string ClientId = "asd";

		[OneTimeSetUp]
	    public void SetUp()
	    {
			storage = new Mock<IStorage>();
			rule = new ResourceRequestLimit(TimeSpan.FromSeconds(5));
			target = new RateLimiter(storage.Object, rule);
	    }

	    [TestCase(1)]
	    [TestCase(2)]
	    [TestCase(3)]
	    [TestCase(4)]
        public void Denies_When_Limit_Exceeded(int secondsSinceLastCall)
        {
	        storage
		        .Setup(x => x.GetLastUsage(ResourceId, ClientId))
		        .Returns(DateTime.UtcNow - TimeSpan.FromSeconds(secondsSinceLastCall));

	        var actual = target.IsRequestAllowed(ResourceId, ClientId);

            Assert.IsFalse(actual);
		}

		[TestCase(14)]
		[TestCase(24)]
		[TestCase(6)]
		public void Allow_When_Limit_Does_Not_Reached(int secondsSinceLastCall)
		{
			storage
				.Setup(x => x.GetLastUsage(ResourceId, ClientId))
				.Returns(DateTime.UtcNow - TimeSpan.FromSeconds(secondsSinceLastCall));

			var actual = target.IsRequestAllowed(ResourceId, ClientId);

			Assert.IsTrue(actual);
		}
	}
}
