using Moq;
using NUnit.Framework;
using RateLimiter.RateLimiter;
using RateLimiter.RateLimiter.Models;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    public class RateLimitProcessorTest
    {
        private const string DefaultPolicyName = "testPolicy";
        private const Region DefaultRegion = Region.EU;

        private RateLimitProcessor _subject;

        [Test]
        public void VerifyRequest_LimiterExistsForPolicyAndLimitNotExceeded_ShouldAllowRequest()
        {
            var limiter = new Mock<ILimiter>();
            limiter
                .Setup(x => x.CheckLimit(It.IsAny<ClientRequest>()))
                .Returns(new LimitResult { Limited = false, CurrentLimit = 1, RemainingAmountOfCalls = 1 });

            var data = new Dictionary<(string, Region), ILimiter>()
            {
                { (DefaultPolicyName, DefaultRegion), limiter.Object }
            };

            _subject = new RateLimitProcessor(data);

            var result = _subject.VerifyRequest("token", DefaultRegion, "resource", [DefaultPolicyName]);

            Assert.That(result.Limited, Is.False);
            Assert.That(result.CurrentLimit, Is.EqualTo(1));
            Assert.That(result.RemainingAmountOfCalls, Is.EqualTo(1));
        }

        [Test]
        public void VerifyRequest_LimiterNotExistForPolicy_ShouldAllowRequest()
        {
            var data = new Dictionary<(string, Region), ILimiter>();

            _subject = new RateLimitProcessor(data);

            var result = _subject.VerifyRequest("token", DefaultRegion, "resource", [DefaultPolicyName]);

            Assert.That(result.Limited, Is.False);
            Assert.That(result.CurrentLimit, Is.EqualTo(0));
            Assert.That(result.RemainingAmountOfCalls, Is.EqualTo(0));
        }
    }
}
