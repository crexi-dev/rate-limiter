using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using RateLimiter.Builders;
using RateLimiter.Builders.Configuration;
using RateLimiter.Models;
using RateLimiter.Models.Configuration;
using System.Threading;

namespace RateLimiter.Tests
{
    /// <summary>
    /// Defaults:
    /// - MaxRequests = 5
    /// - TimeSpan = 20 sec
    /// - Region = USA
    /// - IsClientAuthenticated = false
    /// - HasSubscription = false
    /// </summary>
    [TestFixture]
    public class RateLimiterTest
    {
        private const string FirstResource = "Resource1";
        private const string SecondResource = "Resource2";
        private const string USARegion = "USA";
        private const string EURegion = "EU";

        [Test]
        public void ExecuteRateLimiting_When_Resource_Has_Rule_And_Max_Requests_Exceeded_Should_Be_Limited()
        {
            // Arrange
            var configuration = new RateLimiterConfigurationBuilder()
                .AddResourcePoliciesConfiguration(new ResourcePoliciesConfigurationBuilder()
                    .ForResource(FirstResource)
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new RequestsPerTimeSpanRuleConfigurationBuilder().Build())
                        .Build()
                    )
                    .Build()
                )
                .Build();
            
            var context = new RateLimiterContextBuilder()
                .ForResource(FirstResource)
                .Build();

            // Act
            RunRateLimiting(configuration, context, failedRequestNumber: 6);
        }

        [Test]
        public void ExecuteRateLimiting_When_Resource_Has_Rule_And_Max_Requests_Not_Exceeded_Should_Not_Be_Limited()
        {
            // Arrange
            var configuration = new RateLimiterConfigurationBuilder()
                .AddResourcePoliciesConfiguration(new ResourcePoliciesConfigurationBuilder()
                    .ForResource(FirstResource)
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new RequestsPerTimeSpanRuleConfigurationBuilder().Build())
                        .Build()
                    )
                    .Build()
                )
                .Build();

            var context = new RateLimiterContextBuilder()
                .ForResource(FirstResource)
                .Build();

            // Act
            RunRateLimiting(configuration, context, true, maxRequest: 5);
        }

        [Test]
        public void ExecuteRateLimiting_When_Resource_Has_Rule_And_Requests_Are_Rare_Should_Not_Be_Limited()
        {
            // Arrange
            var configuration = new RateLimiterConfigurationBuilder()
                .AddResourcePoliciesConfiguration(new ResourcePoliciesConfigurationBuilder()
                    .ForResource(FirstResource)
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new RequestsPerTimeSpanRuleConfigurationBuilder().Build())
                        .Build()
                    )
                    .Build()
                )
                .Build();

            var context = new RateLimiterContextBuilder()
                .ForResource(FirstResource)
                .Build();

            // Act
            RunRateLimiting(configuration, context, true, sleepTime: 4100);
        }

        [Test]
        public void ExecuteRateLimiting_When_Two_Rules_With_Conditions_For_Resource_And_First_Rule_Match_Should_Be_Limited()
        {
            // Arrange
            var configuration = new RateLimiterConfigurationBuilder()
                .AddResourcePoliciesConfiguration(new ResourcePoliciesConfigurationBuilder()
                    .ForResource(FirstResource)
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new RequestsPerTimeSpanRuleConfigurationBuilder().Build())
                        .WithCondition(new RegionConditionConfigurationBuilder()
                            .ForRegion(USARegion)
                            .Build()
                        )
                        .Build()
                    )
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new TimeSpanSinceLastRequestRuleConfigurationBuilder()
                            .WithTimeSpanInSeconds(1)
                            .Build()
                        )
                        .WithCondition(new RegionConditionConfigurationBuilder()
                            .ForRegion(EURegion)
                            .Build()
                        )
                        .Build()
                    )
                    .Build()
                )
                .Build();

            var context = new RateLimiterContextBuilder()
                .ForResource(FirstResource)
                .InRegion(USARegion)
                .Build();

            // Act
            RunRateLimiting(configuration, context, failedRequestNumber: 6, sleepTime: 1300);
        }

        [Test]
        public void ExecuteRateLimiting_When_Two_Rules_With_Conditions_For_Resource_And_Second_Rule_Match_Should_Be_Limited()
        {
            // Arrange
            var configuration = new RateLimiterConfigurationBuilder()
                .AddResourcePoliciesConfiguration(new ResourcePoliciesConfigurationBuilder()
                    .ForResource(FirstResource)
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new RequestsPerTimeSpanRuleConfigurationBuilder().Build())
                        .WithCondition(new RegionConditionConfigurationBuilder()
                            .ForRegion(USARegion)
                            .Build()
                        )
                        .Build()
                    )
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new TimeSpanSinceLastRequestRuleConfigurationBuilder()
                            .WithTimeSpanInSeconds(2)
                            .Build()
                        )
                        .WithCondition(new RegionConditionConfigurationBuilder()
                            .ForRegion(EURegion)
                            .Build()
                        )
                        .Build()
                    )
                    .Build()
                )
                .Build();

            var context = new RateLimiterContextBuilder()
                .ForResource(FirstResource)
                .InRegion(EURegion)
                .Build();

            // Act
            RunRateLimiting(configuration, context, failedRequestNumber: 2, sleepTime: 1500);
        }

        [Test]
        public void ExecuteRateLimiting_When_Client_Does_Not_Have_Subscription_For_Second_Resource_Should_Be_Limited()
        {
            // Arrange
            var configuration = new RateLimiterConfigurationBuilder()
                .AddResourcePoliciesConfiguration(new ResourcePoliciesConfigurationBuilder()
                    .ForResource(FirstResource)
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new RequestsPerTimeSpanRuleConfigurationBuilder().Build())
                        .WithCondition(new RegionConditionConfigurationBuilder()
                            .ForRegion(USARegion)
                            .Build()
                        )
                        .WithCondition(new IsClientAuthenticatedConditionConfigurationBuilder().Build())
                        .WithCondition(new HasRoleConditionConfigurationBuilder().Build())
                        .Build()
                    )
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new TimeSpanSinceLastSuccessfulRequestRuleConfigurationBuilder()
                            .WithTimeSpanInSeconds(1)
                            .Build()
                        )
                        .WithCondition(new RegionConditionConfigurationBuilder()
                            .ForRegion(EURegion)
                            .Build()
                        )
                        .WithCondition(new IsClientAuthenticatedConditionConfigurationBuilder().Build())
                        .WithCondition(new HasRoleConditionConfigurationBuilder().Build())
                        .Build()
                    )
                    .Build()
                )
                .AddResourcePoliciesConfiguration(new ResourcePoliciesConfigurationBuilder()
                    .ForResource(SecondResource)
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new RequestsPerTimeSpanRuleConfigurationBuilder()
                            .WithMaxRequests(3)
                            .Build()
                        )
                        .WithCondition(new HasSubscriptionConditionConfigurationBuilder()
                            .WithSubscription(false)
                            .Build()
                        )
                        .Build()
                    )
                    .AddPolicy(new PolicyConfigurationBuilder()
                        .WithRule(new RequestsPerTimeSpanRuleConfigurationBuilder()
                            .WithMaxRequests(100)
                            .Build()
                        )
                        .WithCondition(new HasSubscriptionConditionConfigurationBuilder().Build())
                        .Build()
                    )
                    .Build()
                )
                .Build();

            var context = new RateLimiterContextBuilder()
                .ForResource(SecondResource)
                .InRegion(EURegion)
                .ClientAuthenticated()
                .WithRole("UserRole")
                //.ClientHasSubscription()
                .Build();

            // Act
            RunRateLimiting(configuration, context, false, failedRequestNumber: 4, sleepTime: 1100);
        }

        private void RunRateLimiting(RateLimiterConfiguration configuration, 
            RateLimiterContext context, 
            bool isPositive = false, 
            int? failedRequestNumber = null, 
            int maxRequest = 10, 
            int sleepTime = 1000)
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var rateLimiter = new RateLimiter(configuration, cache);

            for (var requestNumber = 1; requestNumber <= maxRequest; requestNumber++)
            {
                var result = rateLimiter.Execute(context);
                if (result.IsRateLimited == true && (!failedRequestNumber.HasValue || requestNumber == failedRequestNumber))
                {
                    if (isPositive == false)
                    {
                        Assert.Pass();
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }

                Thread.Sleep(sleepTime);
            }

            if (isPositive == true)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
