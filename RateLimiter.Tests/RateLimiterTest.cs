using Moq;
using NUnit.Framework;
using RateLimiter.Api.Queries;
using RateLimiter.Domain.ApiLimiter;
using RateLimiter.Domain.Resource;
using RateLimiter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private const string US = "US";
        private const string EU = "EU";

        [Test]
        public void SlidingWindowRuleTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new SlidingWindowRule(mockTimer.Object, 1, 20);
            var request = rule.NewVisitAndRuleCheck();
            Assert.IsTrue(request);
            Assert.AreEqual(1, rule.RequestCount);
        }

        [Test]
        public void SlidingWindowRuleLimitExceededTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new SlidingWindowRule(mockTimer.Object, 1, 200);
            var request1 = rule.NewVisitAndRuleCheck();
            var request2 = rule.NewVisitAndRuleCheck();
            Assert.IsTrue(request1);
            Assert.IsFalse(request2);
            Assert.AreEqual(1, rule.RequestCount);
        }

        [Test]
        public void SlidingWindowRuleParrallelTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new SlidingWindowRule(mockTimer.Object, 10, 200);
            
            int goodCount = 0;
            int badCount = 0;

            // Ensure the function is multithreaded-compatible
            Parallel.For(0, 20, (i) =>
            {
                var request = rule.NewVisitAndRuleCheck();
                if (request == true)
                {
                    Interlocked.Increment(ref goodCount);
                }
                else
                {
                    Interlocked.Increment(ref badCount);
                }
            });

            Assert.AreEqual(10, goodCount);
            Assert.AreEqual(10, badCount);
        }

        [Test]
        public void IntervalRuleTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new IntervalRule(mockTimer.Object, 20);
            var request = rule.NewVisitAndRuleCheck();
            Assert.IsTrue(request);
        }

        [Test]
        public void IntervalRuleLimitExceededTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new IntervalRule(mockTimer.Object, 20);
            var request = rule.NewVisitAndRuleCheck();
            Assert.IsTrue(request);

            request = rule.NewVisitAndRuleCheck();
            Assert.IsFalse(request);
        }

        [Test]
        public void IntervalRuleDelayShouldSucceedTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();

            var rule = new IntervalRule(mockTimer.Object, 20);
            var request = rule.NewVisitAndRuleCheck();
            Assert.IsTrue(request);

            // 25 ms later should succeed
            mockTimer.Setup(x => x.GetTimestamp()).Returns(25 * TimeSpan.TicksPerMillisecond);
            request = rule.NewVisitAndRuleCheck();
            Assert.IsTrue(request);
        }

        [Test]
        public void SearchResourceTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();
            Mock<IInMemoryRulesRepository> mockInMemoryRulesRepository = new Mock<IInMemoryRulesRepository>();

            var resourceRules = new ResourceRules()
            {
                ResourceRuleList = new List<ResourceRule>() {
                    new ResourceRule(SearchQuery.RESOURCE, EU, new List<IRule>() { new SlidingWindowRule(mockTimer.Object, 1, 20) })
                }
            };

            mockInMemoryRulesRepository.Setup(x => x.ResourceRules).Returns(resourceRules);

            ApiLimiter apiLimiter = new ApiLimiter(resourceRules);

            var apiEntry = new ApiEntry(mockInMemoryRulesRepository.Object, apiLimiter);
            var response = apiEntry.Search("EUtoken1");
            Assert.AreEqual(ApiEntry.SUCCESS, response);

            // 10 ms later should fail
            mockTimer.Setup(x => x.GetTimestamp()).Returns(10 * TimeSpan.TicksPerMillisecond);
            response = apiEntry.Search("EUtoken1");
            Assert.AreEqual(ApiEntry.FAIL, response);

            // 25 ms later should succeed
            mockTimer.Setup(x => x.GetTimestamp()).Returns(25 * TimeSpan.TicksPerMillisecond);
            response = apiEntry.Search("EUtoken1");
            Assert.AreEqual(ApiEntry.SUCCESS, response);
        }

        [Test]
        public void ApiLimiterParrallelTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();
            Mock<IInMemoryRulesRepository> mockInMemoryRulesRepository = new Mock<IInMemoryRulesRepository>();

            var resourceRules = new ResourceRules()
            {
                ResourceRuleList = new List<ResourceRule>() {
                    new ResourceRule(SearchQuery.RESOURCE, US, new List<IRule>() { new SlidingWindowRule(mockTimer.Object, 15, 2) })
                }
            };

            mockInMemoryRulesRepository.Setup(x => x.ResourceRules).Returns(resourceRules);

            ApiLimiter apiLimiter = new ApiLimiter(resourceRules);

            var apiEntry = new ApiEntry(mockInMemoryRulesRepository.Object, apiLimiter);

            int goodCount = 0;
            int badCount = 0;

            // Ensure the function is multithreaded-compatible
            Parallel.For(0, 30, (i) =>
            {
                var request = apiEntry.Search("UStoken1");
                if (request == ApiEntry.SUCCESS)
                {
                    Interlocked.Increment(ref goodCount);
                }
                else
                {
                    Interlocked.Increment(ref badCount);
                }
            });

            Assert.AreEqual(15, goodCount);
            Assert.AreEqual(15, badCount);
        }

        [Test]
        public void ManyResourceTest()
        {
            Mock<ITimestamp> mockTimer = new Mock<ITimestamp>();
            Mock<IInMemoryRulesRepository> mockInMemoryRulesRepository = new Mock<IInMemoryRulesRepository>();

            var resourceRules = new ResourceRules()
            {
                ResourceRuleList = new List<ResourceRule>() {
                    new ResourceRule(SearchQuery.RESOURCE, US, new List<IRule>() { new SlidingWindowRule(mockTimer.Object, 1, 20) }),
                    new ResourceRule(SearchQuery.RESOURCE, EU, new List<IRule>() { new SlidingWindowRule(mockTimer.Object, 1, 7),
                                                                                    new SlidingWindowRule(mockTimer.Object, 2, 10)}),
                    new ResourceRule(UpdateCommand.RESOURCE, "GLOBAL", new List<IRule>() {new IntervalRule(mockTimer.Object, 7) })
                }
            };

            mockInMemoryRulesRepository.Setup(x => x.ResourceRules).Returns(resourceRules);

            ApiLimiter apiLimiter = new ApiLimiter(resourceRules);
            var apiEntry = new ApiEntry(mockInMemoryRulesRepository.Object, apiLimiter);
            Assert.AreEqual(ApiEntry.SUCCESS, apiEntry.Search("UStoken1"));
            Assert.AreEqual(ApiEntry.SUCCESS, apiEntry.Search("EUtoken1"));
            Assert.AreEqual(ApiEntry.SUCCESS, apiEntry.Update("UStoken1"));

            // 5 ms later
            mockTimer.Setup(x => x.GetTimestamp()).Returns(5 * TimeSpan.TicksPerMillisecond);
            Assert.AreEqual(ApiEntry.FAIL, apiEntry.Search("UStoken1"));
            Assert.AreEqual(ApiEntry.FAIL, apiEntry.Search("EUtoken1"));
            Assert.AreEqual(ApiEntry.FAIL, apiEntry.Update("UStoken1"));

            // 11 ms later
            mockTimer.Setup(x => x.GetTimestamp()).Returns(11 * TimeSpan.TicksPerMillisecond);
            Assert.AreEqual(ApiEntry.FAIL, apiEntry.Search("UStoken1"));
            Assert.AreEqual(ApiEntry.SUCCESS, apiEntry.Search("EUtoken1"));
            Assert.AreEqual(ApiEntry.SUCCESS, apiEntry.Update("UStoken1"));

            // 21 ms later
            mockTimer.Setup(x => x.GetTimestamp()).Returns(21 * TimeSpan.TicksPerMillisecond);
            Assert.AreEqual(ApiEntry.SUCCESS, apiEntry.Search("UStoken1"));
            Assert.AreEqual(ApiEntry.SUCCESS, apiEntry.Search("EUtoken1"));
            Assert.AreEqual(ApiEntry.SUCCESS, apiEntry.Update("UStoken1"));
        }
    }
}
