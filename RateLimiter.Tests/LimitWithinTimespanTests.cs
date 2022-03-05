using Moq;
using NUnit.Framework;
using RateLimiter.Attributes;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests
{
    [TestFixture]
    public class LimitWithinTimespanTests
    {
        [Test]
        public async Task ExecuteLimiterAsync_MustAllowRequest_WhenIsInitialRequest()
        {
            // arrange
            var requestServiceMoq = new Mock<IRateLimiterRequestService>();

            requestServiceMoq.Setup(rs => rs.GetAllowedRequests(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                             .Returns(new List<RateLimiterRequest>());

            var limitWithinTimespanAttribute = new LimitWithinTimespanAttribute() { RequestService = requestServiceMoq.Object, AllowedRequestCount = 1, PerMinutes = 1 };

            // act
            var resp = await limitWithinTimespanAttribute.ExecuteLimiterAsync(new RateLimiterRequest());

            // assert
            Assert.IsTrue(resp.WasAllowed);
        }

        [Test]
        public async Task ExecuteLimiterAsync_MustAllowRequest_WhenLimitNotOverReached()
        {
            // arrange
            var requestServiceMoq = new Mock<IRateLimiterRequestService>();

            requestServiceMoq.Setup(rs => rs.GetAllowedRequests(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                             .Returns(new List<RateLimiterRequest>() {
                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 11)
                                },

                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 13)
                                },

                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 15)
                                },

                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 17)
                                },
                             });

            var limitWithinTimespanAttribute = new LimitWithinTimespanAttribute() { RequestService = requestServiceMoq.Object, AllowedRequestCount = 5, PerMinutes = 1 };

            // act
            var resp = await limitWithinTimespanAttribute.ExecuteLimiterAsync(new RateLimiterRequest());

            // assert
            Assert.IsTrue(resp.WasAllowed);
        }

        [Test]
        public async Task ExecuteLimiterAsync_MustDenyRequest_WhenLimitOverReached()
        {
            // arrange
            var requestServiceMoq = new Mock<IRateLimiterRequestService>();

            requestServiceMoq.Setup(rs => rs.GetAllowedRequests(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                             .Returns(new List<RateLimiterRequest>() {
                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 11)
                                },

                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 13)
                                },

                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 15)
                                },

                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 17)
                                },

                                new RateLimiterRequest
                                {
                                    ActionName = "act",
                                    ControllerName = "contr",
                                    GroupId = "group_id",
                                    RequestDate = new DateTime(year: 2022, month: 2, day: 20, hour: 1, minute: 10, second: 19)
                                },
                             });

            var limitWithinTimespanAttribute = new LimitWithinTimespanAttribute() { RequestService = requestServiceMoq.Object, AllowedRequestCount = 5, PerMinutes = 1 };

            // act
            var resp = await limitWithinTimespanAttribute.ExecuteLimiterAsync(new RateLimiterRequest());

            // assert
            Assert.IsTrue(resp.WasAllowed);
        }
    }
}
