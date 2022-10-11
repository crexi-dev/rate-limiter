using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class RateLimiterTest
    {
        private readonly List<User> _users = new()
        {
            new User()
            {
                CountryCode = "EU",
                Token = "1"
            },
            new User()
            {
                CountryCode = "US",
                Token = "2"
            },
            new User()
            {
                CountryCode = "AU",
                Token = "3"
            }
        };

        [Test]
        public void MaxRequestsPerPeriodLimiter()
        {
            //Arrange
            IRateLimiterRule rule = new MaxRequestsPerPeriodRule(5, TimeSpan.FromSeconds(1));
            var rateLimiter = new RateLimiter(new RequestsHistory(), rule);

            var limiterProvider = GetLimiterProvider(r => rateLimiter);

            var requestsCount = 10;
            var expectedAllowedRequestsCount = 24;

            //Act
            var actualAllowedRequestsCount = CallApi(_users, limiterProvider, requestsCount, TimeSpan.FromMilliseconds(150));

            //Assert
            Assert.AreEqual(expectedAllowedRequestsCount, actualAllowedRequestsCount);
        }

        [Test]
        public void MaxRequestsPerPeriodLimiterForUS()
        {
            //Arrange
            IRateLimiterRule rule = new MaxRequestsPerPeriodRule(5, TimeSpan.FromSeconds(1));
            var rateLimiter = new RateLimiter(new RequestsHistory(), rule);

            var limiterProvider = GetLimiterProvider(r =>
            {
                if (r.User.CountryCode == "US") return rateLimiter;
                else return null;
            });

            var requestsCount = 10;
            var expectedAllowedRequestsCount = 28;

            //Act
            var actualAllowedRequestsCount = CallApi(_users, limiterProvider, requestsCount, TimeSpan.FromMilliseconds(150));

            //Assert
            Assert.AreEqual(expectedAllowedRequestsCount, actualAllowedRequestsCount);
        }

        [Test]
        public void MaxRequestsPerPeriodLimiterForLimitedApi()
        {
            //Arrange
            DateTimeOffset[] requestsDateTime = new DateTimeOffset[_users.Count];

            IRateLimiterRule rule = new MaxRequestsPerPeriodRule(5, TimeSpan.FromSeconds(1));
            var rateLimiter = new RateLimiter(new RequestsHistory(), rule);

            var limiterProvider = GetLimiterProvider(r =>
            {
                if (r.Api == "limitedApi") return rateLimiter;
                else return null;
            });

            var requestsCount = 10;
            var expectedAllowedRequestsCount = 28;

            var actualAllowedRequestsCount = 0;

            //Act
            for (int i = 0; i < requestsCount; i++)
            {
                for (int u = 0; u < _users.Count; u++)
                {
                    var request = new Request
                    {
                        User = _users[u],
                        DateTime = requestsDateTime[u],
                        Api = u == 1 ? "limitedApi" : "api"
                    };

                    if (ProcessRequest(limiterProvider, request))
                    {
                        actualAllowedRequestsCount++;
                    }

                    requestsDateTime[u] = requestsDateTime[u].Add(TimeSpan.FromMilliseconds(150));
                }
            }

            //Assert
            Assert.AreEqual(expectedAllowedRequestsCount, actualAllowedRequestsCount);
        }

        [Test]
        public void MultipleLimiterForUSAndMaxRequestsPerPeriodLimiterForEU()
        {
            //Arrange
            IRateLimiterRule maxRequestsPerPeriodRule = new MaxRequestsPerPeriodRule(5, TimeSpan.FromSeconds(1));
            IRateLimiterRule minPeriodRule = new MinPeriodRule(TimeSpan.FromMilliseconds(160));

            var usRateLimiter = new RateLimiter(new RequestsHistory(), new[] { maxRequestsPerPeriodRule, minPeriodRule });
            var euRateLimiter = new RateLimiter(new RequestsHistory(), maxRequestsPerPeriodRule);

            var limiterProvider = GetLimiterProvider(r =>
            {
                return (r.User.CountryCode) switch
                {
                    "US" => usRateLimiter,
                    "EU" => euRateLimiter,
                    _ => null
                };
            });

            var requestsCount = 10;
            var expectedAllowedRequestsCount = 23;

            //Act
            var actualAllowedRequestsCount = CallApi(_users, limiterProvider, requestsCount, TimeSpan.FromMilliseconds(150));

            //Assert
            Assert.AreEqual(expectedAllowedRequestsCount, actualAllowedRequestsCount);
        }


        private static IRateLimiterProvider GetLimiterProvider(Func<Request, RateLimiter> func)
        {
            var mock = new Mock<IRateLimiterProvider>();
            mock.Setup(x => x.GetRateLimiter(It.IsAny<Request>()))
                .Returns(func);

            return mock.Object;
        }

        private static int CallApi(IList<User> users, IRateLimiterProvider limiterProvider, int requestsCount, TimeSpan requestDelay)
        {
            DateTimeOffset[] requestsDateTime = new DateTimeOffset[users.Count];
            var actualAllowedRequestsCount = 0;

            for (int i = 0; i < requestsCount; i++)
            {
                for (int u = 0; u < users.Count; u++)
                {
                    var request = new Request
                    {
                        User = users[u],
                        DateTime = requestsDateTime[u],
                    };

                    if (ProcessRequest(limiterProvider, request))
                    {
                        actualAllowedRequestsCount++;
                    }

                    requestsDateTime[u] = requestsDateTime[u].Add(requestDelay);
                }
            }

            return actualAllowedRequestsCount;
        }

        private static bool ProcessRequest(IRateLimiterProvider limiterProvider, Request request)
        {
            var rateLimiter = limiterProvider.GetRateLimiter(request);

            if (rateLimiter == null) return true;

            return rateLimiter.IsAllowed(request.User.Token, request.DateTime);
        }
    }
}