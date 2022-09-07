using Core.Models;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using System;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class LimitedRequestPerTimespanHandlerTest
    {
        private LimitedRequestPerTimespanHandler handler_;
        private RateLimitDecorator decorator_;
        private string clientKey_;

        [OneTimeSetUp]
        public void Setup()
        {
            clientKey_ = "sampleInCacheKey";
            var clientData = new ClientData(DateTime.UtcNow, 1);

            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.Set(clientKey_, clientData);

            handler_ = new LimitedRequestPerTimespanHandler(cache);

            decorator_ = new RateLimitDecorator();
        }

        [Test]
        public void IsRateLimitSucceded_Succeeds()
        {
            // Setup.
            decorator_.TimeSpan = 10;
            decorator_.MaxRequests = 1;
            bool expectedResult = true;

            // Execution.
            bool actualResult = handler_.IsRateLimitSucceded(decorator_, clientKey_);

            // Assertion.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsRateLimitSucceded_NotSucceeds()
        {
            // Setup.
            decorator_.TimeSpan = 0;
            decorator_.MaxRequests = 0;
            bool expectedResult = false;

            // Execution.
            bool actualResult = handler_.IsRateLimitSucceded(decorator_, clientKey_);

            // Assertion.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsRateLimitSucceded_ClientFirstAttemp()
        {
            // Setup.
            var notFoundKey = "sampleNotInCacheKey";
            bool expectedResult = false;

            // Execution.
            bool actualResult = handler_.IsRateLimitSucceded(decorator_, notFoundKey);

            // Assertion.
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void UpdateClientData_Succeeds()
        {
            // Setup.
            decorator_.MaxRequests = 2;

            ClientData clientData = handler_.GetClientDataByKey(clientKey_);
            var previousSuccessfullCallTime = clientData.PreviousSuccessfullCallTime;

            // Execution.
            handler_.UpdateClientData(decorator_, clientKey_);

            // Assertion.
            ClientData updatedClientData = handler_.GetClientDataByKey(clientKey_);
            var previousSuccessfullCallTimeAfterUpdate = updatedClientData.PreviousSuccessfullCallTime;
            Assert.That(
                previousSuccessfullCallTimeAfterUpdate,
                Is.GreaterThanOrEqualTo(previousSuccessfullCallTime));
        }
    }
}
