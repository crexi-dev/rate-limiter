using NUnit.Framework;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private RateLimiterConfig _rules;

        // Where there is a host (will populate from the appsettings.json file)
        public RateLimiterTest(RateLimiterConfig rules)
        {
            _rules = rules;
        }


        // When there is NO host (ie unit test project only)
        public RateLimiterTest()
        {
            // Arrange
            _rules = new RateLimiterConfig
            {
                XRequestsPerTimespan = 10,
                TimeSinceLastCall = 10
            };
        }

        [TestCase(10, ExpectedResult = true)]   // positive test
        [TestCase(15, ExpectedResult = false)]  // negative test
        public bool IsTimeSinceLastCallAllowable_ShouldReturnCorrectResult(int timeSinceLastCall)
        {
            // Act
            RateLimiterState state = new RateLimiterState(null);
            var result = RateLimiterRules.JWTIngest(state).IsTimeSinceLastCallAllowable(_rules, timeSinceLastCall);

            // Assert (implict: Assert.AreEqual(ExpectedResult, result);"
            return result.IsAlloweable ?? false;
        }

        [TestCase(10, ExpectedResult = true)]   // positive test
        [TestCase(15, ExpectedResult = false)]  // negative test
        public bool IsXRequestPerTimespanAllowable_ShouldReturnCorrectResult(int noOfRequests)
        {
            // Act
            RateLimiterState state = new RateLimiterState(null);
            var result = RateLimiterRules.JWTIngest(state).IsXRequestPerTimespanAllowable(_rules, noOfRequests);

            // Assert (implicit: Assert.AreEqual(ExpectedResult, result);)
            return result.IsAlloweable ?? false;
        }

        // combination test
        [TestCase(10, 10, ExpectedResult = true)]   // positive test
        [TestCase(10, 15, ExpectedResult = false)]  // negative test
        public bool MultipleRules_SortedA_ShouldReturnCorrectResult(int noOfRequests, int timeSinceLastCall)
        {
            // Act
            RateLimiterState? state = new RateLimiterState(null);
            var result = RateLimiterRules.JWTIngest(state)
                                            .IsTimeSinceLastCallAllowable(_rules, timeSinceLastCall)
                                            .IsXRequestPerTimespanAllowable(_rules, noOfRequests);  

            // Assert (implict: Assert.AreEqual(ExpectedResult, result);"
            return result.IsAlloweable ?? false;
        }

        // combination test (swapped)
        [TestCase(10, 10, ExpectedResult = true)]   // positive test
        [TestCase(10, 15, ExpectedResult = false)]  // negative test
        public bool MultipleRules_SortedB_ShouldReturnCorrectResult(int noOfRequests, int timeSinceLastCall)
        {
            // Act
            RateLimiterState? state = new RateLimiterState(null);
            var result = RateLimiterRules.JWTIngest(state)
                                            .IsXRequestPerTimespanAllowable(_rules, noOfRequests)
                                            .IsTimeSinceLastCallAllowable(_rules, timeSinceLastCall);

            // Assert (implict: Assert.AreEqual(ExpectedResult, result);"
            return result.IsAlloweable ?? false;
        }
    }
}