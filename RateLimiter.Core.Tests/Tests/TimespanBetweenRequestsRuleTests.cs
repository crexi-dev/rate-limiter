using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Core.Rules;
using Xunit;

namespace RateLimiter.Core.Tests.Tests
{
    public class TimespanBetweenRequestsRuleTests
    {
        [InlineData("")]
        [InlineData(null)]
        [Theory]
        public void Ctor_ThrowsOnInvalidIdentifier(string identifier)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new TimespanBetweenRequestsRule(identifier, 1);
            });
        }

        [InlineData("")]
        [InlineData(null)]
        [Theory]
        public void AllowsExecution_ThrowsOnInvalidAuthToken(string authToken)
        {
            Assert.Throws<ArgumentException>(() =>
                {
                    new TimespanBetweenRequestsRule("test", 1).AllowExecution(authToken);
                });
        }

        [ClassData(typeof(AllowExecutionTestData))]
        [Theory]
        public async Task AllowExecution_HappyPath(int ms, RequestTimeline[] timeline)
        {
            var sourceIdentifier = Guid.NewGuid().ToString();
            var timespanBetweenRequestsRule = new TimespanBetweenRequestsRule(sourceIdentifier, ms);
            
            foreach (var request in timeline)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(request.DelayMS));
                var actual = timespanBetweenRequestsRule.AllowExecution(request.AuthToken);

                Assert.Equal(request.Expected, actual);
            }
        }

        class AllowExecutionTestData : IEnumerable<object[]>
        {
            private const string AuthToken = "AuthToken";

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                1000,
                new[]
                {
                    new RequestTimeline(0, true, AuthToken),
                    new RequestTimeline(100, false, AuthToken),
                    new RequestTimeline(400, false, AuthToken),
                    new RequestTimeline(300, false, AuthToken),
                    new RequestTimeline(600, true, AuthToken),
                }
                };

                yield return new object[]
                {
                500,
                new[]
                {
                    new RequestTimeline(0, true, AuthToken),
                    new RequestTimeline(100, false, AuthToken),
                    new RequestTimeline(200, false, AuthToken),
                    new RequestTimeline(100, false, AuthToken),
                    new RequestTimeline(200, true, AuthToken),
                }
                };

                yield return new object[]
                {
                100,
                new[]
                {
                    new RequestTimeline(0, true, AuthToken),
                    new RequestTimeline(50, false, AuthToken),
                    new RequestTimeline(10, false, AuthToken),
                    new RequestTimeline(10, false, AuthToken),
                    new RequestTimeline(40, true, AuthToken),
                    new RequestTimeline(50, false, AuthToken),
                    new RequestTimeline(60, true, AuthToken),
                }
                };

                yield return new object[]
                {
                    1000,
                    new[]
                    {
                        new RequestTimeline(0, true, AuthToken),
                        new RequestTimeline(500, false, AuthToken),
                        new RequestTimeline(100, true, "AuthToken2"),
                        new RequestTimeline(100, false, AuthToken),
                        new RequestTimeline(100, false, "AuthToken2"),
                        new RequestTimeline(100, false, AuthToken),
                        new RequestTimeline(200, true, AuthToken),
                        new RequestTimeline(100, false, "AuthToken2"),
                        new RequestTimeline(500, false, AuthToken),
                        new RequestTimeline(100, true, "AuthToken2"),
                        new RequestTimeline(600, true, AuthToken),
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
