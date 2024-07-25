using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Rules;
using System;
using System.Threading;


namespace RateLimiter.Tests.RulesTest
{
    [TestFixture]
    public class TimeSpanPassedRateLimiterTest
    {
        private IRateLimiter rateLimiter = new TimespanPassedRateLimiter();

        [Test]
        public void ValidateSingleRequest()
        {
            var result = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should pass as we are only requesting GenerateLabelService once
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateTwoRequests()
        {
            rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            var result = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should fail as we didnt wait 1 min before requesting the service again
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidateTwoRequestsWithWait()
        {
            rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);

            // Sleep for a little over 1 min
            Thread.Sleep(30001);

            var result = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should fail as we didnt wait 1 min before requesting the service again
            Assert.That(result, Is.True);
        }
    }
}
