using Moq;
using NUnit.Framework;
using System.Linq;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class TimespanPassedSinceTheLastStrategyTest
    {
        [Test]
        public void IsRequestAllowed_Returns_True_When_There_Is_No_Request_Log_For_User()
        {
            //Arrange
            var timeService = new Mock<ITimeService>();
            var requestRepository = new Mock<IRequestRepository>();
            requestRepository.Setup(t => t.GetLastRequest(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns((RequestLog)null);

            var strategy = new TimespanPassedSinceTheLastStrategy(TimeSpan.FromSeconds(10), timeService.Object);

            //Action
            var result = strategy.IsRequestAllowed("john", "x", requestRepository.Object);

            //Assert
            Assert.IsTrue(result);
        }
        [Test]
        public void IsRequestAllowed_Returns_False_When_Passed_Less_Than_Certain_Time_From_The_Last()
        {
            //Arrange
            var time = DateTime.Parse("2022-02-02");
            var timeService = new Mock<ITimeService>();
            timeService.Setup(t => t.GetCurrentTime).Returns(time);

            var requestRepository = new Mock<IRequestRepository>();
            requestRepository.Setup(t => t.GetLastRequest(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new RequestLog() { RequestTime = time - TimeSpan.FromSeconds(2) });

            var strategy = new TimespanPassedSinceTheLastStrategy(TimeSpan.FromSeconds(10), timeService.Object);

            //Action
            var result = strategy.IsRequestAllowed("john", "x", requestRepository.Object);

            //Assert
            Assert.IsFalse(result);
        }
        [Test]
        public void IsRequestAllowed_Returns_True_When_Passed_More_Than_Certain_Time_From_The_Last()
        {
            //Arrange
            var time = DateTime.Parse("2022-02-02");
            var timeService = new Mock<ITimeService>();
            timeService.Setup(t => t.GetCurrentTime).Returns(time);

            var requestRepository = new Mock<IRequestRepository>();
            requestRepository.Setup(t => t.GetLastRequest(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new RequestLog() { RequestTime = time - TimeSpan.FromSeconds(12) });

            var strategy = new TimespanPassedSinceTheLastStrategy(TimeSpan.FromSeconds(10), timeService.Object);

            //Action
            var result = strategy.IsRequestAllowed("john", "x", requestRepository.Object);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
