using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Domain;
using RateLimiter.Storage;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Tests.Storage
{
    [TestFixture]
    public sealed class UserRequestRepositoryTests
    {
        private IUserRequestRepository underTest;

        private ConcurrentDictionary<string, List<UserRequest>> initialStorage;

        [SetUp]
        public void SetUp()
        {
            initialStorage = new ConcurrentDictionary<string, List<UserRequest>>
            {
                ["defaultToken"] = new List<UserRequest>
                {
                    new UserRequest(),
                    new UserRequest(),
                    new UserRequest()
                }
            };

            underTest = new UserRequestRepository(initialStorage);
        }

        [Test]
        public async Task AddOrUpdateAsync_When_UserRequests_Are_Already_Exist_Should_Add_To_Existing()
        {
            // arrange
            var userRequest = new UserRequest
            {
                AccessToken = "defaultToken",
                ResourceName = "test"
            };

            // act
            await underTest.AddOrUpdateAsync(userRequest);

            // assert
            var userRequests = initialStorage["defaultToken"];
            userRequests.Should().NotBeNull();
            userRequests[3].Should().BeEquivalentTo(userRequest);
        }

        [Test]
        public async Task AddOrUpdateAsync_When_UserRequests_Not_Exist_Should_Create_New()
        {
            // arrange
            var userRequest = new UserRequest
            {
                AccessToken = "defaultToken2",
                ResourceName = "test"
            };
            var expected = new ConcurrentDictionary<string, List<UserRequest>>
            {
                ["defaultToken"] = initialStorage["defaultToken"],
                ["defaultToken2"] = new List<UserRequest> { userRequest }
            };

            // act
            await underTest.AddOrUpdateAsync(userRequest);

            // assert
            initialStorage.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetAllAsync_When_AccessToken_Not_Exists_In_Storage_Should_Return_Empty_Enumerable()
        {
            // arrange
            // act
            var result = await underTest.GetAllAsync("nonExistingToken");

            // assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllAsync_When_AccessToken_Exists_In_Storage_Should_Return_Expected()
        {
            // arrange
            // act
            var result = await underTest.GetAllAsync("defaultToken");

            // assert
            result.Should().BeEquivalentTo(initialStorage["defaultToken"]);
        }
    }
}