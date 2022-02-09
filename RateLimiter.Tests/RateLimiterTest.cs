using System;
using System.Linq;
using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [Test]
        public void USConditionCorrectData_Test()
        {
            var mockUsRequests = MockDataGenerator.GetCorrectUsData();
            var api = new Api();

            foreach (var response in mockUsRequests.Select(request => api.GetRequest(request)))
            {
                Assert.AreEqual("Ok", response);
            }
        }
        
        [Test]
        public void USConditionIncorrectData_Test()
        {
            var mockUsRequests = MockDataGenerator.GetIncorrectUsData();
            var api = new Api();

            for (var i = 0; i < mockUsRequests.Count; ++i)
            {
                if (i < 20)
                {
                    api.GetRequest(mockUsRequests[i]);
                }
                else
                {
                    var ex = Assert.Throws<Exception>(() => api.GetRequest(mockUsRequests[i]));
                    Assert.That(ex?.Message, Is.EqualTo("BadRequest"));
                }
            }
        }
        
        [Test]
        public void EUConditionCorrectData_Test()
        {
            var mockUsRequests = MockDataGenerator.GetCorrectEuData();
            var api = new Api();

            foreach (var response in mockUsRequests.Select(request => api.GetRequest(request)))
            {
                Assert.AreEqual("Ok", response);
            }
        }
        
        [Test]
        public void EUConditionIncorrectData_Test()
        {
            var mockUsRequests = MockDataGenerator.GetIncorrectEuData();
            var api = new Api();

            for (var i = 0; i < mockUsRequests.Count; ++i)
            {
                if (i == 0)
                {
                    api.GetRequest(mockUsRequests[i]);
                }
                else
                {
                    var ex = Assert.Throws<Exception>(() => api.GetRequest(mockUsRequests[i]));
                    Assert.That(ex?.Message, Is.EqualTo("BadRequest"));
                }
            }
        }


        [Test]
        public void BothConditionCorrectData_Test()
        {
            var mockUsRequests = MockDataGenerator.GetCorrectBothRuleData();
            var api = new Api();

            foreach (var response in mockUsRequests.Select(request => api.GetRequest(request, true)))
            {
                Assert.AreEqual("Ok", response);
            }
        }
        
        [Test]
        public void BothConditionIncorrectData_Test()
        {
            var mockUsRequests = MockDataGenerator.GetIncorrectBothRuleData();
            var api = new Api();

            for (var i = 0; i < mockUsRequests.Count; ++i)
            {
                if (i < 20)
                {
                    api.GetRequest(mockUsRequests[i]);
                }
                else
                {
                    var ex = Assert.Throws<Exception>(() => api.GetRequest(mockUsRequests[i]));
                    Assert.That(ex?.Message, Is.EqualTo("BadRequest"));
                }
            }
        }
    }
}
