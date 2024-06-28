using NUnit.Framework;
using RateLimiter.RateLimiter.Options;
using RateLimiter.RateLimiter;
using System;
using RateLimiter.RateLimiter.Models;

namespace RateLimiter.Tests
{
    public class TimespanLimiterTest
    {
        private TimespanLimiter _subject;
        private TimespanLimiterOptions _options;

        [SetUp]
        public void Setup()
        {
            _options = new TimespanLimiterOptions
            {
                Window = TimeSpan.FromSeconds(3),
            };

            _subject = new TimespanLimiter(_options);
        }

        [Test]
        public void CheckLimit_FirstRequest_ShouldAllowRequest()
        {
            var clientRequest = new ClientRequest
            {
                LastHitAt = DateTime.MinValue.ToUniversalTime(),
                AmountOfHits = 0,
            };

            var result = _subject.CheckLimit(clientRequest);

            Assert.That(result.Limited, Is.False);
            Assert.That(result.CurrentLimit, Is.EqualTo(_options.Limit));
            Assert.That(result.RemainingAmountOfCalls, Is.EqualTo(0));
            Assert.That(result.RetryAfterSeconds, Is.EqualTo(_options.Window.TotalSeconds));
        }

        [Test]
        public void CheckLimit_SecondRequest_ShouldNotAllowRequest()
        {
            var lastRequestDelay = 1;
            var clientRequest = new ClientRequest
            {
                LastHitAt = DateTime.UtcNow.AddSeconds(-lastRequestDelay),
                AmountOfHits = 0,
            };

            var result = _subject.CheckLimit(clientRequest);

            Assert.That(result.Limited, Is.True);
            Assert.That(result.CurrentLimit, Is.EqualTo(_options.Limit));
            Assert.That(result.RemainingAmountOfCalls, Is.EqualTo(0));
            Assert.That(result.RetryAfterSeconds, Is.EqualTo(_options.Window.TotalSeconds - lastRequestDelay));
        }
    }
}
