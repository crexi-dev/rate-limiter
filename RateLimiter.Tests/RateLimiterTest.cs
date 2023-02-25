using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void Set_Ratelimitter_By_10_Times_Per_Second_Limit__Send_10_Request_In_Less_Than_10_Seconds_To__Reject_Request()
        {
            //Arrange
            var requestRepository = new InMemoryRequestRepository();
            var limiter = new RateLimiter(requestRepository);
            var perTimespanStrategy = new PerTimespanRateLimitStrategy(10, TimeSpan.FromSeconds(10));
            limiter.AddLimitationRule("x", perTimespanStrategy);

            //Action
            bool allFirs9ReuestiAccepted = true;
            for (int i = 0; i < 9; i++)
            {
                if (!limiter.TryMakeRequest("john", "x"))
                    allFirs9ReuestiAccepted = false;
                break;
            }
            var lastRequestIsRejected = limiter.TryMakeRequest("john", "x");
            //Assert
            Assert.IsTrue(allFirs9ReuestiAccepted);
            Assert.IsTrue(lastRequestIsRejected);
        }
        [Test]
        public void Set_Ratelimitter_By_10_Times_Per_Second_Rule__Send_Less_Than_10_Request_To__Accept_Request()
        {
            //Arrange
            var requestRepository = new InMemoryRequestRepository();
            var limiter = new RateLimiter(requestRepository);
            var perTimespanStrategy = new PerTimespanRateLimitStrategy(10, TimeSpan.FromSeconds(10));
            limiter.AddLimitationRule("x", perTimespanStrategy);

            //Action
            var isRejected = false;
            for (int i = 0; i < 9; i++)
            {
                if (!limiter.TryMakeRequest("john", "x"))
                    isRejected = true;
            }

            //Assert
            Assert.IsFalse(isRejected);
        }
        [Test]
        public async Task Set_Ratelimitter_By_10_Seocond_Since_Last_Rule__Send_2_Requests_By_2_Seconds_Interval__Reject_Request()
        {
            //Arrange
            var requestRepository = new InMemoryRequestRepository();
            var limiter = new RateLimiter(requestRepository);
            var perTimespanStrategy = new CertainTimespanPassedSinceTheLastCall(TimeSpan.FromSeconds(10));
            limiter.AddLimitationRule("x", perTimespanStrategy);

            //Action
            limiter.TryMakeRequest("john", "x");
            await Task.Delay(2);
            var isRejected = limiter.TryMakeRequest("john", "x");

            //Assert
            Assert.IsTrue(isRejected);
        }
        [Test]
        public async Task Set_Ratelimitter_By_10_Seocond_Since_Last_Rule__Send_2_Request_By_10_Second_interval__Reject_Request()
        {
            //Arrange
            var requestRepository = new InMemoryRequestRepository();
            var limiter = new RateLimiter(requestRepository);
            var certainTimespanPassedSinceTheLastCall = new CertainTimespanPassedSinceTheLastCall(TimeSpan.FromSeconds(10));

            limiter.AddLimitationRule("x", certainTimespanPassedSinceTheLastCall);

            //Action

            limiter.TryMakeRequest("john", "x");

            await Task.Delay(10);

            var isRejected = limiter.TryMakeRequest("john", "x");

            //Assert
            Assert.IsFalse(isRejected);
        }
        [Test]
        public async Task Set_Ratelimitter_By_10_Seocond_Since_Last_And_10_Times_In_10_Seconds__Send_2_Request_By_10_Second_interval__Reject_Request()
        {
            //Arrange
            var requestRepository = new InMemoryRequestRepository();
            var limiter = new RateLimiter(requestRepository);
            var certainTimespanPassedSinceTheLastCall = new CertainTimespanPassedSinceTheLastCall(TimeSpan.FromSeconds(10));
            var perTimeSpanRateLimit = new PerTimespanRateLimitStrategy(10, TimeSpan.FromSeconds(10));
            limiter.AddLimitationRule("x", perTimeSpanRateLimit, certainTimespanPassedSinceTheLastCall);

            //Action
            limiter.TryMakeRequest("john", "x");
            await Task.Delay(2);

            var isRejected = limiter.TryMakeRequest("john", "x");

            //Assert
            Assert.IsTrue(isRejected);
        }
    }
}
