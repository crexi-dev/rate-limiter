using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Core.Rules;
using Xunit;

namespace RateLimiter.Core.Tests.Tests
{
    public class RequestsPerTimespanRuleTests
    {
        [InlineData("")]
        [InlineData(null)]
        [Theory]
        public void Ctor_ThrowsOnInvalidIdentifier(string identifier)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new RequestsPerTimespanRule(identifier, 1, 1);
            });
        }

        [InlineData("")]
        [InlineData(null)]
        [Theory]
        public void AllowsExecution_ThrowsOnInvalidAuthToken(string authToken)
        {
            Assert.Throws<ArgumentException>(() =>
                {
                    new RequestsPerTimespanRule("test", 1, 1).AllowExecution(authToken);
                });
        }

        [ClassData(typeof(AllowExecutionTestData))]
        [Theory]
        public async Task AllowExecution_HappyPath(int maxRequestCount, int seconds, RequestTimeline[] timeline)
        {
            var sourceIdentifier = Guid.NewGuid().ToString();
            var requestsPerTimespanRule = new RequestsPerTimespanRule(sourceIdentifier, maxRequestCount, seconds);

            for (var i = 0; i < timeline.Length; i++)
            {
                var request = timeline[i];
                await Task.Delay(TimeSpan.FromMilliseconds(request.DelayMS));
                var actual = requestsPerTimespanRule.AllowExecution(request.AuthToken);

                Assert.Equal(request.Expected, actual);
            }
        }


        class AllowExecutionTestData : IEnumerable<object[]>
        {
            private const string AuthToken = "AuthToken";
            private const string AuthToken2 = "AuthToken2";

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    5,
                    2,
                    new[]
                    {
                        new RequestTimeline(0, true, AuthToken),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(200, false, AuthToken),
                        new RequestTimeline(1100, true, AuthToken),
                        new RequestTimeline(0, false, AuthToken),
                        new RequestTimeline(100, true, AuthToken),
                        new RequestTimeline(400, true, AuthToken),
                    }
                };

                yield return new object[]
                {
                    5,
                    2,
                    new[]
                    {
                        new RequestTimeline(0, true, AuthToken),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(0, true, AuthToken2),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(0, true, AuthToken2),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(0, true, AuthToken2),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(0, true, AuthToken2),
                        new RequestTimeline(200, false, AuthToken),
                        new RequestTimeline(0, true, AuthToken2),
                        new RequestTimeline(1100, true, AuthToken),
                        new RequestTimeline(0, false, AuthToken2),
                        new RequestTimeline(0, false, AuthToken),
                        new RequestTimeline(100, true, AuthToken),
                        new RequestTimeline(400, true, AuthToken),
                        new RequestTimeline(400, true, AuthToken2),
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
