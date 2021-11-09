using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using RateLimiter.Data.Repository;
using RateLimiter.Domain.Entities;
using Xunit;

namespace RateLimiter.Test
{
    [ExcludeFromCodeCoverage]
    public class ResourceAccessRepositoryTests
    {
        private readonly IResourceAccessRepository _resourceAccessRepository;

        public ResourceAccessRepositoryTests(IResourceAccessRepository resourceAccessRepository)
        {
            _resourceAccessRepository = resourceAccessRepository;
        }

        [Fact(DisplayName = "Access a resource from 2 different users, verify that selecting only for the first user returns their accesses.")]
        public void AddGet_1_Result()
        {
            //Arrange
            const int userId = 123;
            const int userId2 = 456;

            //Act
            _resourceAccessRepository.Add<ResourceA>(1, userId);
            _resourceAccessRepository.Add<ResourceA>(1, userId2);

            List<IResourceAccess> ra = _resourceAccessRepository.Get<ResourceA>(1, userId);

            //Assert
            ra.Count.Should().Be(1);
            ra[0].UserId.Should().Be(userId);
        }

        [Fact(DisplayName = "Access the same resource multiple times, verify count by resource type, resource id and userid")]
        public void AddGet_3_Result()
        {
            //Arrange
            const int userId = 123;

            //Act
            _resourceAccessRepository.Add<ResourceA>(1, userId);
            _resourceAccessRepository.Add<ResourceA>(1, userId);
            _resourceAccessRepository.Add<ResourceA>(1, userId);
            _resourceAccessRepository.Add<ResourceA>(2, userId); //Different instance of resource, shouldn't be returned

            List<IResourceAccess> ra = _resourceAccessRepository.Get<ResourceA>(1, userId);

            //Assert
            ra.Count.Should().Be(3);
            ra[0].Id.Should().Be(1);
            ra[0].UserId.Should().Be(userId);
            ra[1].Id.Should().Be(1);
            ra[1].UserId.Should().Be(userId);
            ra[2].Id.Should().Be(1);
            ra[2].UserId.Should().Be(userId);
        }
    }
}