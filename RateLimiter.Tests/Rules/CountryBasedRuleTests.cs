using Moq;
using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Storage;
using System.Collections.Generic;

namespace RateLimiter.Tests.Rules
{
    [TestFixture]
    public class CountryBasedRuleTests
    {
        [Test]
        public void IsRequestAllowed_CountryWithoutRules_ReturnsTrue()
        {
            // arrange
            string resource = "resource";
            string token = "token";
            string tokenCountry = "us";

            DataStorage.TokenOrigins = new Dictionary<string, string>
            {
                { token, tokenCountry }
            };

            var rules = new Dictionary<string, IEnumerable<IRateLimitRule>>
            {
                { tokenCountry, new List<IRateLimitRule> { } }
            };

            var sut = new CountryBasedRule(rules);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.True(actualResult);
        }

        [Test]
        public void IsRequestAllowed_CountryWithoutFailingRules_ReturnsTrue()
        {
            // arrange
            string resource = "resource";
            string token = "token";
            string tokenCountry = "us";

            DataStorage.TokenOrigins = new Dictionary<string, string>
            {
                { token, tokenCountry }
            };

            Mock<IRateLimitRule> rateLimiter = new Mock<IRateLimitRule>();
            rateLimiter.Setup(x => x.IsRequestAllowed(resource, token))
                .Returns(true);

            var rules = new Dictionary<string, IEnumerable<IRateLimitRule>>
            {
                { tokenCountry, new List<IRateLimitRule> { rateLimiter.Object } }
            };

            var sut = new CountryBasedRule(rules);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.True(actualResult);
        }

        [Test]
        public void IsRequestAllowed_CountryWithOneFailingRule_ReturnsFalse()
        {
            // arrange
            string resource = "resource";
            string token = "token";
            string tokenCountry = "us";

            DataStorage.TokenOrigins = new Dictionary<string, string>
            {
                { token, tokenCountry }
            };
            
            var rateLimiter1 = new Mock<IRateLimitRule>();
            rateLimiter1.Setup(x => x.IsRequestAllowed(resource, token))
                .Returns(true);

            var rateLimiter2 = new Mock<IRateLimitRule>();
            rateLimiter2.Setup(x => x.IsRequestAllowed(resource, token))
                .Returns(false);

            var rules = new Dictionary<string, IEnumerable<IRateLimitRule>>
            {
                { tokenCountry, new List<IRateLimitRule> { rateLimiter1.Object, rateLimiter2.Object } }
            };

            var sut = new CountryBasedRule(rules);

            // act
            var actualResult = sut.IsRequestAllowed(resource, token);

            // assert
            Assert.False(actualResult);
        }
    }
}
