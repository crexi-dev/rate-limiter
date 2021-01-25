using NUnit.Framework;
using System;
using System.Threading;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        public RateLimiterTest() { }

        [Test]
        public void RequestManager_Creates_APIRequest()
        {
            RequestManager Manager = new RequestManager();
            APIRequest CurrentRequest = Manager.CreateRequest("Customer001Token");

            Assert.IsTrue(CurrentRequest != null);
        }

        [Test]
        public void Request_Satisfies_TimeSpanSinceLastCallLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIRequest CurrentRequest = Manager.CreateRequest("Customer001Token");

            if (!Limitations.IsSatisfied(new TimeSpanSinceLastCallLimitation(1), CurrentRequest))
                throw new TimeSpanSinceLastCallLimitationException();
        }

        [Test]
        public void SecondRequest_Satisfies_TimeSpanSinceLastCallLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIRequest CurrentRequest = Manager.CreateRequest("Customer001Token");
            Thread.Sleep(1000);
            CurrentRequest = Manager.CreateRequest("Customer001Token");

            if (!Limitations.IsSatisfied(new TimeSpanSinceLastCallLimitation(1), CurrentRequest))
                throw new TimeSpanSinceLastCallLimitationException();
        }

        [Test]
        public void Request_Satisfies_XRequestsPerTimeSpanLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIRequest CurrentRequest = Manager.CreateRequest("Customer001Token");

            if (!Limitations.IsSatisfied(new XRequestsPerTimeSpanLimitation(20000000, 1), CurrentRequest))
                throw new XRequestsPerTimeSpanException();
        }

        [Test]
        public void SecondRequest_Satisfies_XRequestsPerTimeSpanLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIRequest CurrentRequest = Manager.CreateRequest("Customer001Token");
            CurrentRequest = Manager.CreateRequest("Customer001Token");

            if (!Limitations.IsSatisfied(new XRequestsPerTimeSpanLimitation(20000000, 2), CurrentRequest))
                throw new XRequestsPerTimeSpanException();
        }

        [Test]
        public void Request_DoesNot_Satisfies_TimeSpanSinceLastCallLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIRequest CurrentRequest = Manager.CreateRequest("Customer001Token");
            CurrentRequest = Manager.CreateRequest("Customer001Token");

            try
            {
                if (!Limitations.IsSatisfied(new TimeSpanSinceLastCallLimitation(1), CurrentRequest))
                    throw new TimeSpanSinceLastCallLimitationException();
            }
            catch(Exception Ex)
            {
                if (!(Ex is TimeSpanSinceLastCallLimitationException))
                    Assert.Fail();
            }
        }

        [Test]
        public void Request_DoesNot_Satisfies_XRequestsPerTimeSpanLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIRequest CurrentRequest = Manager.CreateRequest("Customer001Token");
            CurrentRequest = Manager.CreateRequest("Customer001Token");

            try
            {
                if (!Limitations.IsSatisfied(new XRequestsPerTimeSpanLimitation(20000000, 1), CurrentRequest))
                    throw new XRequestsPerTimeSpanException();
            }
            catch (Exception Ex)
            {
                if (!(Ex is XRequestsPerTimeSpanException))
                    Assert.Fail();
            }
        }
    }
}
