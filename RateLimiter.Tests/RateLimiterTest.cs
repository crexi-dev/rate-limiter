using Moq;
using NUnit.Framework;
using RateLimiter.ConfigurationHelper;
using RateLimiter.Interface;
using RateLimiter.Model;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private Mock<IRateLimiterRepository> _repositoryMock;
        private Mock<IConfigurationHelper> _configHelperMock;
        public RateLimiterTest()
        {
            _repositoryMock = new Mock<IRateLimiterRepository>();
            _configHelperMock = new Mock<IConfigurationHelper>();
        }
        [Test]
        public void AccessVerifcation_Return_False_Token_Is_Not_Set()
        {
            _configHelperMock.Setup(x => x.MaxRequestTimespan).Returns("3600");
            _repositoryMock.Setup(x => x.RetrieveRequestByBeginningTime<Request>(string.Empty, DateTime.Now)).Returns(GenerateRequests());
            var test = new AccessVerification(_repositoryMock.Object, _configHelperMock.Object);
            var result = test.EnableAccess(null, string.Empty);

            Assert.False(result);
        }

        [Test]
        public void AccessVerifcation_Return_True_No_Rule_Set()
        {
            _configHelperMock.Setup(x => x.MaxRequestTimespan).Returns("3600");
            _repositoryMock.Setup(x => x.RetrieveRequestByBeginningTime<Request>(string.Empty, DateTime.Now)).Returns(GenerateRequests());
            var test = new AccessVerification(_repositoryMock.Object, _configHelperMock.Object);
            var result = test.EnableAccess(null, "1111");

            Assert.True(result);
        }


        [Test]
        public void AccessVerifcation_Return_True_With_Rules()
        {
            _configHelperMock.Setup(x => x.MaxRequestTimespan).Returns("3600");
            _configHelperMock.Setup(x => x.MaxRequestCountPerTimespan).Returns("1");
            _configHelperMock.Setup(x => x.LastRequestTimePeriod).Returns("1");
            
            var repositoryMock =  new Mock<IRateLimiterRepository>();
            repositoryMock.Setup(x => x.RetrieveRequestByBeginningTime<Request>(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(GenerateRequests());
            var test = new AccessVerification(repositoryMock.Object, _configHelperMock.Object);
            var rule = new Mock<IRateLimiterVerification>();
            rule.Setup(x => x.VerifyAccess(It.IsAny<IEnumerable<Request>>())).Returns(true);

            var rules = new List<IRateLimiterVerification>();
            rules.Add(rule.Object);
            var result = test.EnableAccess(rules, "1111");

            Assert.True(result);
        }

        [Test]
        public void AccessVerifcation_Return_False_With_Failed_Rules()
        {
            _configHelperMock.Setup(x => x.MaxRequestTimespan).Returns("3600");
            _configHelperMock.Setup(x => x.MaxRequestCountPerTimespan).Returns("1");
            _configHelperMock.Setup(x => x.LastRequestTimePeriod).Returns("1");

            var repositoryMock = new Mock<IRateLimiterRepository>();
            repositoryMock.Setup(x => x.RetrieveRequestByBeginningTime<Request>(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(GenerateRequests());
            var test = new AccessVerification(repositoryMock.Object, _configHelperMock.Object);
            var rule = new Mock<IRateLimiterVerification>();
            rule.Setup(x => x.VerifyAccess(It.IsAny<IEnumerable<Request>>())).Returns(false);

            var rules = new List<IRateLimiterVerification>();
            rules.Add(rule.Object);
            var result = test.EnableAccess(rules, "1111");

            Assert.False(result);
        }
        private IEnumerable<Request> GenerateRequests()
        {
            var dateTimeNow = DateTime.Now;
            return new List<Request>
            {
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.Subtract(new TimeSpan(0,0,3))
               },
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.Subtract(new TimeSpan(0,0,10))
               },
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.Subtract(new TimeSpan(0,0,7))
               }
            };
        }
    }
}
