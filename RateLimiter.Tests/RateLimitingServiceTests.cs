using System;
using FluentAssertions;
using Moq;
using RateLimiter.Extensions;
using RateLimiter.Models;
using RateLimiter.Services;
using RateLimiter.Services.Interfaces;
using Xunit;

namespace RateLimiter.Tests;

public class RateLimitingServiceTests
{
    private const long MaxRequestsPerPeriod = 3;
    private static readonly TimeSpan Period = TimeSpan.FromSeconds(5);
    private static readonly DateTime Now = DateTime.Now;

    private readonly IRateLimitingService _rateLimitingService;

    private readonly Mock<IRateLimitStorageService> _rateLimitStorageServiceMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly RateLimitRule _rateLimitRule;

    public RateLimitingServiceTests()
    {
        _rateLimitStorageServiceMock = new Mock<IRateLimitStorageService>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);
        _dateTimeProviderMock.SetupGet(x => x.UtcNow).Returns(Now);

        _rateLimitingService = new RateLimitingService(
            _rateLimitStorageServiceMock.Object,
            _dateTimeProviderMock.Object);

        _rateLimitRule = new RateLimitRule(Period, MaxRequestsPerPeriod);
    }

    [Fact]
    public void GetRateLimitCounter_FirstRequest_ShouldReturnDefaultRateLimitCounter()
    {
        // Arrange
        var identity = new ClientRequestIdentity(Guid.NewGuid().ToString(), "/create", "POST");
        var key = identity.GetStorageKey(Period);
        _rateLimitStorageServiceMock.Setup(x => x.Get(key)).Returns((RateLimitCounter?)null);

        // Act
        var result = _rateLimitingService.GetRateLimitCounter(identity, _rateLimitRule);

        // Assert
        result.StartedAt.Should().Be(Now);
        result.ExceededAt.Should().BeNull();
        result.TotalRequests.Should().Be(1);
        _rateLimitStorageServiceMock.VerifyAll();
        _rateLimitStorageServiceMock
            .Verify(x =>
                x.Set(
                    key,
                    It.Is<RateLimitCounter>(c =>
                        c.StartedAt == Now
                        && c.ExceededAt.HasValue == false
                        && c.TotalRequests == 1),
                    Period),
                Times.Once);
        _rateLimitStorageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetRateLimitCounter_OneRequestUntilBan_ShouldReturnRateLimitCounterWithMaxRequestsPerPeriod()
    {
        // Arrange
        var identity = new ClientRequestIdentity(Guid.NewGuid().ToString(), "/create", "POST");
        var key = identity.GetStorageKey(Period);
        var halfOfPeriod = Period / 2;
        var startedAt = Now.Add(-halfOfPeriod);
        var exceededAt = Now.Add(halfOfPeriod);
        _rateLimitStorageServiceMock
            .Setup(x => x.Get(key))
            .Returns(new RateLimitCounter(
                startedAt,
                exceededAt,
                MaxRequestsPerPeriod - 1));

        // Act
        var result = _rateLimitingService.GetRateLimitCounter(identity, _rateLimitRule);

        // Assert
        result.StartedAt.Should().Be(startedAt);
        result.ExceededAt.Should().Be(exceededAt);
        result.TotalRequests.Should().Be(MaxRequestsPerPeriod);
        _rateLimitStorageServiceMock.VerifyAll();
        _rateLimitStorageServiceMock
            .Verify(x =>
                x.Set(
                    key,
                    It.Is<RateLimitCounter>(c =>
                        c.StartedAt == startedAt
                        && c.ExceededAt == exceededAt
                        && c.TotalRequests == MaxRequestsPerPeriod),
                    Period),
                Times.Once);
        _rateLimitStorageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetRateLimitCounter_BanExpired_ShouldReturnDefaultRateLimitCounter()
    {
        // Arrange
        var identity = new ClientRequestIdentity(Guid.NewGuid().ToString(), "/create", "POST");
        var key = identity.GetStorageKey(Period);
        var startedAt = Now.Add(-Period).AddSeconds(-1);
        var exceededAt = Now.Add(-Period).AddMilliseconds(-1);
        _rateLimitStorageServiceMock
           .Setup(x => x.Get(key))
           .Returns(new RateLimitCounter(
               startedAt,
               exceededAt,
               MaxRequestsPerPeriod));

        // Act
        var result = _rateLimitingService.GetRateLimitCounter(identity, _rateLimitRule);

        // Assert
        result.StartedAt.Should().Be(Now);
        result.ExceededAt.Should().BeNull();
        result.TotalRequests.Should().Be(1);
        _rateLimitStorageServiceMock.VerifyAll();
        _rateLimitStorageServiceMock
            .Verify(x =>
                x.Set(
                    key,
                    It.Is<RateLimitCounter>(c =>
                        c.StartedAt == Now
                        && c.ExceededAt.HasValue == false
                        && c.TotalRequests == 1),
                    Period),
                Times.Once);
        _rateLimitStorageServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetRateLimitCounter_BanNotExpired_ShouldReturnRateLimitCounterWithBan()
    {
        // Arrange
        var identity = new ClientRequestIdentity(Guid.NewGuid().ToString(), "/create", "POST");
        var key = identity.GetStorageKey(Period);
        var startedAt = Now.Add(-Period).AddSeconds(-1);
        var exceededAt = Now.Add(-Period).AddMilliseconds(1);

        _rateLimitStorageServiceMock
           .Setup(x => x.Get(key))
           .Returns(new RateLimitCounter(
               startedAt,
               exceededAt,
               MaxRequestsPerPeriod));

        // Act
        var result = _rateLimitingService.GetRateLimitCounter(identity, _rateLimitRule);

        // Assert
        result.StartedAt.Should().Be(startedAt);
        result.ExceededAt.Should().Be(exceededAt);
        result.TotalRequests.Should().Be(MaxRequestsPerPeriod + 1);
    }
}
