using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using RateLimiter.Cache;
using RateLimiter.Rules;
using RateLimiter.Rules.Settings;
using Xunit;

namespace RateLimiter.Tests
{
    public class RateLimiterTest
    {
        private readonly IConfiguration _configuration;

        public RateLimiterTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);

            _configuration = builder.Build();
        }

        [Fact]
        public async Task WhenCallsAreTooClose_TimespanSinceLastCallRule_ShouldWork()
        {
            // Arrange
            var cache = new CacheService();

            var request1 = new UserRequest(Region.Other, 1);
            var request2 = new UserRequest(Region.Other, 1);

            var endpoint = new Endpoint(_configuration, cache);

            var resource = new Resource("api/other");

            var settings = _configuration.GetSection("TimespanPassedSinceLastCall")
                .Get<TimespanSinceLastCallSettings>();
            var rule = new TimespanSinceLastCallRule(settings);
            resource.ApplyRule(rule);
            endpoint.AddResource(resource);

            // Act
            endpoint.AcceptRequest(request1, "api/other");
            await Task.Delay(settings.InMs - 50); // shorter than in the settings
            endpoint.AcceptRequest(request2, "api/other");

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

            var endpoint = new Endpoint(_configuration, cache);

            var resource = new Resource("api/other");

            var settings = _configuration.GetSection("TimespanPassedSinceLastCall")
                .Get<TimespanSinceLastCallSettings>();
            var rule = new TimespanSinceLastCallRule(settings);
            resource.ApplyRule(rule);
            endpoint.AddResource(resource);

            // Act
            endpoint.AcceptRequest(request1, "api/other");
            await Task.Delay(settings.InMs - 50); // shorter than in the settings
            endpoint.AcceptRequest(request2, "api/other");

            // Assert
            request1.State.Should().Be(RequestState.Success);
            request2.State.Should().Be(RequestState.Success);
        }

        [Fact]
        public async Task WhenCallsAreMoreThanMaxLimit_RequestPerTimespanRule_ShouldWork()
        {
            // Arrange
            var cache = new CacheService();

            var endpoint = new Endpoint(_configuration, cache);

            var resource = new Resource("api/other");

            var settings = _configuration.GetSection("RequestPerTimespan").Get<RequestPerTimespanSettings>();
            var rule = new RequestPerTimespanRule(settings);
            resource.ApplyRule(rule);
            endpoint.AddResource(resource);

            var requests = Enumerable.Repeat(0, settings.Count + 1).Select(x => new UserRequest(Region.Other, 1))
                .ToList();

            // Act
            requests.ForEach(r => endpoint.AcceptRequest(r, "api/other"));

            // Assert
            requests.Take(requests.Count - 1).Select(x => x.State).Should().AllBeEquivalentTo(RequestState.Success);
            requests[^1].State.Should().Be(RequestState.AccessDenied);
        }

        [Fact]
        public async Task WhenTwoRulesCombinedOnePassesOtherDenies_Request_ShouldBeDenied()
        {
            // Arrange
            var cache = new CacheService();
            var request1 = new UserRequest(Region.Other, 1);
            var request2 = new UserRequest(Region.Other, 1);

            var endpoint = new Endpoint(_configuration, cache);

            var resource = new Resource("api/other");

            var settings1 = _configuration.GetSection("RequestPerTimespan").Get<RequestPerTimespanSettings>();
            var rule1 = new RequestPerTimespanRule(settings1);
            var settings2 = _configuration.GetSection("TimespanPassedSinceLastCall")
                .Get<TimespanSinceLastCallSettings>();
            var rule2 = new TimespanSinceLastCallRule(settings2);
            resource.ApplyRule(rule1);
            resource.ApplyRule(rule2);
            endpoint.AddResource(resource);

            // Act
            endpoint.AcceptRequest(request1, "api/other");
            await Task.Delay(
                100); // for RequestPerTimespanRule second request is ok, but TimespanSinceLastCallRule will block it
            endpoint.AcceptRequest(request2, "api/other");

            // Assert
            request1.State.Should().Be(RequestState.Success);
            request2.State.Should().Be(RequestState.AccessDenied);
        }

        [Fact]
        public async Task WhenUserIsFromUS_Rule_RequestPerTimespanRule_ShouldWork()
        {
            // Arrange
            var cache = new CacheService();

            var endpoint = new Endpoint(_configuration, cache);

            var resource = new Resource("api/us");
            endpoint.AddResource(resource);

            var settings = _configuration.GetSection("RequestPerTimespan").Get<RequestPerTimespanSettings>();

            var requests = Enumerable.Repeat(0, settings.Count + 1).Select(x => new UserRequest(Region.US, 1)).ToList();

            // Act
            requests.ForEach(r => endpoint.AcceptRequest(r, "api/us"));

            // Assert
            requests.Take(requests.Count - 1).Select(x => x.State).Should().AllBeEquivalentTo(RequestState.Success);
            requests[^1].State.Should().Be(RequestState.AccessDenied);
        }

        [Fact]
        public async Task WhenUserIsFromEU_Rule_TimespanSinceLastCallRule_ShouldWork()
        {
            // Arrange
            var cache = new CacheService();

            var request1 = new UserRequest(Region.EU, 1);
            var request2 = new UserRequest(Region.EU, 1);

            var endpoint = new Endpoint(_configuration, cache);

            var resource = new Resource("api/eu");
            endpoint.AddResource(resource);

            var settings = _configuration.GetSection("TimespanPassedSinceLastCall")
                .Get<TimespanSinceLastCallSettings>();

            // Act
            endpoint.AcceptRequest(request1, "api/eu");
            await Task.Delay(settings.InMs - 50); // shorter than in the settings
            endpoint.AcceptRequest(request2, "api/eu");

            // Assert
            request1.State.Should().Be(RequestState.Success);
            request2.State.Should().Be(RequestState.AccessDenied);
        }
    }
}