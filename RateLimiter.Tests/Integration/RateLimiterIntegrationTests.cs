using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using RateLimiter.Enumerators;

namespace RateLimiter.Tests.Integration
{
    [TestFixture]
    public class RateLimiterIntegrationTests
    {
        private User UserFreeUS;
        private User UserFreeEU;
        private User UserBasicSubscription;
        private User UserProSubscription;

        private RateLimiterOptions Options;

        [SetUp]
        public void Setup()
        {
            Options = new RateLimiterOptions();

            //Free Users
            Options.AddPolicy("Free-US", policy => policy
                                                    .SetRateLimits(new RateLimits(1, 20, 300, 1000))
                                                    .RequireClaim("FreeSubscription")
                                                    .RequireLocale("en-US"));

            Options.AddPolicy("Free-EU", policy => policy
                                        .SetRateLimits(new RateLimits()
                                        {
                                            MinimumSpan = TimeSpan.FromSeconds(10)
                                        })
                                        .RequireClaim("FreeSubscription")
                                        .RequireAssertion(context => context.User.Locale == "en-GB" || context.User.Locale == "fr-FR"));

            //Basic Subscription
            Options.AddPolicy("Basic", policy => policy
                                        .SetRateLimits(new RateLimits(5, 100, 1500, 10000))
                                        .RequireClaim("BasicSubscription"));

            //Pro Subscription
            Options.AddPolicy("Pro", policy => policy
                                                    .SetRateLimits(new RateLimits(50, 500, 15000, 100000))
                                                    .RequireClaim("ProSubscription"));

            //Policy to limit requests to long running historical data
            Options.AddPolicy("History", policy => policy
                                                    .SetRateLimits(new RateLimits(1, 20, 300, 1000))
                                                    .RequireAssertion(context => context.RequestPath.StartsWith("/history", StringComparison.OrdinalIgnoreCase)));

            UserFreeUS = new User();
            UserFreeUS.Claims.Add("FreeSubscription");
            UserFreeUS.Locale = "en-US";
            UserFreeUS.Id = new Guid("d6187169-c3cb-471f-ab86-7c11325c23d9");

            UserFreeEU = new User();
            UserFreeEU.Claims.Add("FreeSubscription");
            UserFreeEU.Locale = "en-GB";
            UserFreeEU.Id = new Guid("75ab7849-6495-4a38-bfb6-f4892a4990f6");

            UserBasicSubscription = new User();
            UserBasicSubscription.Claims.Add("BasicSubscription");
            UserBasicSubscription.Id = new Guid("429ce588-2aa6-46b2-9fb4-1d2e23c15d78");

            UserProSubscription = new User();
            UserProSubscription.Claims.Add("BasicSubscription");
            UserProSubscription.Id = new Guid("a69df207-5981-447f-a127-625bbfb7c328");

        }

        private RateLimiterService GetRateLimiterServiceForPolicy(RateLimiterPolicy policy)
        {
            var requirementHandlers = policy.Requirements.Select(x => x as IRateLimiterHandler).ToList();
            IRateLimiterHandlerProvider provider = new DefaultRateLimiterHandlerProvider(requirementHandlers);

            var contextFactory = new RateLimiterDefaultContextFactory();
            var repository = new RateLimiterInMemoryRepository();
            var evaluator = new RateLimiterDefaultEvaluator(repository);

            var service = new RateLimiterService(provider, Substitute.For<ILogger<RateLimiterService>>(), contextFactory, evaluator);
            return service;
        }

        [Test]
        public async Task RateLimiter_AllowsRequest_WhenRequestCountDoesNotExceedRateLimit()
        {
            var rateLimiterResults = new List<RateLimitResult>();

            //Arrange
            foreach (var kvp in Options.Policies)
            {
                var policy = kvp.Value;
                var service = GetRateLimiterServiceForPolicy(policy);

                var requestDate = Substitute.For<IDateTimeWrapper>();

                //the limit for basic subscription users no more than 5 per second

                //Act
                for (int i = 0; i < 5; i++)
                {
                    requestDate.Now.Returns(new DateTime(2022, 09, 22, 5, 5, 5, 5));
                    var result = await service.RateLimitRequestAsync(UserBasicSubscription, "/test", policy, requestDate);
                    rateLimiterResults.Add(result);
                }

            }

            //Assert
            var basicSubscriptionResult = rateLimiterResults.Where(x => x.Policy.Name == "Basic").ToList();
            Assert.IsTrue(basicSubscriptionResult.All(x => x.Type == eRateLimiterResultType.Allow));
        }

        [Test]
        public async Task RateLimiter_HaltsRequest_WhenNumberOfRequestsExceedsRateLimit()
        {
            var rateLimiterResults = new List<RateLimitResult>();

            //Arrange
            foreach (var kvp in Options.Policies)
            {
                var policy = kvp.Value;
                var service = GetRateLimiterServiceForPolicy(policy);

                var requestDate = Substitute.For<IDateTimeWrapper>();

                //the limit for basic subscription users no more than 5 per second, so let's send 6 requests

                //Act
                for (int i = 0; i < 6; i++)
                {
                    requestDate.Now.Returns(new DateTime(2022, 09, 22, 5, 5, 5, 5));
                    var result = await service.RateLimitRequestAsync(UserBasicSubscription, "/test", policy, requestDate);
                    rateLimiterResults.Add(result);
                }
            }

            //Assert
            var basicSubscriptionResult = rateLimiterResults.Where(x => x.Policy.Name == "Basic").ToList();
            Assert.IsTrue(basicSubscriptionResult.Any(x => x.Type == eRateLimiterResultType.Deny));
        }
    }
}
