using NUnit.Framework;
using RateLimiter.BLL;
using RateLimiter.Entities;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        [TestCase("token1", true)]
        [TestCase("token2", false)]
        public void CountryRuleRequest(string token, bool expectedResult)
        {
            var result = RequestManager.SendRequest(new RequestModel() { ClientToken = token });
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase("token3", true)]
        [TestCase("token3", false)]
        public void LastCallRuleRequest(string token, bool expectedResult)
        {
            var result = RequestManager.SendRequest(new RequestModel() { ClientToken = token });
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase("token4", true)]
        [TestCase("token4", false)]
        [TestCase("token5", true)]
        public void LastCallForCountryRuleRequest(string token, bool expectedResult)
        {
            var result = RequestManager.SendRequest(new RequestModel() { ClientToken = token });
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase("token6", true)]
        [TestCase("token7", true)]
        [TestCase("token6", false)]
        public void RequestCountForPeriodRuleRequest(string token, bool expectedResult)
        {
            var result = RequestManager.SendRequest(new RequestModel() { ClientToken = token });
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase("token8", true)]
        [TestCase("token8", false)]
        [TestCase("token9", true)]
        public void RequestCountRuleRequest(string token, bool expectedResult)
        {
            var result = RequestManager.SendRequest(new RequestModel() { ClientToken = token });
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase("token10", false)]
        [TestCase("token11", true)]
        public void RequestCountForCountryRuleRequest(string token, bool expectedResult)
        {
            var result = RequestManager.SendRequest(new RequestModel() { ClientToken = token });
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase("token12", true)]
        [TestCase("token12", true)]
        [TestCase("token12", false)]
        public void RequestCountForCountryForPeriodRuleRequest(string token, bool expectedResult)
        {
            var result = RequestManager.SendRequest(new RequestModel() { ClientToken = token });
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase("token13", true)]
        [TestCase("token13", true)]
        [TestCase("token13", true)]
        [TestCase("token13", false)]
        [TestCase("token13", false)]
        [TestCase("token14", true)]
        [TestCase("token14", false)]
        [TestCase("token14", false)]
        [TestCase("token14", false)]
        [TestCase("token14", false)]

        public void MultipleRulesRequest(string token, bool expectedResult)
        {
            var result = RequestManager.SendRequest(new RequestModel() { ClientToken = token });
            Assert.AreEqual(expectedResult, result);
        }
    }
}
