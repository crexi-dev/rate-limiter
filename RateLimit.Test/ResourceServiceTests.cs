using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Application.AccessRestriction.Rule.RateLimit;
using RateLimiter.Application.Exception;
using RateLimiter.Application.Services;
using RateLimiter.Data.Repository;
using RateLimiter.Domain.Entities;
using RateLimiter.ViewModels;
using Xunit;

namespace RateLimiter.Test
{
    [ExcludeFromCodeCoverage]
    public class ResourceServiceTests
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly IResourceRepository _resourceRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IResourceService _resourceService;

        private readonly UserToken _userToken = new() { UserId = 5 };

        public ResourceServiceTests(IRuleRepository ruleRepository, IResourceRepository resourceRepository, IServiceProvider serviceProvider, IResourceService resourceService)
        {
            _ruleRepository = ruleRepository;
            _resourceRepository = resourceRepository;
            _serviceProvider = serviceProvider;
            _resourceService = resourceService;
        }


        [Fact(DisplayName = "Attempt to throw a non-existent resource, throws ApplicationException.")]
        public void ResourceA_PerMinuteRule_NonExistentResource()
        {
            //Arrange
            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 5;

            _ruleRepository.Add<ResourceA>(perMinuteRule5);

            //Act/Assert (going over limit throws exception)
            _resourceService.Invoking(r => r.Get<ResourceA, ResourceAReadViewModel>(-1, _userToken))
                .Should().Throw<ApplicationException>().WithMessage("No resource found for id -1");
        }


        [Fact(DisplayName = "Get an existing resource fewer times in a minute than the rate limit.")]
        public void ResourceA_PerMinuteRule_Get3_PassUnderLimit()
        {
            //Arrange
            _resourceRepository.Add<ResourceA>(new ResourceA() { Id = 1, ResourceAProperty1 = "Foo", Updated = DateTime.Parse("2010-05-01") });

            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 5;

            _ruleRepository.Add<ResourceA>(perMinuteRule5);

            //Act
            DateTime beforeTest = DateTime.Now;

            _resourceService.Get<ResourceA, ResourceAReadViewModel>(1, _userToken);
            _resourceService.Get<ResourceA, ResourceAReadViewModel>(1, _userToken);
            ResourceAReadViewModel model = _resourceService.Get<ResourceA, ResourceAReadViewModel>(1, _userToken);

            //Assert
            model.Should().NotBeNull();
            model.Id.Should().Be(1);
            model.ResourceAProperty1.Should().Be("Foo");
            model.Updated.Should().Be(DateTime.Parse("2010-05-01"));
            model.Retrieved.Should().BeAfter(beforeTest);
        }

        [Fact(DisplayName = "Get an existing resource at the rate limit.")]
        public void ResourceA_PerMinuteRule_Get5_PassAtLimit()
        {
            //Arrange
            _resourceRepository.Add<ResourceA>(new ResourceA() { Id = 1, ResourceAProperty1 = "Foo", Updated = DateTime.Parse("2010-05-01") });

            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 5;

            _ruleRepository.Add<ResourceA>(perMinuteRule5);

            //Act
            for (int i = 0; i < 4; i++)
            {
                _resourceService.Get<ResourceA, ResourceAReadViewModel>(1, _userToken);
            }

            ResourceAReadViewModel model = _resourceService.Get<ResourceA, ResourceAReadViewModel>(1, _userToken);

            //Assert
            model.Should().NotBeNull();
            model.Id.Should().Be(1);
        }

        [Fact(DisplayName = "Get an existing resource more times than the rate limit, throws a RateLimitException.")]
        public void ResourceA_5PerMinuteRule_Get6_FailOverLimit()
        {
            //Arrange
            _resourceRepository.Add<ResourceA>(new ResourceA() { Id = 1, ResourceAProperty1 = "Foo", Updated = DateTime.Parse("2010-05-01") });

            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 5;

            _ruleRepository.Add<ResourceA>(perMinuteRule5);

            //Act
            for (int i = 0; i < 5; i++)
            {
                _resourceService.Get<ResourceA, ResourceAReadViewModel>(1, _userToken);
            }

            //Act/Assert (going over limit throws exception)
            _resourceService.Invoking(r => r.Get<ResourceA, ResourceAReadViewModel>(1, _userToken)).Should().Throw<RateLimitException>();
        }


        [Fact(DisplayName = "Get an existing resource fewer times in a minute than the rate limit.")]
        public void ResourceB_PerMinuteRule_Get3_PassUnderLimit()
        {
            //Arrange
            _resourceRepository.Add<ResourceB>(new ResourceB() { Id = 1, ResourceBProperty1 = "Foo", ResourceBProperty2 = "Bar", Updated = DateTime.Parse("2010-05-01") });

            PerMinuteRule perMinuteRule5 = (PerMinuteRule)_serviceProvider.GetService(typeof(IPerMinuteRule))!;
            perMinuteRule5.AccessPerMinute = 5;

            _ruleRepository.Add<ResourceB>(perMinuteRule5);

            //Act
            DateTime beforeTest = DateTime.Now;

            _resourceService.Get<ResourceB, ResourceBReadViewModel>(1, _userToken);
            _resourceService.Get<ResourceB, ResourceBReadViewModel>(1, _userToken);
            ResourceBReadViewModel model = _resourceService.Get<ResourceB, ResourceBReadViewModel>(1, _userToken);

            //Assert
            model.Should().NotBeNull();
            model.Id.Should().Be(1);
            model.ResourceBProperty1.Should().Be("Foo");
            model.ResourceBProperty2.Should().Be("Bar");
            model.Updated.Should().Be(DateTime.Parse("2010-05-01"));
            model.Retrieved.Should().BeAfter(beforeTest);
        }

    }
}