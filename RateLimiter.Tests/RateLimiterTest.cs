using NUnit.Framework;
using NUnit.Framework.Internal;
using RateLimiter.Model;
using System;
using System.Collections.Generic;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private RateLimiterParameters defaultRateLimitParameters = new RateLimiterParameters
        {
            DifferenceBetweenRequests = new TimeSpan(0, 0, 0, 0, 1000),
            LimitRequests = 10,
            LimitsRequestsPerResource = 5,
            TimeLimit = new TimeSpan(0, 0, 0, 10),
            TimeLimitPerResource = new TimeSpan(0, 0, 0, 1),
        };

        [Test]
        public void MakeACallAndItAllowed()
        {
            var rateLimiter = new RateLimiter(defaultRateLimitParameters, null, null);

            var result = rateLimiter.IsResourceAllowed("test1", "testToken1233");
            Assert.IsTrue(result);
        }

        [Test]
        public void MakeACallWithTwoCloserRequests()
        {
            var requestStorage = new List<ResourceRequestModel> {
            new ResourceRequestModel{
            ResourceName="test1",
            AccessToken="testToken1233",
            UtcDateRequest=DateTime.UtcNow.AddSeconds(-1),
            } };

            string testToken = "testToken1233";

            var rateLimiter = new RateLimiter(defaultRateLimitParameters, null, requestStorage);

            var result = rateLimiter.IsResourceAllowed("test1", testToken);
            Assert.IsTrue(result);

            result = rateLimiter.IsResourceAllowed("test1", testToken);
            Assert.IsFalse(result);
        }

        [Test]
        public void MakeSeveralCallsWithDifferentTokens()
        {
            var requestStorage = new List<ResourceRequestModel> {
            new ResourceRequestModel{
            ResourceName="test1",
            AccessToken="testToken1233",
            UtcDateRequest=DateTime.UtcNow.AddSeconds(-1),
            } };

            var rateLimiter = new RateLimiter(defaultRateLimitParameters, null, requestStorage);

            var result = rateLimiter.IsResourceAllowed("test1", "testToken1233");
            Assert.IsTrue(result);
            result = rateLimiter.IsResourceAllowed("test1", "testToken1133");
            Assert.IsTrue(result);
        }

        [Test]
        public void MakeSeveralCallsWithSameTokensButInsidePerResourceLimit()
        {
            var requestStorage = new List<ResourceRequestModel> {
            new ResourceRequestModel{
            ResourceName="test11",
            AccessToken="testToken1233",
            UtcDateRequest=DateTime.UtcNow.AddSeconds(-1),
            } };
            string testToken = "testToken1233";

            var rateLimitParameters = new RateLimiterParameters
            {
                DifferenceBetweenRequests = null,
                LimitRequests = defaultRateLimitParameters.LimitRequests,
                LimitsRequestsPerResource = defaultRateLimitParameters.LimitsRequestsPerResource,
                TimeLimit = defaultRateLimitParameters.TimeLimit,
                TimeLimitPerResource = defaultRateLimitParameters.TimeLimitPerResource
            };

            var rateLimiter = new RateLimiter(rateLimitParameters, null, requestStorage);
            
            for (int i = 0; i < 5; i++)
            {
                var result = rateLimiter.IsResourceAllowed("test1", testToken);
                Assert.IsTrue(result);
            }            
        }

        [Test]
        public void MakeSeveralCallsWithSameTokensAndOutsidePerResourceLimit()
        {
            var requestStorage = new List<ResourceRequestModel> {
            new ResourceRequestModel{
            ResourceName="test11",
            AccessToken="testToken1233",
            UtcDateRequest=DateTime.UtcNow.AddSeconds(-1),
            } };
            string testToken = "testToken1233";

            var rateLimitParameters = new RateLimiterParameters
            {
                DifferenceBetweenRequests = null,
                LimitRequests = defaultRateLimitParameters.LimitRequests,
                LimitsRequestsPerResource = defaultRateLimitParameters.LimitsRequestsPerResource,
                TimeLimit = defaultRateLimitParameters.TimeLimit,
                TimeLimitPerResource = defaultRateLimitParameters.TimeLimitPerResource
            };

            var rateLimiter = new RateLimiter(rateLimitParameters, null, requestStorage);
            
            for (int i = 0; i < 5; i++)
            {
                var rateResult = rateLimiter.IsResourceAllowed("test1", testToken);
                Assert.IsTrue(rateResult);
            }
            var result = rateLimiter.IsResourceAllowed("test1", testToken);
            Assert.IsFalse(result);
        }

        [Test]
        public void MakeSeveralCallsWithSameTokensAndInLimit()
        {
            var requestStorage = new List<ResourceRequestModel> {
            new ResourceRequestModel{
            ResourceName="test11",
            AccessToken="testToken1233",
            UtcDateRequest=DateTime.UtcNow.AddSeconds(-1),
            } };
            string testToken = "testToken1233";

            var rateLimitParameters = new RateLimiterParameters
            {
                DifferenceBetweenRequests = null,
                LimitRequests = defaultRateLimitParameters.LimitRequests,
                LimitsRequestsPerResource = defaultRateLimitParameters.LimitsRequestsPerResource,
                TimeLimit = defaultRateLimitParameters.TimeLimit,
                TimeLimitPerResource = defaultRateLimitParameters.TimeLimitPerResource
            };

            var rateLimiter = new RateLimiter(rateLimitParameters, null, requestStorage);
            
            for (int i = 0; i < 5; i++)
            {
                var rateResult = rateLimiter.IsResourceAllowed("test1", testToken);
                Assert.IsTrue(rateResult);
            }

            for (int i = 0; i < 4; i++)
            {
                var rateResult = rateLimiter.IsResourceAllowed("test2", testToken);
                Assert.IsTrue(rateResult);
            }
        }

        [Test]
        public void MakeSeveralCallsWithSameTokensAndOutOfTheLimit()
        {
            var requestStorage = new List<ResourceRequestModel> {
            new ResourceRequestModel{
            ResourceName="test11",
            AccessToken="testToken1233",
            UtcDateRequest=DateTime.UtcNow.AddSeconds(-1),
            } };
            string testToken = "testToken1233";

            var rateLimitParameters = new RateLimiterParameters
            {
                DifferenceBetweenRequests = null,
                LimitRequests = defaultRateLimitParameters.LimitRequests,
                LimitsRequestsPerResource = defaultRateLimitParameters.LimitsRequestsPerResource,
                TimeLimit = defaultRateLimitParameters.TimeLimit,
                TimeLimitPerResource = defaultRateLimitParameters.TimeLimitPerResource
            };

            var rateLimiter = new RateLimiter(rateLimitParameters, null, requestStorage);
            
            for (int i = 0; i < 5; i++)
            {
                var rateResult = rateLimiter.IsResourceAllowed("test1", testToken);
                Assert.IsTrue(rateResult);
            }

            for (int i = 0; i < 4; i++)
            {
                var rateResult = rateLimiter.IsResourceAllowed("test2", testToken);
                Assert.IsTrue(rateResult);
            }
            var result = rateLimiter.IsResourceAllowed("test3", testToken);
            Assert.IsFalse(result);
        }

        [Test]
        public void MakeCallsWithRuleToResourceAndInPerResourceLimit()
        {
            var resourceRule = new List<ResourceRuleModel>
            {
                new ResourceRuleModel
                {
                    ResourceName="test1",
                    LimitRequests=10,
                    TimeLimit=new TimeSpan(0,0,5)
                }
            };
            var requestStorage = new List<ResourceRequestModel> {
            new ResourceRequestModel{
            ResourceName="test11",
            AccessToken="testToken1233",
            UtcDateRequest=DateTime.UtcNow.AddSeconds(-10),
            } };

            string testToken = "testToken1233";

            var rateLimitParameters = new RateLimiterParameters
            {
                DifferenceBetweenRequests = null,
                LimitRequests = 20,
                LimitsRequestsPerResource = defaultRateLimitParameters.LimitsRequestsPerResource,
                TimeLimit = defaultRateLimitParameters.TimeLimit,
                TimeLimitPerResource = defaultRateLimitParameters.TimeLimitPerResource
            };

            var rateLimiter = new RateLimiter(rateLimitParameters, resourceRule, requestStorage);
            
            for (int i = 0; i < 10; i++)
            {
                var rateResult = rateLimiter.IsResourceAllowed("test1", testToken);
                Assert.IsTrue(rateResult);
            }

            for (int i = 0; i < 5; i++)
            {
                var rateResult = rateLimiter.IsResourceAllowed("test12", testToken);
                Assert.IsTrue(rateResult);
            }
        }

        [Test]
        public void MakeCallsWithRuleToResourceAndOutOfPerResourceLimit()
        {
            var resourceRule = new List<ResourceRuleModel>
            {
                new ResourceRuleModel
                {
                    ResourceName="test1",
                    LimitRequests=10,
                    TimeLimit=new TimeSpan(0,0,5)
                }
            };
            var requestStorage = new List<ResourceRequestModel> {
            new ResourceRequestModel{
            ResourceName="test11",
            AccessToken="testToken1233",
            UtcDateRequest=DateTime.UtcNow.AddSeconds(-15),
            } };

            string testToken = "testToken1233";

            var rateLimitParameters = new RateLimiterParameters
            {
                DifferenceBetweenRequests = null,
                LimitRequests = defaultRateLimitParameters.LimitRequests,
                LimitsRequestsPerResource = defaultRateLimitParameters.LimitsRequestsPerResource,
                TimeLimit = defaultRateLimitParameters.TimeLimit,
                TimeLimitPerResource = defaultRateLimitParameters.TimeLimitPerResource
            };

            var rateLimiter = new RateLimiter(rateLimitParameters, resourceRule, requestStorage);
            
            for (int i = 0; i < 10; i++)
            {
                var rateResult = rateLimiter.IsResourceAllowed("test1", testToken);
                Assert.IsTrue(rateResult);
            }
            var result = rateLimiter.IsResourceAllowed("test1", testToken);
            Assert.IsFalse(result);
        }
    }
}
