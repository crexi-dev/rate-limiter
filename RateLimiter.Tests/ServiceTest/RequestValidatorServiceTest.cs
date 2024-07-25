using Moq;
using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests.ServiceTest
{
    [TestFixture]
    public class RequestValidatorServiceTest
    {
        private Mock<IExtendableRuleFactory> factory;
        private Mock<IRateLimiter> mockRule;
        private RequestValidatorService service;

        [SetUp]
        public void SetUp()
        {
            factory = new Mock<IExtendableRuleFactory>();
            
            mockRule = new Mock<IRateLimiter>();
            mockRule.Setup(x => x.Acquire(Enums.ServiceType.GenerateLabelService, "user1", DateTime.Now)).Returns(true);

            service = new RequestValidatorService(factory.Object);
        }

        [Test]
        public void ValidateSingleRequest()
        {
            factory.Setup(x => x.GetRulesByServiceType(Enums.ServiceType.GenerateLabelService)).Returns(new List<IRateLimiter> { mockRule.Object });

            UserRequest request = new UserRequest()
            {
                userToken = "user1",
                requestedServiceType = Enums.ServiceType.GenerateLabelService,
                requestedDate = DateTime.Now
            };

            var result = service.ValidateUserRequest(request);

            Assert.That(result, Is.False);
        }
    }
}
