using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RateLimiter.Business.Rules;
using RateLimiter.Factories;
using RateLimiter.Interfaces.Configuration;
using RateLimiter.Interfaces.DataAccess;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.Factories
{
    [TestClass]
    public class RateLimitRuleFactoryTests
    {
        private Mock<IRateLimitRepository> _repository;

        [TestInitialize]
        public void Initialize()
        {
            _repository = new Mock<IRateLimitRepository>();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void InvalidConstructorTest()
        {
            new RateLimitRuleFactory(null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void UnsupportedTypeCreateTest()
        {
            var target = new RateLimitRuleFactory(_repository.Object);

            var configuration = new Mock<IRateLimitRuleConfiguration>();
            configuration.Setup(x => x.Type).Returns("123");
            target.Create(configuration.Object);
        }

        [DataRow(typeof(CadenceRateLimitRule))]
        [DataRow(typeof(RequestsPerWindowRateLimitRule))]
        public void CreateTest(Type expectedType)
        {
            var parameters = new Dictionary<string, object>
            {
                { "MinimumDelayBetweenRequestsMs", 1},
                { "WindowMs", 1000},
                { "RequestLimit", 5}
            };

            var target = new RateLimitRuleFactory(_repository.Object);

            var configuration = new Mock<IRateLimitRuleConfiguration>();
            configuration.Setup(x => x.Type).Returns(expectedType.Name);
            configuration.Setup(x => x.Parameters).Returns(parameters);

            var actual = target.Create(configuration.Object);

            Assert.IsInstanceOfType(actual, expectedType);
        }

        [TestMethod]
        public void SupportsTypeTest()
        {
            var target = new RateLimitRuleFactory(_repository.Object);

            Assert.IsTrue(target.SupportsType(nameof(CadenceRateLimitRule)));
            Assert.IsTrue(target.SupportsType(nameof(RequestsPerWindowRateLimitRule)));
        }
    }
}
