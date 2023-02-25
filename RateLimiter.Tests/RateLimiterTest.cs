using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        
        [Test]
        public void TryMakeRequest_Returns_False_When_There_Is_No_Strategy()
        {
            //Arrange
            var requestRepository = new Mock<IRequestRepository>();
            var limiter = new RateLimiter(requestRepository.Object);

            //Action
            var result = limiter.TryMakeRequest("john", "x");

            //Assert
            Assert.IsTrue(result);
        }
        [Test]
        public void TryMakeRequest_Returns_True_When_All_Strategies_Return_True()
        {
            //Arrange
            var requestRepository = new Mock<IRequestRepository>();
            var rateLimitStrategy1 = new Mock<IRateLimitStrategy>();
            var rateLimitStrategy2 = new Mock<IRateLimitStrategy>();

            rateLimitStrategy1.Setup(t => t.IsRequestAllowed(It.IsAny<string>(), It.IsAny<string>(), requestRepository.Object,It.IsAny<string>()))
                    .Returns(true);
            rateLimitStrategy2.Setup(t => t.IsRequestAllowed(It.IsAny<string>(), It.IsAny<string>(), requestRepository.Object, It.IsAny<string>()))
                    .Returns(true);

            var limiter = new RateLimiter(requestRepository.Object);
            limiter.AddLimitationRule("x",rateLimitStrategy1.Object,rateLimitStrategy2.Object);
            //Action
            var result = limiter.TryMakeRequest("john", "x");

            //Assert
            Assert.IsTrue(result);
        }
        [Test]
        public void TryMakeRequest_Returns_False_When_Any_Of_Strategies_Return_True()
        {
            //Arrange
            var requestRepository = new Mock<IRequestRepository>();
            var rateLimitStrategy1 = new Mock<IRateLimitStrategy>();
            var rateLimitStrategy2 = new Mock<IRateLimitStrategy>();

            rateLimitStrategy1.Setup(t => t.IsRequestAllowed(It.IsAny<string>(), It.IsAny<string>(), requestRepository.Object, It.IsAny<string>()))
                    .Returns(true);
            rateLimitStrategy2.Setup(t => t.IsRequestAllowed(It.IsAny<string>(), It.IsAny<string>(), requestRepository.Object, It.IsAny<string>()))
                    .Returns(false);

            var limiter = new RateLimiter(requestRepository.Object);
            limiter.AddLimitationRule("x", rateLimitStrategy1.Object, rateLimitStrategy2.Object);
            //Action
            var result = limiter.TryMakeRequest("john", "x");

            //Assert
            Assert.IsFalse(result);
        }
    }
}
