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
            APIEndPoint CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);

            Assert.IsTrue(CurrentRequest != null);
        }

        [Test]
        public void Request_Satisfies_TimeSpanSinceLastCallLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIEndPoint CurrentEndPointRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);

            if (!Limitations.IsSatisfied(new TimeSpanSinceLastCallLimitation(1), CurrentEndPointRequest))
                throw new TimeSpanSinceLastCallLimitationException();
        }

        [Test]
        public void SecondRequest_Satisfies_TimeSpanSinceLastCallLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIEndPoint CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);
            Thread.Sleep(1000);
            CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);

            if (!Limitations.IsSatisfied(new TimeSpanSinceLastCallLimitation(1), CurrentRequest))
                throw new TimeSpanSinceLastCallLimitationException();
        }

        [Test]
        public void Request_Satisfies_XRequestsPerTimeSpanLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIEndPoint CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);

            if (!Limitations.IsSatisfied(new XRequestsPerTimeSpanLimitation(20000000, 1), CurrentRequest))
                throw new XRequestsPerTimeSpanException();
        }

        [Test]
        public void SecondRequest_Satisfies_XRequestsPerTimeSpanLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIEndPoint CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);
            CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);

            if (!Limitations.IsSatisfied(new XRequestsPerTimeSpanLimitation(20000000, 2), CurrentRequest))
                throw new XRequestsPerTimeSpanException();
        }

        [Test]
        public void Request_DoesNot_Satisfies_TimeSpanSinceLastCallLimitation()
        {
            RequestManager Manager = new RequestManager();
            APIEndPoint CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);
            CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);

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
            APIEndPoint CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);
            CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);

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

        [Test]
        public void Collection_Of_Limitations_Test()
        {
            RequestManager Manager = new RequestManager();
            APIEndPoint CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);
            Thread.Sleep(1000);
            CurrentRequest = Manager.CreateRequest("Customer001Token", "APIEndPointId001", DateTime.Now);

            System.Collections.Generic.List<IRequestLimitation> Constraints = new System.Collections.Generic.List<IRequestLimitation>();

            XRequestsPerTimeSpanLimitation XRequests = new XRequestsPerTimeSpanLimitation(20000000, 2);
            TimeSpanSinceLastCallLimitation TimeSpanSinceLastCall = new TimeSpanSinceLastCallLimitation(1);

            Constraints.Add(XRequests);
            Constraints.Add(TimeSpanSinceLastCall);

            if (!Limitations.AreSatisfiedAll(Constraints, CurrentRequest))
                Assert.Fail();
        }
    }
}
