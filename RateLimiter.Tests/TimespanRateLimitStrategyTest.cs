using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class TimespanRateLimitStrategyTest
    {
        [Test]
        public void IsRequestAllowed_Returns_True_When_There_Is_Less_Than_Certain_Number_In_Certain_TimeSpan()
        {
            //Arrange
            var requestRepository = new Mock<IRequestRepository>();
            requestRepository.Setup(t => t.GetRequestsByTimeSpan(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                    .Returns(Enumerable.Repeat(Mock.Of<RequestLog>(), 2).ToList());
            
            var strategy = new TimespanRateLimitStrategy(3,TimeSpan.FromSeconds(10));

            //Action
            var result = strategy.IsRequestAllowed("john", "x", requestRepository.Object,"");

            //Assert
            Assert.IsTrue(result);
        }
        [Test]
        public void IsRequestAllowed_Returns_True_When_There_Is_More_Than_Certain_Number_In_Certain_TimeSpan()
        {
            //Arrange
            var requestRepository = new Mock<IRequestRepository>();
            requestRepository.Setup(t => t.GetRequestsByTimeSpan(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                    .Returns(Enumerable.Repeat(Mock.Of<RequestLog>(), 4).ToList());

            var strategy = new TimespanRateLimitStrategy(3, TimeSpan.FromSeconds(10));

            //Action
            var result = strategy.IsRequestAllowed("john", "x", requestRepository.Object,"");

            //Assert
            Assert.IsFalse(result);
        }
    }
}
