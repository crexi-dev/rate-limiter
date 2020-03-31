using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Core.Rules;
using Xunit;

namespace RateLimiter.Core.Tests.Tests
{
    public class GeographicMuxRuleTests
    {
        [InlineData("")]
        [InlineData(null)]
        [Theory]
        public void Ctor_ThrowsOnInvalidIdentifier(string identifier)
        {
            Assert.Throws<ArgumentException>(() => { new GeographicMuxRule(identifier, 1, 1, 1); });
        }

        [InlineData("")]
        [InlineData(null)]
        [InlineData("test")]
        [Theory]
        public void AllowsExecution_ThrowsOnInvalidAuthToken(string authToken)
        {
            Assert.Throws<ArgumentException>(() =>
                {
                    new GeographicMuxRule("sourceIdentifier", 1, 1, 1).AllowExecution(authToken);
                });
        }

        public void GetNotAllowedReason_Works(string authToken)
        {
            var actual = new GeographicMuxRule("test", 1, 1, 1);
        }

        [ClassData(typeof(AllowExecutionTestData))]
        [Theory]
        public async Task AllowExecution_HappyPath(int timespanBetweenRequestsMs, int requestsPerTimespanMaxRequestCount, int requestsPerTimespanSeconds, RequestTimeline[] timeline)
        {
            var sourceIdentifier = Guid.NewGuid().ToString();
            var geographicMuxRule = new GeographicMuxRule(sourceIdentifier, timespanBetweenRequestsMs,
                requestsPerTimespanMaxRequestCount, requestsPerTimespanSeconds);

            for (var i = 0; i < timeline.Length; i++)
            {
                var request = timeline[i];
                await Task.Delay(TimeSpan.FromMilliseconds(request.DelayMS));
                var actual = geographicMuxRule.AllowExecution(request.AuthToken);

                Assert.Equal(request.Expected, actual);
            }
        }

        class AllowExecutionTestData : IEnumerable<object[]>
        {
            private const string USAuthToken = "US-AuthToken";
            private const string USAuthToken2 = "US-AuthToken2";
            private const string EUAuthToken = "EU-AuthToken";
            private const string EUAuthToken2 = "EU-AuthToken2";

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    200,
                    5,
                    2,
                    new[]
                    {
                        new RequestTimeline(0, true, USAuthToken),
                        new RequestTimeline(200, true, USAuthToken),
                        new RequestTimeline(200, true, USAuthToken),
                        new RequestTimeline(200, true, USAuthToken),
                        new RequestTimeline(200, true, USAuthToken),
                        new RequestTimeline(200, false, USAuthToken),
                        new RequestTimeline(1100, true, USAuthToken),
                        new RequestTimeline(0, false, USAuthToken),
                        new RequestTimeline(100, true, USAuthToken),
                        new RequestTimeline(400, true, USAuthToken),
                    }
                };

                yield return new object[]
                {
                    200,
                    5,
                    2,
                    new[]
                    {
                        new RequestTimeline(0, true, USAuthToken),
                        new RequestTimeline(200, true, USAuthToken),
                        new RequestTimeline(0, true, USAuthToken2),
                        new RequestTimeline(200, true, USAuthToken),
                        new RequestTimeline(0, true, USAuthToken2),
                        new RequestTimeline(200, true, USAuthToken),
                        new RequestTimeline(0, true, USAuthToken2),
                        new RequestTimeline(200, true, USAuthToken),
                        new RequestTimeline(0, true, USAuthToken2),
                        new RequestTimeline(200, false, USAuthToken),
                        new RequestTimeline(0, true, USAuthToken2),
                        new RequestTimeline(1100, true, USAuthToken),
                        new RequestTimeline(0, false, USAuthToken2),
                        new RequestTimeline(0, false, USAuthToken),
                        new RequestTimeline(100, true, USAuthToken),
                        new RequestTimeline(400, true, USAuthToken),
                        new RequestTimeline(400, true, USAuthToken2),
                    }
                };

                yield return new object[]
                {
                1000,
                5,
                2,
                new[]
                {
                    new RequestTimeline(0, true, EUAuthToken),
                    new RequestTimeline(100, false, EUAuthToken),
                    new RequestTimeline(400, false, EUAuthToken),
                    new RequestTimeline(300, false, EUAuthToken),
                    new RequestTimeline(600, true, EUAuthToken),
                }
                };

                yield return new object[]
                {
                500,
                5,
                2,
                new[]
                {
                    new RequestTimeline(0, true, EUAuthToken),
                    new RequestTimeline(100, false, EUAuthToken),
                    new RequestTimeline(200, false, EUAuthToken),
                    new RequestTimeline(100, false, EUAuthToken),
                    new RequestTimeline(200, true, EUAuthToken),
                }
                };

                yield return new object[]
                {
                100,
                5,
                2,
                new[]
                {
                    new RequestTimeline(0, true, EUAuthToken),
                    new RequestTimeline(50, false, EUAuthToken),
                    new RequestTimeline(10, false, EUAuthToken),
                    new RequestTimeline(10, false, EUAuthToken),
                    new RequestTimeline(40, true, EUAuthToken),
                    new RequestTimeline(50, false, EUAuthToken),
                    new RequestTimeline(60, true, EUAuthToken),
                }
                };

                yield return new object[]
                {
                    1000,
                    5,
                    2,
                    new[]
                    {
                        new RequestTimeline(0, true, EUAuthToken),
                        new RequestTimeline(500, false, EUAuthToken),
                        new RequestTimeline(100, true, EUAuthToken2),
                        new RequestTimeline(100, false, EUAuthToken),
                        new RequestTimeline(100, false, EUAuthToken2),
                        new RequestTimeline(100, false, EUAuthToken),
                        new RequestTimeline(200, true, EUAuthToken),
                        new RequestTimeline(100, false, EUAuthToken2),
                        new RequestTimeline(500, false, EUAuthToken),
                        new RequestTimeline(100, true, EUAuthToken2),
                        new RequestTimeline(600, true, EUAuthToken),
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
