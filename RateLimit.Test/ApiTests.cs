using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Application.AccessRestriction.Rule.RateLimit;
using RateLimiter.Application.Exception;
using RateLimiter.Application.Services;
using RateLimiter.Data.Repository;
using RateLimiter.Domain.Entities;
using RateLimiter.Presentation;
using RateLimiter.ViewModels;
using Xunit;

namespace RateLimiter.Test
{
    [ExcludeFromCodeCoverage]
    public class ApiTests
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IResourceRepository _resourceRepository;
        private readonly IResourceService _resourceService;

        public ApiTests(IRuleRepository ruleRepository, IServiceProvider serviceProvider, IResourceRepository resourceRepository, IResourceService resourceService)
        {
            _ruleRepository = ruleRepository;
            _serviceProvider = serviceProvider;
            _resourceRepository = resourceRepository;
            _resourceService = resourceService;
        }

        [Fact(DisplayName = "Get resource A")]
        public void GetResourceA()
        {
            //Arrange
            TimeElapsedRule timeElapsedRule2Minutes = (TimeElapsedRule)_serviceProvider.GetService(typeof(ITimeElapsedRule))!;
            timeElapsedRule2Minutes.MinimumSecondsElapsed = 120;

            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 3;

            _ruleRepository.Add<ResourceA>(perMinuteRule5);

            _resourceRepository.Add<ResourceA>(new ResourceA() { Id = 1, ResourceAProperty1 = "foo", Updated = DateTime.Now });

            UserToken userToken = new UserToken() { UserId = 1 };

            //Act
            Api api = new Api(_resourceService);
            ResourceAReadViewModel vm = api.GetResourceA(1, userToken);

            //Assert
            vm.Id.Should().Be(1);
        }


        [Fact(DisplayName = "Exceed rate limit for ResourceA")]
        public void GetResourceA_TooFast()
        {
            //Arrange
            TimeElapsedRule timeElapsedRule2Minutes = (TimeElapsedRule)_serviceProvider.GetService(typeof(ITimeElapsedRule))!;
            timeElapsedRule2Minutes.MinimumSecondsElapsed = 120;

            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 3;

            _ruleRepository.Add<ResourceA>(perMinuteRule5);

            _resourceRepository.Add<ResourceA>(new ResourceA() { Id = 1, ResourceAProperty1 = "foo", Updated = DateTime.Now });

            UserToken userToken = new UserToken() { UserId = 1 };

            Api api = new Api(_resourceService);
            api.GetResourceA(1, userToken);
            api.GetResourceA(1, userToken);
            api.GetResourceA(1, userToken);

            //Act/Assert (going over limit throws exception)
            api.Invoking(a => a.GetResourceA(1, userToken)).Should().Throw<RateLimitException>();
        }

        [Fact(DisplayName = "Get resource B with a bad id, throws exception.")]
        public void GetNonexistentResourceB()
        {
            //Arrange
            TimeElapsedRule timeElapsedRule2Minutes = (TimeElapsedRule)_serviceProvider.GetService(typeof(ITimeElapsedRule))!;
            timeElapsedRule2Minutes.MinimumSecondsElapsed = 120;

            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 3;

            _ruleRepository.Add<ResourceB>(perMinuteRule5);

            _resourceRepository.Add<ResourceB>(new ResourceB() { Id = 1, ResourceBProperty1 = "foo", Updated = DateTime.Now });

            UserToken userToken = new UserToken() { UserId = 1 };

            Api api = new Api(_resourceService);

            //Act/Assert (bad id throws exception)
            api.Invoking(a => a.GetResourceB(-1, userToken)).Should().Throw<ApplicationException>().WithMessage("No resource found for id -1");
        }
    }
}