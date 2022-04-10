using Microsoft.Extensions.Options;
using NSubstitute;
using RateLimiter.Common;
using RateLimiter.Rules;
using Shouldly;
using System;
using Xunit;

namespace RateLimiter.Tests.Rules
{
    public class TimePassedRuleShould
    {
        private IApiClient apiClient;
        private IOptions<TimePassedRuleOptions> options;
        private IRateLimiterRepository rateLimiterRepository;
        private TimePassedRule certainTimePassedRule;

        public TimePassedRuleShould()
        {
            apiClient = Substitute.For<IApiClient>();
            options = Substitute.For<IOptions<TimePassedRuleOptions>>();
            rateLimiterRepository = Substitute.For<IRateLimiterRepository>();

            certainTimePassedRule = new TimePassedRule(apiClient, rateLimiterRepository, options);
        }

        [Fact]
        public void Return_True_On_First_Time_Login()
        {
            apiClient.ClientId.Returns(new Guid());
            rateLimiterRepository.GetLastLoginDateTime(Arg.Any<Guid>())
                .Returns((DateTime?)null);

            var isValid = certainTimePassedRule.IsValid();
            isValid.ShouldBeTrue();
        }


        [Fact]
        public void Return_False_If_Login_Is_Out_Of_Minimum_TimeSpan()
        {
            apiClient.ClientId.Returns(new Guid());
            rateLimiterRepository.GetLastLoginDateTime(Arg.Any<Guid>())
                .Returns((DateTime?)DateTime.Now.AddDays(-1).AddHours(-1));


            options.Value.Returns(new TimePassedRuleOptions
            {
                MinTimespan = new TimeSpan(TimeSpan.TicksPerDay) // Request should be within 1 day.
            });

            var isValid = certainTimePassedRule.IsValid();
            isValid.ShouldBeFalse();
        }

        [Fact]
        public void Return_False_If_Login_Is_Within_Minimum_TimeSpan()
        {
            apiClient.ClientId.Returns(new Guid());
            rateLimiterRepository.GetLastLoginDateTime(Arg.Any<Guid>())
                .Returns((DateTime?)DateTime.Now.AddHours(-5));


            options.Value.Returns(new TimePassedRuleOptions
            {
                MinTimespan = new TimeSpan(TimeSpan.TicksPerDay) // Request should be within 1 day.
            });

            var isValid = certainTimePassedRule.IsValid();
            isValid.ShouldBeTrue();
        }

    }
}
