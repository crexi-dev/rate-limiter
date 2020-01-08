using NUnit.Framework;
using RateLimiter;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private IRateLimiterService _numRequestsService;
        private IRateLimiterService _intervalService;
        public RateLimiterTest()
        {
            _numRequestsService = new MaxRequestsRateLimiter();
            _intervalService = new IntervalRateLimiter();
        }
        [Test]
        public void RunNewUserForFirstTime()
        {
            var currentGuid = Guid.NewGuid();
            var isDenied = _intervalService.ShouldDenyExecution(currentGuid);
            Assert.IsFalse(isDenied);
        }

        [Test]
        public void RunNewUserTwiceBelowInterval()
        {
            var currentGuid = Guid.NewGuid();
            var isDenied = _intervalService.ShouldDenyExecution(currentGuid);
            isDenied = _intervalService.ShouldDenyExecution(currentGuid);
            Assert.IsTrue(isDenied);
        }


        [TestCase(6, 100)]
        [TestCase(6, 2000)]
        public void MaxRequestsTestsWithTimeIncrement(int numberOfCalls, int timeIncrement)
        {
            var currentGuid = Guid.NewGuid();
            var isDenied = false;
            for (int i = 0; i < numberOfCalls; i++)
            {
                isDenied = _numRequestsService.ShouldDenyExecution(currentGuid);
                Thread.Sleep(timeIncrement);
            }

            if (timeIncrement < 1000)
                Assert.IsTrue(isDenied);
            else
                Assert.IsFalse(isDenied);
        }
    }
}
