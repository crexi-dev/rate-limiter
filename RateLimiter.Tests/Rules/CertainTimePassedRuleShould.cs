using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using Shouldly;
using RateLimiter.Common;
using Microsoft.Extensions.Options;
using RateLimiter.Rules;

namespace RateLimiter.Tests.Rules
{
    public class CertainTimePassedRuleShould
    {
        private IApiClient apiClient;
        private IOptions<RulesOptions> options;
        private IRateLimiterRepository rateLimiterRepository;
        private CertainTimePassedRule certainTimePassedRule;

        public CertainTimePassedRuleShould()
        {
            apiClient = Substitute.For<IApiClient>();
            options = Substitute.For<IOptions<RulesOptions>>();
            rateLimiterRepository = Substitute.For<IRateLimiterRepository>();

            certainTimePassedRule = new CertainTimePassedRule(apiClient, rateLimiterRepository, options);
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


            options.Value.Returns(new RulesOptions
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


            options.Value.Returns(new RulesOptions
            {
                MinTimespan = new TimeSpan(TimeSpan.TicksPerDay) // Request should be within 1 day.
            });

            var isValid = certainTimePassedRule.IsValid();
            isValid.ShouldBeTrue();
        }

    }
}
