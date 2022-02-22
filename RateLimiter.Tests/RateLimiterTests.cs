using Moq;
using RateLimits.History;
using RateLimits.RateLimits;
using RateLimits.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RateLimits.Tests
{
    public class RateLimiterTests
    {
        [Fact]
        public void AccessConfirmed()
        {
            //Create mock that returns empty history
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            historyRepositoryMock.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<HistoryModel>());
            historyRepositoryMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            //Inject historyRepositoryMock to RateLimiter's constructor
            var rateLimiter = new RateLimiter(historyRepositoryMock.Object);

            //Create rule that returns True everytime
            var rule = new Mock<IRule>();
            rule.Setup(x => x.Execute(It.IsAny<IEnumerable<HistoryModel>>(), It.IsAny<string>())).Returns(true);

            var result = rateLimiter.HasAccess("Someone", "Something", "Somewhere", rule.Object);

            historyRepositoryMock.Verify(x => x.Get(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            historyRepositoryMock.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Once());
            Assert.True(result);

        }


        [Fact]
        public void AccessDenied()
        {
            //Create mock that returns empty history
            var historyRepositoryMock = new Mock<IHistoryRepository>();
            historyRepositoryMock.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<HistoryModel>());
            historyRepositoryMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            //Inject historyRepositoryMock to RateLimiter's constructor
            var rateLimiter = new RateLimiter(historyRepositoryMock.Object);

            //Create rule that returns False everytime
            var rule = new Mock<IRule>();
            rule.Setup(x => x.Execute(It.IsAny<IEnumerable<HistoryModel>>(), It.IsAny<string>())).Returns(false);

            var result = rateLimiter.HasAccess("Someone", "Something", "Somewhere", rule.Object);

            historyRepositoryMock.Verify(x => x.Get(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            historyRepositoryMock.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.False(result);

        }
    }
}
