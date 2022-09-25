using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RateLimiter.Models;
using RateLimiter.RuleRunners;
using RateLimiter.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RateLimiter.Tests
{
    public class TimeSpanSinceLastCallRuleRunnerTests
    {
        [Fact]
        public async Task RequestsThatViolateTimespanSinceLastCall_Fails()
        {
            var clientRequest = new ClientRequest
            {
                Resource = "Orders",
                ClientKey = "Client_1",
                Region = "US"
            };
            var rule = new TimeSpanSinceLastCallRule
            {
                TimeSpan = TimeSpan.Parse("0:00:1")
            };
            var mockCacheService = new Mock<ICacheService<TimeSpanSinceLastCall>>();
            mockCacheService.SetupSequence(c => c.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(true);
            mockCacheService.SetupSequence(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow })
                .ReturnsAsync(new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow })
                .ReturnsAsync(new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow })
                .ReturnsAsync(new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow });

            var runner = new TimeSpanSinceLastCallRuleRunner(rule, mockCacheService.Object);

            var result1 = await runner.RunAsync(clientRequest);
            var result2 = await runner.RunAsync(clientRequest);
            var result3 = await runner.RunAsync(clientRequest);
            var result4 = await runner.RunAsync(clientRequest);

            result1.IsSuccess.Should().BeTrue();
            mockCacheService.Verify(c => c.AddAsync(It.IsAny<string>(), It.IsAny<TimeSpanSinceLastCall>()), Times.Exactly(4));
            result2.IsSuccess.Should().BeFalse();
            result2.ErrorMessage.Should().NotBeNullOrWhiteSpace();
            result3.IsSuccess.Should().BeFalse();
            result3.ErrorMessage.Should().NotBeNullOrWhiteSpace();
            result4.IsSuccess.Should().BeFalse();
            result4.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task RequestsThatDoesntViolate_Succeeds_Others_Fails()
        {
            var clientRequest = new ClientRequest
            {
                Resource = "Orders",
                ClientKey = "Client_1",
                Region = "US"
            };
            var rule = new TimeSpanSinceLastCallRule
            {
                TimeSpan = TimeSpan.Parse("0:00:1")
            };
            var mockCacheService = new Mock<ICacheService<TimeSpanSinceLastCall>>();
            var windowStartTime = DateTimeOffset.UtcNow;
            mockCacheService.SetupSequence(c => c.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(true);
            mockCacheService.SetupSequence(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow })
                .ReturnsAsync(new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow.AddSeconds(-1) })
                .ReturnsAsync(new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow })
                .ReturnsAsync(new TimeSpanSinceLastCall { LastCallTime = DateTimeOffset.UtcNow.AddSeconds(-1) });

            var runner = new TimeSpanSinceLastCallRuleRunner(rule, mockCacheService.Object);

            var result1 = await runner.RunAsync(clientRequest);
            var result2 = await runner.RunAsync(clientRequest);
            var result3 = await runner.RunAsync(clientRequest);
            var result4 = await runner.RunAsync(clientRequest);
            var result5 = await runner.RunAsync(clientRequest);

            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeFalse();
            result2.ErrorMessage.Should().NotBeNullOrWhiteSpace();
            result3.IsSuccess.Should().BeTrue();
            result4.IsSuccess.Should().BeFalse();
            result4.ErrorMessage.Should().NotBeNullOrWhiteSpace();
            result5.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task RequestsThatDoesntViolate_Succeeds_Others_Fails_WithMemoryCache()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddScoped(typeof(ICacheService<>), typeof(MemoryCacheService<>));
            var serviceProvider = services.BuildServiceProvider();
            var clientRequest = new ClientRequest
            {
                Resource = "Orders",
                ClientKey = "Client_1",
                Region = "US"
            };
            var rule = new TimeSpanSinceLastCallRule
            {
                TimeSpan = TimeSpan.Parse("0:00:1")
            };

            var runner = new TimeSpanSinceLastCallRuleRunner(rule, serviceProvider.GetRequiredService<ICacheService<TimeSpanSinceLastCall>>());

            var result1 = await runner.RunAsync(clientRequest);
            var result2 = await runner.RunAsync(clientRequest);
            await Task.Delay(TimeSpan.FromSeconds(1));
            var result3 = await runner.RunAsync(clientRequest);
            var result4 = await runner.RunAsync(clientRequest);
            await Task.Delay(TimeSpan.FromSeconds(1));
            var result5 = await runner.RunAsync(clientRequest);

            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeFalse();
            result2.ErrorMessage.Should().NotBeNullOrWhiteSpace();
            result3.IsSuccess.Should().BeTrue();
            result4.IsSuccess.Should().BeFalse();
            result4.ErrorMessage.Should().NotBeNullOrWhiteSpace();
            result5.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task NewRequestSucceeds_WithMemoryCache()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddScoped(typeof(ICacheService<>), typeof(MemoryCacheService<>));
            var serviceProvider = services.BuildServiceProvider();
            var clientRequest = new ClientRequest
            {
                Resource = "Orders",
                ClientKey = "Client_1",
                Region = "US"
            };
            var rule = new TimeSpanSinceLastCallRule
            {
                TimeSpan = TimeSpan.Parse("0:00:1")
            };

            var runner = new TimeSpanSinceLastCallRuleRunner(rule, serviceProvider.GetRequiredService<ICacheService<TimeSpanSinceLastCall>>());

            var result1 = await runner.RunAsync(clientRequest);

            result1.IsSuccess.Should().BeTrue();
        }
    }
}
