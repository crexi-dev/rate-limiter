using Microsoft.VisualStudio.TestTools.UnitTesting;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RateLimiter.Test
{
    [TestClass]
    public class UnitTest1
    {
        Manager manager;

        [TestInitialize]
        public void Initialize()
        {
            var clients = new Dictionary<string, Client>
            {
                {
                    "token_0",
                    new Client(
                        ldt => Resource.Create(ldt)
                                       .AddRule(new RequestPerTimeSpanRule(5, TimeSpan.FromSeconds(5)))
                                       .AddRule(new RequestSinceLastCallRule(TimeSpan.FromSeconds(1))))
                },
                {
                    "token_1",
                    new Client(
                        ldt => Resource.Create(ldt)
                                       .AddRule(new RequestPerTimeSpanRule(3, TimeSpan.FromSeconds(3)))
                                       .AddRule(new RequestSinceLastCallRule(TimeSpan.FromMilliseconds(20))))
                }
            };

            manager = new Manager(clients);
        }

        [TestMethod]
        public void ShouldRequestCountBeLimitedToOnePerSecond()
        {
            var result1 = manager.GetEvaluater("token_0").CanGoThrough(DateTimeOffset.Now);
            Assert.IsTrue(result1);

            Thread.Sleep(980);

            var result2 = manager.GetEvaluater("token_0").CanGoThrough(DateTimeOffset.Now);
            Assert.IsFalse(result2);

            Thread.Sleep(980);

            var result3 = manager.GetEvaluater("token_0").CanGoThrough(DateTimeOffset.Now);
            Assert.IsTrue(result3);

            Thread.Sleep(980);

            var result4 = manager.GetEvaluater("token_0").CanGoThrough(DateTimeOffset.Now);
            Assert.IsFalse(result4);
        }

        [TestMethod]
        public void ShouldRequestBeThreePerThreeSecondsTimeSpan()
        {
            var result5 = manager.GetEvaluater("token_1").CanGoThrough(DateTimeOffset.Now);
            Assert.IsTrue(result5);

            Thread.Sleep(980);

            var result6 = manager.GetEvaluater("token_1").CanGoThrough(DateTimeOffset.Now);
            Assert.IsTrue(result6);

            Thread.Sleep(980);

            var result7 = manager.GetEvaluater("token_1").CanGoThrough(DateTimeOffset.Now);
            Assert.IsTrue(result7);

            Thread.Sleep(980);

            var result8 = manager.GetEvaluater("token_1").CanGoThrough(DateTimeOffset.Now);
            Assert.IsFalse(result8);

            Thread.Sleep(980);

            var result9 = manager.GetEvaluater("token_1").CanGoThrough(DateTimeOffset.Now);
            Assert.IsTrue(result9);

            Thread.Sleep(980);

            var result10 = manager.GetEvaluater("token_1").CanGoThrough(DateTimeOffset.Now);
            Assert.IsTrue(result10);

            Thread.Sleep(980);

            var result11 = manager.GetEvaluater("token_1").CanGoThrough(DateTimeOffset.Now);
            Assert.IsTrue(result11);

            Thread.Sleep(980);

            var result12 = manager.GetEvaluater("token_1").CanGoThrough(DateTimeOffset.Now);
            Assert.IsFalse(result12);
        }
    }
}
