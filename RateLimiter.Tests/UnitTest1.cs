//using System;
//using RateLimiter;
//using RateLimiter.RulesEngine;
//using RateLimiter.Repository;

//namespace RateLimiter.Tests
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        [TestMethod]
//        public void Verify_True_RequestsWithinTimespanRuleWithinRatelimit()
//        {
//            // arrange
//            var resource = new APIResource();
//            var clientToken = "abc123";
//            var requestDate = new DateTime(2020, 1, 1, 0, 0, 0, 500);   // 1/1/2020 12:00:05AM
//            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);   // 1/1/2020 12:00:00AM
//            // var rateLimiter = new RateLimiter();
//            var rule = new RegionRule(1, "US rule", RateLimitType.RequestsPerTimespan, RateLimitLevel.Default);

//            // act
//            // var isAllowed = rateLimiter.VerifyTokenBucketFunctional(5, 5, 1, 60, requestDate, lastUpdateDate, 1);

//            // assert
//            // Assert.AreEqual(isAllowed, true);
//        }

//        [TestMethod]
//        public void VerifyTokenBucketFunctional_False_ExceedRatelimit()
//        {
//            // arrange
//            var clientToken = "abc123";
//            var requestDate = new DateTime(2020, 1, 1, 0, 0, 0, 500);   // 1/1/2020 12:00:05AM
//            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);   // 1/1/2020 12:00:00AM
//            // var rateLimiter = new RateLimitVerifier();

//            // act
//            // var isAllowed = rateLimiter.VerifyTokenBucketFunctional(5, 5, 1, 60, requestDate, lastUpdateDate, 6);

//            // assert
//            // Assert.AreEqual(isAllowed, false);
//        }

//        [TestMethod]
//        public void VerifyTimespanRuleFunctional_True_Withinlimit()
//        {
//            // arrange
//            var clientToken = "abc123";
//            var requestDate = new DateTime(2020, 1, 1, 0, 0, 0, 500);   // 1/1/2020 12:00:05AM
//            var lastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);   // 1/1/2020 12:00:00AM
//            var timeSpanLimit = new TimeSpan(0, 1, 0);  // 1 minute
//            var serverIP = "129.384.6.39";
//            // var rateLimiter = new RateLimitVerifier();

//            // var command = new RateLimiterClientCommand() { Token = clientToken, ServerIP = serverIP, ResourceId = resourceId, RequestDate = DateTime.Now };

//            // act
//            // var isAllowed = rateLimiter.VerifyTimespanRuleFunctional(requestDate, timeSpanLimit, lastUpdateDate);

//            // assert
//            // Assert.IsTrue(isAllowed, "true");
//        }

//        [TestMethod]
//        public void Verify_True_RequestsPerTimespanRuleAndWithinRatelimit()
//        {
//            // arrange
//            var clientToken = "abc123";
//            var serverIP = "129.384.6.39";
//            var clientRepository = new ClientRepository();
//            var rulesEngineProxy = new RulesEngineProxy();
//            var rulesEngine = new RulesEngineClient(rulesEngineProxy);
//            var rateLimiter = new RateLimiter(clientRepository, rulesEngine);
//            // var rateLimitParams = new TokenBucketParams();

//            // rateLimitParams.BucketCount = 5;
//            // rateLimitParams.MaxAmount = 5;
//            // rateLimitParams.RefillAmount = 1;
//            // rateLimitParams.RefillTime = 60;
//            // rateLimitParams.RequestDate = new DateTime(2020, 1, 1, 0, 0, 0);   // 1/1/2020 12:00:05AM
//            // rateLimitParams.LastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 1);   // 1/1/2020 12:00:05AM;

//            // Func<TokenBucketParams, bool> tokenBucketVerifier = (rateLimitParams) => {
//            //     // refill
//            //     var refillCount = (int) Math.Floor((double)(rateLimitParams.RequestDate - rateLimitParams.LastUpdateDate).TotalSeconds / rateLimitParams.RefillTime);

//            //     var count = Math.Min(rateLimitParams.MaxAmount, rateLimitParams.BucketCount + refillCount * rateLimitParams.RefillAmount);

//            //     // check if tokens > count
//            //     if (count < rateLimitParams.TokenCount)
//            //         return false;
                
//            //     return true;
//            // };

//            // // act
//            // var isAllowed = rateLimiter.Verify(rateLimitParams, Rules.US);

//            // // assert
//            // Assert.AreEqual(isAllowed, true);
//        }

//        [TestMethod]
//        public void Verify_True_TimespanPassedSinceLastCallRuleAndWithinRatelimit()
//        {
//            var clientToken = "abc123";
//            var requestDate = new DateTime(2020, 1, 1, 0, 0, 10);   // 10 seconds
//            var serverIP = "129.384.6.39";
//            var clientRepository = new ClientRepository();
//            var rulesEngineProxy = new RulesEngineProxy();
//            var rulesEngine = new RulesEngineClient(rulesEngineProxy);
//            var rateLimiter = new RateLimiter(clientRepository, rulesEngine);
//            // var rateLimitParams = new TimespanPassedSinceLastRequestParams();

//            // rateLimitParams.LastUpdateDate = new DateTime(2020, 1, 1, 0, 0, 10);
//            // rateLimitParams.RequestDate = new DateTime(2020, 1, 1, 0, 0, 10);
//            // rateLimitParams.TimespanLimit = new TimeSpan(0, 1, 0);

//            // Func<TimespanPassedSinceLastRequestParams, bool> timespanPassedSinceLastRequestParamsVerifier = (rateLimitParams) => {
//            //     if (rateLimitParams.RequestDate < rateLimitParams.LastUpdateDate.Add(rateLimitParams.TimespanLimit))
//            //         return true;

//            //     return false;
//            // };

//            // var isAllowed = rateLimiter.Verify(rateLimitParams, Rules.US);

//            // Assert.IsTrue(isAllowed, "false");
//        }

//        [TestMethod]
//        public void Verify_True_CombinationRuleAndWithinRatelimit()
//        {
//            var clientToken = "abc123";
//            var requestDate = new DateTime(2020, 1, 1, 12, 0, 0);
//            var serverIP = "129.384.6.39";
//            var clientRepository = new ClientRepository();
//            var rulesEngineProxy = new RulesEngineProxy();
//            var rulesEngine = new RulesEngineClient(rulesEngineProxy);
//            var rateLimiter = new RateLimiter(clientRepository, rulesEngine);
//            // var command = new RateLimiterClientCommand() { Token = clientToken, ServerIP = serverIP, ResourceId = resourceId, RequestDate = DateTime.Now };

//            // rateLimiter.Verify(clientToken, requestDate);

//            Assert.IsTrue(true, "true");
//        }

//        // [TestMethod]
//        // public void RateLimiter_Verify_Should_Return_False_When_Request_Exceeds_Max_Requests_Per_Timespan()
//        // {
//        //     var clientToken = "abc123";
//        //     var rateLimiter = new RateLimiter();

//        //     var isAllowed = rateLimiter.Verify(clientToken, Rules.RequestsPerTimespan);

//        //     Assert.IsTrue(isAllowed, "false");
//        // }
//    }
//}