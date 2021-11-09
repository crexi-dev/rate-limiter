using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using RateLimiter.Data.Repository;
using RateLimiter.Domain.Entities;
using Xunit;

namespace RateLimiter.Test
{
    [ExcludeFromCodeCoverage]
    public class ResourceRepositoryTests
    {
        private readonly IResourceRepository _resourceRepository;

        public ResourceRepositoryTests(IResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;

            LoadTestData();
        }

        [Fact(DisplayName = "Get a known Resource")]
        public void GetResources_ResourceA_1()
        {
            //Act
            IResource res = _resourceRepository.Get<ResourceA>(1);

            //Assert
            res.Should().NotBeNull();
            res.Updated.Should().Be(DateTime.Parse("2010-05-01"));
        }

        [Fact(DisplayName = "Try to get a non-existent Resource")]

        public void GetResources_ResourceB_5_Nonexistent()
        {
            //Act
            IResource res = _resourceRepository.Get<ResourceB>(15);

            //Assert
            res.Should().BeNull();
        }

        private void LoadTestData()
        {
            _resourceRepository.Add<ResourceA>(new ResourceA() { Id = 1, ResourceAProperty1 = "Foo", Updated = DateTime.Parse("2010-05-01") });
            _resourceRepository.Add<ResourceA>(new ResourceA() { Id = 2, ResourceAProperty1 = "Bar", Updated = DateTime.Parse("2020-01-01") });
            _resourceRepository.Add<ResourceA>(new ResourceA() { Id = 3, ResourceAProperty1 = "Baz", Updated = DateTime.Parse("2018-03-01") });
            _resourceRepository.Add<ResourceB>(new ResourceB() { Id = 1, ResourceBProperty1 = "Lorem", ResourceBProperty2 = "Ipsum", Updated = DateTime.Parse("2020-05-01") });
            _resourceRepository.Add<ResourceB>(new ResourceB() { Id = 5, ResourceBProperty1 = "Set", ResourceBProperty2 = "Dolorum", Updated = DateTime.Parse("2020-05-01") });
        }
    }
}