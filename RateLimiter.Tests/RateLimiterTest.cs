using NUnit.Framework;
using System;
using System.Collections.Concurrent;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void TestMaxRequestPerTime()
        {
            var window = new SlidingWindow(1000, 2);
            window.requests.Enqueue(DateTime.Now.Ticks);
            window.requests.Enqueue(DateTime.Now.AddMilliseconds(2000).Ticks);
            Assert.IsFalse(window.AllowRequest());
        }

        [Test]
        public void TestPassedSinceLastcall()
        {
            var window = new SlidingWindow(null, null, 500);
            window.requests.Enqueue(DateTime.Now.Ticks);
            window.requests.Enqueue(DateTime.Now.AddMilliseconds(2000).Ticks);
            Assert.IsTrue(window.AllowRequest());
        }

        [Test]
        public void TestMaxRequestPerTimeAndPassedSinceLastCall()
        {
            var window = new SlidingWindow(1000, 3, 500);
            window.requests.Enqueue(DateTime.Now.Ticks);
            window.requests.Enqueue(DateTime.Now.AddMilliseconds(2000).Ticks);
            Assert.IsTrue(window.AllowRequest());
        }
    }
}
