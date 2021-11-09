using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using RateLimiter.Application.AccessRestriction.Rule;
using RateLimiter.Application.AccessRestriction.Rule.RateLimit;
using RateLimiter.Data.Repository;
using RateLimiter.Domain.Entities;
using Xunit;

namespace RateLimiter.Test
{
    [ExcludeFromCodeCoverage]
    public class RuleRepositoryTests
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly IServiceProvider _serviceProvider;

        public RuleRepositoryTests(IRuleRepository ruleRepository, IServiceProvider serviceProvider)
        {
            _ruleRepository = ruleRepository;
            _serviceProvider = serviceProvider;
        }

        [Fact(DisplayName = "Get rules by resource type")]
        public void GetRules()
        {
            //Arrange
            TimeElapsedRule timeElapsedRule2Minutes = (TimeElapsedRule)_serviceProvider.GetService(typeof(ITimeElapsedRule))!;
            timeElapsedRule2Minutes.MinimumSecondsElapsed = 120;

            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 5;

            _ruleRepository.Add<ResourceA>(perMinuteRule5);
            _ruleRepository.Add<ResourceA>(timeElapsedRule2Minutes);
            _ruleRepository.Add<ResourceB>(timeElapsedRule2Minutes);

            //Act
            IRuleSet rs = _ruleRepository.GetAll<ResourceA>();

            IRuleSet rs2 = _ruleRepository.GetAll<ResourceB>();

            //Assert
            rs.Rules.Count.Should().Be(2);
            rs2.Rules.Count.Should().Be(1);
        }
    }
}