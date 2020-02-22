using NUnit.Framework;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    internal class RequestFromCountryTest
    {
        [Test]
        public void Rule_Match_Country()
        {
            var rule = new RequestFromCountry("US");

            var rslt = rule.Execute(new RequestInfo { Country = "US" });

            Assert.IsTrue(rslt);
        }

        [Test]
        public void Rule_DoesNot_Match_Country()
        {
            var rule = new RequestFromCountry("US");

            var rslt = rule.Execute(new RequestInfo { Country = "UK" });

            Assert.IsFalse(rslt);
        }
    }
}