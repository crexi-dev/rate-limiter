using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Rules;
using System;
using System.Threading;

namespace RateLimiter.Tests.RulesTest
{
    [TestFixture]
    public class MaxRequestsRateLimiterTest
    {
        private IRateLimiter rateLimiter = new MaxRequestsPerTimespanRateLimiter();


        [Test]
        public void ValidateSingleRequest()
        {
            var result = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should pass as the max request per minute is 2
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateWithTwoRequests()
        {
            rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            var result = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should pass as the max request per minute is 2
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateWithThreeRequests()
        {
            rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            var result1 = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should pass as the max request per minute is 2
            Assert.That(result1, Is.True);

            var result2 = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should return false as we are trying to request GenerateLabelService for a third time in 1 min
            Assert.That(result2, Is.False);
        }

        [Test]
        public void ValidateWithThreeRequestsAddWait()
        {
            rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            var result1 = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should pass as the max request per minute is 2
            Assert.That(result1, Is.True);

            // Sleep for a little over 1 min
            Thread.Sleep(60001);

            var result2 = rateLimiter.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now);
            // This should return false as we are trying to request GenerateLabelService for a third time in 1 min
            Assert.That(result2, Is.True);
        }
    }
}
