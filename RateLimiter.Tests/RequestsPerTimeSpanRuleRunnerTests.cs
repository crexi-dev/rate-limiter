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
    public class RequestsPerTimeSpanRuleRunnerTests
    {
        [Fact]
        public async Task RequestsThatExceedConfiguredCountForTheSameClient_Fails()
        {
            var clientRequest = new ClientRequest
            {
                Resource = "Orders",
                ClientKey = "Client_1",
                Region = "US"
            };
            var rule = new RequestsPerTimeSpanRule
            {
                TimeSpan = TimeSpan.Parse("0:00:2"),
                AllowedNumberOfRequests = 1
            };
            var mockCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            mockCacheService.SetupSequence(c => c.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(true);
            mockCacheService.SetupSequence(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 1 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 1 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 1 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 1 });

            var runner = new RequestsPerTimeSpanRuleRunner(rule, mockCacheService.Object);

            var result1 = await runner.RunAsync(clientRequest);
            var result2 = await runner.RunAsync(clientRequest);
            var result3 = await runner.RunAsync(clientRequest);
            var result4 = await runner.RunAsync(clientRequest);

            result1.IsSuccess.Should().BeTrue();
            mockCacheService.Verify(c => c.AddAsync(It.IsAny<string>(), It.IsAny<RequestsPerTimeSpanCount>()), Times.Exactly(1));
            result2.IsSuccess.Should().BeFalse();
            result2.ErrorMessage.Should().NotBeNullOrWhiteSpace();
            result3.IsSuccess.Should().BeFalse();
            result3.ErrorMessage.Should().NotBeNullOrWhiteSpace();
            result4.IsSuccess.Should().BeFalse();
            result4.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task RequestsThatDoesntExceedCountInTimeSpan_Succeeds_Others_Fails()
        {
            var clientRequest = new ClientRequest
            {
                Resource = "Orders",
                ClientKey = "Client_1",
                Region = "US"
            };
            var rule = new RequestsPerTimeSpanRule
            {
                TimeSpan = TimeSpan.Parse("0:00:5"),
                AllowedNumberOfRequests = 3
            };
            var mockCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            var windowStartTime = DateTimeOffset.UtcNow;
            mockCacheService.SetupSequence(c => c.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(true);
            mockCacheService.SetupSequence(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = windowStartTime, Count = 1 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = windowStartTime, Count = 2 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = windowStartTime, Count = 3 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = windowStartTime, Count = 4 });

            var runner = new RequestsPerTimeSpanRuleRunner(rule, mockCacheService.Object);

            var result1 = await runner.RunAsync(clientRequest);
            var result2 = await runner.RunAsync(clientRequest);
            var result3 = await runner.RunAsync(clientRequest);
            var result4 = await runner.RunAsync(clientRequest);
            await Task.Delay(TimeSpan.FromSeconds(5));
            var result5 = await runner.RunAsync(clientRequest);

            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue();
            result3.IsSuccess.Should().BeTrue();
            result4.IsSuccess.Should().BeFalse();
            result5.IsSuccess.Should().BeTrue();
            mockCacheService.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public async Task RequestsThatExceedCountInTimeSpan_Fails_Else_Succeeds_WithMemoryCache()
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
            var rule = new RequestsPerTimeSpanRule
            {
                TimeSpan = TimeSpan.Parse("0:00:5"),
                AllowedNumberOfRequests = 3
            };

            var runner = new RequestsPerTimeSpanRuleRunner(rule, serviceProvider.GetRequiredService<ICacheService<RequestsPerTimeSpanCount>>());

            var result1 = await runner.RunAsync(clientRequest);
            var result2 = await runner.RunAsync(clientRequest);
            var result3 = await runner.RunAsync(clientRequest);
            var result4 = await runner.RunAsync(clientRequest);
            await Task.Delay(TimeSpan.FromSeconds(5));
            var result5 = await runner.RunAsync(clientRequest);

            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue();
            result3.IsSuccess.Should().BeTrue();
            result4.IsSuccess.Should().BeFalse();
            result5.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task RequestsThatDoesntExceedConfiguredCountForTheSameClient_Succeeds()
        {
            var clientRequest = new ClientRequest
            {
                Resource = "Orders",
                ClientKey = "Client_1",
                Region = "US"
            };
            var rule = new RequestsPerTimeSpanRule
            {
                TimeSpan = TimeSpan.Parse("0:00:2"),
                AllowedNumberOfRequests = 10
            };
            var mockCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            mockCacheService.SetupSequence(c => c.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(true);
            mockCacheService.SetupSequence(c => c.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 1 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 2 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 3 })
                .ReturnsAsync(new RequestsPerTimeSpanCount { StartTime = DateTimeOffset.UtcNow, Count = 4 });

            var runner = new RequestsPerTimeSpanRuleRunner(rule, mockCacheService.Object);

            var result1 = await runner.RunAsync(clientRequest);
            var result2 = await runner.RunAsync(clientRequest);
            var result3 = await runner.RunAsync(clientRequest);
            var result4 = await runner.RunAsync(clientRequest);

            result1.IsSuccess.Should().BeTrue();
            mockCacheService.Verify(c => c.AddAsync(It.IsAny<string>(), It.IsAny<RequestsPerTimeSpanCount>()), Times.Exactly(4));
            result2.IsSuccess.Should().BeTrue();
            result3.IsSuccess.Should().BeTrue();
            result4.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task NewRequest_Succeeds()
        {

            var clientRequest = new ClientRequest
            {
                Resource = "Orders",
                ClientKey = "Client_1",
                Region = "US"
            };
            var rule = new RequestsPerTimeSpanRule
            {
                TimeSpan = TimeSpan.Parse("0:01:00"),
                AllowedNumberOfRequests = 60
            };
            var mockCacheService = new Mock<ICacheService<RequestsPerTimeSpanCount>>();
            mockCacheService.Setup(c => c.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var runner = new RequestsPerTimeSpanRuleRunner(rule, mockCacheService.Object);

            var result = await runner.RunAsync(clientRequest);

            result.IsSuccess.Should().BeTrue();
            mockCacheService.Verify(c => c.AddAsync(It.IsAny<string>(), It.IsAny<RequestsPerTimeSpanCount>()), Times.Exactly(1));
        }
    }
}
