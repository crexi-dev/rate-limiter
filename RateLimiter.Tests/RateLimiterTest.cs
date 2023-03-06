using Moq;
using NUnit.Framework;
using RateLimiter.Repositories;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private Mock<IClientRepository> _clientRepository;

        [SetUp]
        public void Init()
        {
            _clientRepository = new Mock<IClientRepository>();
        }

        [Test]
        [TestCase("TestToken1")]
        public void CanMakeRequest_WithNoExceeded_ReturnTrue(string token)
        {
            _clientRepository
                .Setup(x => x.GetRulesOfResource(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new System.Collections.Generic.List<Model.RateLimiterRule>
            {
                 Model.RateLimiterRule.Create(TimeSpan.FromMinutes(3),3)
            });

            var rateLimiter = new RateLimiter(_clientRepository.Object);

            var result1 = rateLimiter.CanMakeRequest(token, "Path");
            var result2 = rateLimiter.CanMakeRequest(token, "Path");
            var result3 = rateLimiter.CanMakeRequest(token, "Path");

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
        }

        [Test]
        [TestCase("TestToken2")]
        public void CanMakeRequest_WithExceeded_ReturnFalse(string token)
        {
            _clientRepository
                .Setup(x => x.GetRulesOfResource(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new System.Collections.Generic.List<Model.RateLimiterRule>
            {
                 Model.RateLimiterRule.Create(TimeSpan.FromMinutes(3),1)
            });

            var rateLimiter = new RateLimiter(_clientRepository.Object);

            rateLimiter.CanMakeRequest(token, "Path");

            var result = rateLimiter.CanMakeRequest(token, "Path");

            Assert.IsFalse(result);
        }

        [Test]
        [TestCase("TestToken3")]
        public void CanMakeRequest_WithNoRules_ReturnTrue(string token)
        {
            _clientRepository
                .Setup(x => x.GetRulesOfResource(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new System.Collections.Generic.List<Model.RateLimiterRule>
                {
                });

            var rateLimiter = new RateLimiter(_clientRepository.Object);

            rateLimiter.CanMakeRequest(token, "Path");

            var result = rateLimiter.CanMakeRequest(token, "Path");

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("TestToken4")]
        public void CanMakeRequest_WithPassAllrules_ReturnTrue(string token)
        {
            _clientRepository
                .Setup(x => x.GetRulesOfResource(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new System.Collections.Generic.List<Model.RateLimiterRule>
                {
                   Model.RateLimiterRule.Create(TimeSpan.FromMinutes(3),3),
                   Model.RateLimiterRule.Create(TimeSpan.FromMinutes(2),2)
                });

            var rateLimiter = new RateLimiter(_clientRepository.Object);

            rateLimiter.CanMakeRequest(token, "Path");
            var result = rateLimiter.CanMakeRequest(token, "Path");

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("TestToken5")]
        public void CanMakeRequest_WithFailAtLeastOneRule_ReturnFalse(string token)
        {
            _clientRepository
                .Setup(x => x.GetRulesOfResource(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new System.Collections.Generic.List<Model.RateLimiterRule>
                {
                   Model.RateLimiterRule.Create(TimeSpan.FromMinutes(3),3),
                   Model.RateLimiterRule.Create(TimeSpan.FromMinutes(2),1)
                });

            var rateLimiter = new RateLimiter(_clientRepository.Object);

            rateLimiter.CanMakeRequest(token, "Path");
            var result = rateLimiter.CanMakeRequest(token, "Path");

            Assert.IsFalse(result);
        }

        [Test]
        [TestCase("TestToken6")]
        public void CanMakeRequest_WithWhenOneRuleWithLessTimePeriodFails_ReturnFalse(string token)
        {
            _clientRepository
                .Setup(x => x.GetRulesOfResource(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new System.Collections.Generic.List<Model.RateLimiterRule>
                {
                   Model.RateLimiterRule.Create(TimeSpan.FromMinutes(3),2),
                   Model.RateLimiterRule.Create(TimeSpan.FromMinutes(1),1)
                });

            var rateLimiter = new RateLimiter(_clientRepository.Object);

            rateLimiter.CanMakeRequest(token, "Path");
            var result = rateLimiter.CanMakeRequest(token, "Path");

            Assert.IsFalse(result);
        }

        [Test]
        [TestCase("TestToken7")]
        public void CanMakeRequest_WithWhenAllRulePass_ReturnTrue(string token)
        {
            _clientRepository
                .Setup(x => x.GetRulesOfResource(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new System.Collections.Generic.List<Model.RateLimiterRule>
                {
                   Model.RateLimiterRule.Create(TimeSpan.FromMinutes(3),2),
                   Model.RateLimiterRule.Create(TimeSpan.FromMinutes(2),2)
                });

            var rateLimiter = new RateLimiter(_clientRepository.Object);

            var result1 = rateLimiter.CanMakeRequest(token, "Path");
            var result2 = rateLimiter.CanMakeRequest(token, "Path");

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
        }

        [Test]
        [TestCase("TestToken8")]
        public void CanMakeRequest_WithWhenRulesAreNull_ReturnTrue(string token)
        {
            _clientRepository
                .Setup(x => x.GetRulesOfResource(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(default(System.Collections.Generic.List<Model.RateLimiterRule>));

            var rateLimiter = new RateLimiter(_clientRepository.Object);

            rateLimiter.CanMakeRequest(token, "Path");
            var result = rateLimiter.CanMakeRequest(token, "Path");

            Assert.IsTrue(result);
        }
    }
}
