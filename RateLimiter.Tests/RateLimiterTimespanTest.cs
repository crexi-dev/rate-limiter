using Moq;
using NUnit.Framework;
using RateLimiter.ConfigurationHelper;
using RateLimiter.Model;
using RateLimiter.Rule.RequestPerTimespan;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTimespanTest
    {
        [Test]
        public void Request_per_timespan_within_limit()
        {
            var configurationHelperMock = new Mock<IConfigurationHelper>();
            configurationHelperMock.Setup(x => x.MaxRequestTimespan).Returns("5");
            configurationHelperMock.Setup(x => x.MaxRequestCountPerTimespan).Returns("4");            
            var testValidator = new RequestPerTimespanValidator(configurationHelperMock.Object);
            Assert.True(testValidator.VerifyAccess(GenerateRequests()));
        }

        [Test]
        public void Request_per_timespan_outside_limit()
        {
            var configurationHelperMock = new Mock<IConfigurationHelper>();
            configurationHelperMock.Setup(x => x.MaxRequestTimespan).Returns("3600");
            configurationHelperMock.Setup(x => x.MaxRequestCountPerTimespan).Returns("1");
            var testValidator = new RequestPerTimespanValidator(configurationHelperMock.Object);
            Assert.False(testValidator.VerifyAccess(GenerateRequests()));
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
