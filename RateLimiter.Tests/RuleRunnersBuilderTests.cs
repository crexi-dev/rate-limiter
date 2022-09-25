using FluentAssertions;
using Moq;
using RateLimiter.Builders;
using RateLimiter.Models;
using RateLimiter.RuleRunners;
using RateLimiter.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace RateLimiter.Tests
{
    public class RuleRunnersBuilderTests
    {
        [Fact]
        public void NullRules_Returns_Zero_RuleRunners()
        {
            var mockRequestsPerTimeSpanCountCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            var mockTimeSpanSinceLastCallCacheService = new Mock<ICacheService<TimeSpanSinceLastCall>>();
            

            var builder = new RuleRunnersBuilder(mockRequestsPerTimeSpanCountCacheService.Object, mockTimeSpanSinceLastCallCacheService.Object);

            var runners = builder.GetRuleRunners(null, null);

            runners.Should().BeEmpty();
        }

        [Fact]
        public void Configuring_RegionSpecificRules_WithoutConfiguringRuleTypeItUses_Returns_Zero_RuleRunners()
        {
            var mockRequestsPerTimeSpanCountCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            var mockTimeSpanSinceLastCallCacheService = new Mock<ICacheService<TimeSpanSinceLastCall>>();
            var rateLimitRule = new RateLimitRule
            {
                RegionBasedRules = new List<RegionBasedRule>
                {
                    { new RegionBasedRule{ Region = "US", RuleType= RuleType.RequestsPerTimeSpanRule } },
                    { new RegionBasedRule{ Region = "EU", RuleType= RuleType.TimeSpanSinceLastCallRule } }
                }
            };
            var options = new RateLimitRuleOptions
            {
                Rules = new Dictionary<string, RateLimitRule>
                {
                    { "Orders", rateLimitRule}
                }
            };


            var builder = new RuleRunnersBuilder(mockRequestsPerTimeSpanCountCacheService.Object, mockTimeSpanSinceLastCallCacheService.Object);

            var runners1 = builder.GetRuleRunners(options, new ClientRequest { Region = "US", ClientKey = "Client_1", Resource = "Orders" });
            var runners2 = builder.GetRuleRunners(options, new ClientRequest { Region = "EU", ClientKey = "Client_1", Resource = "Orders" });

            runners1.Should().BeEmpty();
            runners2.Should().BeEmpty();
        }

        [Fact]
        public void Configuring_RegionSpecificRules_Returns_Correct_RuleRunners()
        {
            var mockRequestsPerTimeSpanCountCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            var mockTimeSpanSinceLastCallCacheService = new Mock<ICacheService<TimeSpanSinceLastCall>>();
            var reqPerTimeSpanRule = new RequestsPerTimeSpanRule
            {
                TimeSpan = TimeSpan.FromSeconds(60),
                AllowedNumberOfRequests = 60
            };
            var timeSinceRule = new TimeSpanSinceLastCallRule
            {
                TimeSpan = TimeSpan.FromSeconds(1)
            };
            var rateLimitRule = new RateLimitRule
            {
                RequestsPerTimeSpanRule = reqPerTimeSpanRule,
                TimeSpanSinceLastCallRule = timeSinceRule,
                RegionBasedRules = new List<RegionBasedRule>
                {
                    { new RegionBasedRule{ Region = "US", RuleType= RuleType.RequestsPerTimeSpanRule } },
                    { new RegionBasedRule{ Region = "EU", RuleType= RuleType.TimeSpanSinceLastCallRule } }
                }
            };
            var options = new RateLimitRuleOptions
            {
                Rules = new Dictionary<string, RateLimitRule>
                {
                    { "Orders", rateLimitRule}
                }
            };


            var builder = new RuleRunnersBuilder(mockRequestsPerTimeSpanCountCacheService.Object, mockTimeSpanSinceLastCallCacheService.Object);

            var runners1 = builder.GetRuleRunners(options, new ClientRequest { Region = "US", ClientKey = "Client_1", Resource = "Orders"});
            var runners2 = builder.GetRuleRunners(options, new ClientRequest { Region = "EU", ClientKey = "Client_1", Resource = "Orders" });

            runners1.Should().NotBeEmpty();
            runners1.Should().AllBeOfType<RequestsPerTimeSpanRuleRunner>();
            runners2.Should().NotBeEmpty();
            runners2.Should().AllBeOfType<TimeSpanSinceLastCallRuleRunner>();
        }

        [Fact]
        public void Configuring_Only_RequestsPerTimeSpanRule_Returns_Correct_RuleRunner()
        {
            var mockRequestsPerTimeSpanCountCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            var mockTimeSpanSinceLastCallCacheService = new Mock<ICacheService<TimeSpanSinceLastCall>>();
            var reqPerTimeSpanRule = new RequestsPerTimeSpanRule
            {
                TimeSpan = TimeSpan.FromSeconds(60),
                AllowedNumberOfRequests = 60
            };
            var rateLimitRule = new RateLimitRule
            {
                RequestsPerTimeSpanRule = reqPerTimeSpanRule
            };
            var options = new RateLimitRuleOptions
            {
                Rules = new Dictionary<string, RateLimitRule>
                {
                    { "Orders", rateLimitRule}
                }
            };


            var builder = new RuleRunnersBuilder(mockRequestsPerTimeSpanCountCacheService.Object, mockTimeSpanSinceLastCallCacheService.Object);

            var runners = builder.GetRuleRunners(options, new ClientRequest { Region = "US", ClientKey = "Client_1", Resource = "Orders" });

            runners.Should().NotBeEmpty();
            runners.Should().AllBeOfType<RequestsPerTimeSpanRuleRunner>();
        }

        [Fact]
        public void Configuring_Only_TimeSpanSinceLastCallRule_Returns_Correct_RuleRunner()
        {
            var mockRequestsPerTimeSpanCountCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            var mockTimeSpanSinceLastCallCacheService = new Mock<ICacheService<TimeSpanSinceLastCall>>();
            var timeSinceRule = new TimeSpanSinceLastCallRule
            {
                TimeSpan = TimeSpan.FromSeconds(1)
            };
            var rateLimitRule = new RateLimitRule
            {
                TimeSpanSinceLastCallRule = timeSinceRule
            };
            var options = new RateLimitRuleOptions
            {
                Rules = new Dictionary<string, RateLimitRule>
                {
                    { "Orders", rateLimitRule}
                }
            };


            var builder = new RuleRunnersBuilder(mockRequestsPerTimeSpanCountCacheService.Object, mockTimeSpanSinceLastCallCacheService.Object);

            var runners = builder.GetRuleRunners(options, new ClientRequest { Region = "US", ClientKey = "Client_1", Resource = "Orders" });

            runners.Should().NotBeEmpty();
            runners.Should().AllBeOfType<TimeSpanSinceLastCallRuleRunner>();
        }
    }
}
