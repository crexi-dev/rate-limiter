using FluentAssertions;
using RuleLimiterTask;
using RuleLimiterTask.Rules;
using RuleLimiterTask.Rules.Settings;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class RateLimiterTests
    {
        private readonly RequestPerTimespanSettings _requestPerTimespanSettings;
        private readonly TimespanSinceLastCallSettings _timespanSinceLastCallSettings;

        public RateLimiterTests()
        {
            _requestPerTimespanSettings = new RequestPerTimespanSettings()
            {
                Count = 2,
                TimespanInMs = 5000
            };

            _timespanSinceLastCallSettings = new TimespanSinceLastCallSettings()
            {
                InMs = 1000
            };
        }

        [Fact]
        public async Task WhenCallsAreTooClose_TimespanSinceLastCallRule_ShouldWork()
        {
            // Arrange
            var cache = new CacheService();
            var request1 = new UserRequest(Region.Other, 1);
            var request2 = new UserRequest(Region.Other, 1);
            var resource = new Resource();
            var rule = new TimespanSinceLastCallRule(_timespanSinceLastCallSettings);
            resource.ApplyRule(rule);

            // Act
            request1.RequestAccess(resource, cache);
            await Task.Delay(_timespanSinceLastCallSettings.InMs - 50); // shorter than in the settings
            request2.RequestAccess(resource, cache);

            // Assert
            request1.State.Should().Be(RequestState.Success);
            request2.State.Should().Be(RequestState.AccessDenied);
        }

        [Fact]
        public async Task WhenCallsAreTooCloseButFromDifferentUsers_TimespanSinceLastCallRule_ShouldNotWork()
        {
            // Arrange
            var cache = new CacheService();
            var request1 = new UserRequest(Region.Other, 1);
            var request2 = new UserRequest(Region.Other, 2);
            var resource = new Resource();
            var rule = new TimespanSinceLastCallRule(_timespanSinceLastCallSettings);
            resource.ApplyRule(rule);

            // Act
            request1.RequestAccess(resource, cache);
            await Task.Delay(_timespanSinceLastCallSettings.InMs - 50); // shorter than in the settings
            request2.RequestAccess(resource, cache);

            // Assert
            request1.State.Should().Be(RequestState.Success);
            request2.State.Should().Be(RequestState.Success);
        }

        [Fact]
        public async Task WhenCallsAreMoreThanMaxLimit_RequestPerTimespanRule_ShouldWork()
        {
            // Arrange
            var cache = new CacheService();
            var request1 = new UserRequest(Region.Other, 1);
            var request2 = new UserRequest(Region.Other, 1);
            var request3 = new UserRequest(Region.Other, 1);
            var request4 = new UserRequest(Region.Other, 1);
            var resource = new Resource();
            var rule = new RequestPerTimespanRule(_requestPerTimespanSettings);
            resource.ApplyRule(rule);

            // Act
            request1.RequestAccess(resource, cache);
            await Task.Delay(1000);
            request2.RequestAccess(resource, cache);
            await Task.Delay(1000);
            request3.RequestAccess(resource, cache);
            await Task.Delay(3000);
            request4.RequestAccess(resource, cache);

            // Assert
            request1.State.Should().Be(RequestState.Success);
            request2.State.Should().Be(RequestState.Success);
            request3.State.Should().Be(RequestState.AccessDenied);
            request2.State.Should().Be(RequestState.Success);
        }

        [Fact]
        public async Task WhenTwoRulesCombinedOnePassesOtherDenies_Request_ShouldBeDenied()
        {
            // Arrange
            var cache = new CacheService();
            var request1 = new UserRequest(Region.Other, 1);
            var request2 = new UserRequest(Region.Other, 1);
            var resource = new Resource();
            var rule1 = new RequestPerTimespanRule(_requestPerTimespanSettings);
            var rule2 = new TimespanSinceLastCallRule(_timespanSinceLastCallSettings);
            resource.ApplyRule(rule1);
            resource.ApplyRule(rule2);

            // Act
            request1.RequestAccess(resource, cache);
            await Task.Delay(100); // for RequestPerTimespanRule second request is ok, but TimespanSinceLastCallRule will block it
            request2.RequestAccess(resource, cache);

            // Assert
            request1.State.Should().Be(RequestState.Success);
            request2.State.Should().Be(RequestState.AccessDenied);
        }

        [Fact]
        public async Task WhenUserIsFromUS_Rule_RequestPerTimespanRuleShouldWork()
        {
            // Arrange
            var cache = new CacheService();

            var request1 = new UserRequest(Region.US, 1);
            var request2 = new UserRequest(Region.US, 1);
            var request3 = new UserRequest(Region.US, 1);
            var request4 = new UserRequest(Region.US, 1);

            var resource = new Resource();

            var rule = new USBasedTokenRule(_requestPerTimespanSettings);
            resource.ApplyRule(rule);

            // Act
            request1.RequestAccess(resource, cache);
            await Task.Delay(1000);
            request2.RequestAccess(resource, cache);
            await Task.Delay(1000);
            request3.RequestAccess(resource, cache);
            await Task.Delay(3000);
            request4.RequestAccess(resource, cache);

            // Assert
            request1.State.Should().Be(RequestState.Success);
            request2.State.Should().Be(RequestState.Success);
            request3.State.Should().Be(RequestState.AccessDenied);
            request2.State.Should().Be(RequestState.Success);
        }
    }
}
