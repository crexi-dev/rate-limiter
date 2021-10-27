using Moq;
using NUnit.Framework;
using RateLimiter.Domain.ApiLimiter;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void SlidingWindowRuleTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new SlidingWindowRule(mockTimer.Object, 1, 20);
            var request = rule.NewVisitAndRuleCheck();
            Assert.IsTrue(request);
            Assert.AreEqual(1, rule.RequestCount);
        }

        [Test]
        public void SlidingWindowRuleLimitExceededTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new SlidingWindowRule(mockTimer.Object, 1, 200);
            var request1 = rule.NewVisitAndRuleCheck();
            var request2 = rule.NewVisitAndRuleCheck();
            Assert.IsTrue(request1);
            Assert.IsFalse(request2);
            Assert.AreEqual(1, rule.RequestCount);
        }

        [Test]
        public void SlidingWindowRuleParrallelTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new SlidingWindowRule(mockTimer.Object, 10, 200);
            
            int goodCount = 0;
            int badCount = 0;

            // Ensure the function is multithreaded-compatible
            Parallel.For(0, 20, (i) =>
            {
                var request = rule.NewVisitAndRuleCheck();
                if (request == true)
                {
                    Interlocked.Increment(ref goodCount);
                }
                else
                {
                    Interlocked.Increment(ref badCount);
                }
            });

            Assert.AreEqual(10, goodCount);
            Assert.AreEqual(10, badCount);
        }
    }
}
