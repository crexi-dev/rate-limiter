using NUnit.Framework;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void Example()
        {
            
            Assert.IsTrue(true);
        }

        [Test]
        public void RequestPerTimespanTestSingle()
        {
            List<IRule> rules = new List<IRule>();

            rules.Add(new RequestPerTimespanRule());

            var rateLimiter = new RateLimiter(rules);

            Assert.IsTrue(rateLimiter.Request("token"));

        }

        [Test]
        public void RequestPerTimespanTestMultiple()
        {
            List<IRule> rules = new List<IRule>();
            string token = "testToken";

            rules.Add(new RequestPerTimespanRule());

            var rateLimiter = new RateLimiter(rules);

            for(int i = 0; i < 10; i++)
            {
                rateLimiter.Request(token);
            }

            Assert.IsFalse(rateLimiter.Request(token));

        }
    }
}
