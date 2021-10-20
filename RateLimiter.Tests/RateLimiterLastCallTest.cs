using Moq;
using NUnit.Framework;
using RateLimiter.ConfigurationHelper;
using RateLimiter.Model;
using RateLimiter.Rule.RequestByLastCall;
using RateLimiter.Rule.RequestPerTimespan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterLastCallTest
    {
        [Test]
        public void Last_Request_Call_Pass_TimePeriod()
        {
            var configurationHelperMock = new Mock<IConfigurationHelper>();
            configurationHelperMock.Setup(x => x.LastRequestTimePeriod).Returns("3200");            
            var testValidator = new RequestByLastCallValitator(configurationHelperMock.Object);
            var testing = testValidator.VerifyAccess(GenerateRequests());
            Assert.True(testValidator.VerifyAccess(GenerateRequests()));
        }

        [Test]
        public void Request_per_timespan_outside_limit()
        {
            var configurationHelperMock = new Mock<IConfigurationHelper>();
            configurationHelperMock.Setup(x => x.LastRequestTimePeriod).Returns("1");            
            var testValidator = new RequestByLastCallValitator(configurationHelperMock.Object);
            Assert.False(testValidator.VerifyAccess(GenerateFutureRequests()));
        }

        private IEnumerable<Request> GenerateFutureRequests()
        {
            var dateTimeNow = DateTime.Now;
            return new List<Request>
            {
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.AddHours(1)
               },
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.AddHours(2)
               },
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.AddHours(3)
               }
            };
        }

        private IEnumerable<Request> GenerateRequests()
        {
            var dateTimeNow = DateTime.Now;
            return new List<Request>
            {
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.Subtract(new TimeSpan(0,0,1))
               },
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.Subtract(new TimeSpan(2,0,0))
               },
               new Request{
                   Token = "111111",
                   LastAccessTime = dateTimeNow.Subtract(new TimeSpan(3,0,0))
               }
            };
        }
    }
}
