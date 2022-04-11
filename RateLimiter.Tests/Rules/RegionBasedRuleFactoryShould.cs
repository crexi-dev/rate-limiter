using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using Shouldly;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Rules;
using RateLimiter.Common;
using Microsoft.Extensions.Options;

namespace RateLimiter.Tests.Rules
{
    public class RegionBasedRuleFactoryShould
    {
        private RegionBasedRuleFactory regionBasedRuleFactory;

        public RegionBasedRuleFactoryShould()
        {
            var services = new ServiceCollection();

            services.AddScoped<RequestsPerTimespanRule>()
                .AddScoped<IRateLimiterRule, RequestsPerTimespanRule>(s => s.GetService<RequestsPerTimespanRule>());
            services.AddScoped<TimePassedRule>()
                .AddScoped<IRateLimiterRule, TimePassedRule>(s => s.GetService<TimePassedRule>());

            services.AddScoped(x => Substitute.For<IApiClient>());
            services.AddScoped(x => Substitute.For<IRateLimiterRepository>());
            services.AddScoped(x => Substitute.For<IOptions<RequestPerTimeSpanOptions>>());
            services.AddScoped(x => Substitute.For<IOptions<TimePassedRuleOptions>>());

            regionBasedRuleFactory = new RegionBasedRuleFactory(services.BuildServiceProvider());
        }

        [Fact]
        public void Return_RequestsPerTimespanRule_For_US_Based_Client()
        {
            var response = regionBasedRuleFactory.GetRateLimiterRule("US");
            response.ShouldBeOfType<RequestsPerTimespanRule>();
        }


        [Fact]
        public void Return_TimePassedRule_For_EU_Based_Client()
        {
            var response = regionBasedRuleFactory.GetRateLimiterRule("EU");
            response.ShouldBeOfType<TimePassedRule>();
        }

        [Fact]
        public void Throw_Exception_For_Unknown_Region()
        {
            var exception = Assert.Throws<Exception>(() => regionBasedRuleFactory.GetRateLimiterRule("somewhere"));
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe($"Unknown region entered. Region: somewhere");

        }
    }
}
