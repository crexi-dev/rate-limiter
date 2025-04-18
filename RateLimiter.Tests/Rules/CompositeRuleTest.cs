using System;
using NUnit.Framework;
using RateLimiter.Core.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class CompositeRuleTest
    {
        [Test]
        public void TestCompositeAnd()
        {
            var rule1 = new FixedWindowRule(2, TimeSpan.FromSeconds(10)); // 2 requests max
            var rule2 = new FixedWindowRule(3, TimeSpan.FromSeconds(10)); // 3 requests max
            var compositeRule = new CompositeAndRule(rule1, rule2);

            string clientToken = "test-client";
            string resourceId = "api/test";

            Assert.That(compositeRule.IsAllowed(clientToken, resourceId), Is.True, "First request should be allowed by both rules");
            Assert.That(compositeRule.IsAllowed(clientToken, resourceId), Is.True, "Second request should be allowed by both rules");
            Assert.That(compositeRule.IsAllowed(clientToken, resourceId), Is.False, "Third request should be denied by rule1");
        }

        [Test]
        public void TestCompositeOr()
        {
            var rule1 = new FixedWindowRule(1, TimeSpan.FromSeconds(10)); // 1 request max
            var rule2 = new TokenBucketRule(2, 0); // allow 2 requests
            var compositeRule = new CompositeOrRule(rule1, rule2);

            string clientToken = "test-client";
            string resourceId = "api/test";

            Assert.That(compositeRule.IsAllowed(clientToken, resourceId), Is.True, "First request should be allowed by both rules");
            Assert.That(compositeRule.IsAllowed(clientToken, resourceId), Is.True, "Second request should be allowed by rule2");
            Assert.That(compositeRule.IsAllowed(clientToken, resourceId), Is.True, "Third request should be allowed by 1 rule");
            Assert.That(compositeRule.IsAllowed(clientToken, resourceId), Is.False, "Fourth request should be denied by both rules");
        }
    }
}