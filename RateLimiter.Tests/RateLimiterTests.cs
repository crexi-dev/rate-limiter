using NUnit.Framework;
using RateLimiter.Classes;
using RateLimiter.Rules;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    class RateLimiterTests
    {
        private RateLimiterManager _rateLimiter;
        ClientRequest _requestUS;
        ClientRequest _requestEU;
        
        [SetUp]
        public void SetUp()
        {
            _rateLimiter = new RateLimiterManager();
            _requestUS = new ClientRequest("token1", "172.10.10.10");
            _requestEU = new ClientRequest("token2", "188.10.10.10");
        }

        [Test]
        [TestCase(RulesSettings.MaxRequestsLimit, true)]
        [TestCase(RulesSettings.MaxRequestsLimit + 1, false)]
        public void GeoLocationLimit_USCallMaxRequestsNotReached_ReturnTrueOrFalse(int a, bool expectedResult)
        {
            _rateLimiter.AddRule("ApiFunction1", new GeoLocationLimit());
            var result = false;
            for (int i = 0; i < a; i++)
                result = _rateLimiter.Validate("ApiFunction1", _requestUS);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void GeoLocationLimit_EUCallTimespanSinceLastCallLimit_ReturnTrueOrFalse(bool delay_enabled, bool expectedResult)
        {            
            _rateLimiter.AddRule("ApiFunction1", new GeoLocationLimit());
            var result = false;

            for (int i = 0; i < RulesSettings.MaxRequestsLimit + 1; i++) // hit the max limit
                result = _rateLimiter.Validate("ApiFunction1", _requestEU);

            if (delay_enabled)
            {
                int sleep_time = Convert.ToInt32(Math.Round(RulesSettings.TimeSpanLimit.TotalSeconds / RulesSettings.MaxRequestsLimit) + 5);
                Thread.Sleep(sleep_time * 1000);
            }

            result = _rateLimiter.Validate("ApiFunction1", _requestEU); // add 1 new request

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void RequestsPerTimespanLimit_USCallMaxRequestsReachedForOneTokenAddNewToken_ReturnTrue()
        {
            _rateLimiter.AddRule("ApiFunction1", new RequestsPerTimespanLimit());
            var result = false;
            for (int i = 0; i < RulesSettings.MaxRequestsLimit + 5; i++) // "DDOS" the server
                result = _rateLimiter.Validate("ApiFunction1", _requestUS);

            // add new call from diff user
            result = _rateLimiter.Validate("ApiFunction1", _requestEU);

            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void RequestsPerTimespanLimit_USCallMaxRequestsReachedForOneToken_ReturnFalse()
        {
            _rateLimiter.AddRule("ApiFunction1", new RequestsPerTimespanLimit());
            var result = false;
            for (int i = 0; i < RulesSettings.MaxRequestsLimit + 10; i++) // "DDOS" the server
                result = _rateLimiter.Validate("ApiFunction1", _requestUS);

            Assert.That(result, Is.EqualTo(false));
        }
    }
}
