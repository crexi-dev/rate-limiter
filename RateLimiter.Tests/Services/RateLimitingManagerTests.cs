using Moq;
using RateLimiter.Models;
using RateLimiter.Repositories.Interfaces;
using RateLimiter.Services.Interfaces;
using RateLimiter.Services;

namespace RateLimiter.Tests.Services;

public class RateLimitingManagerTests
{
    private const string DefaultResource = "get:/api/test";
    private const string DefaultAccessToken = "US-access-token";
    private const Region DefaultRegion = Region.US;
    private const int StatusCode429 = 429;
    private const int StatusCode200 = 200;

    [Fact]
    public async Task IsRequestAllowedAsync_RequestAllowed_ReturnsOk()
    {
        // Arrange
        var requestHistory = new Dictionary<DateTime, RateLimitRequestModel>();

        var mockValidator = new Mock<IRequestValidator>();
        mockValidator
            .Setup(v => v.Check(DefaultResource, DefaultRegion, requestHistory))
            .Returns((true, StatusCode200, string.Empty));

        var mockRepository = new Mock<IRateLimitRepository>();
        mockRepository
            .Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(requestHistory);

        var manager = new RateLimitingManager(mockValidator.Object, mockRepository.Object);

        // Act
        var result = await manager.IsRequestAllowedAsync(DefaultResource, DefaultAccessToken);

        // Assert
        Assert.Equal(StatusCode200, result.StatusCode);
        Assert.Equal(string.Empty, result.Message);
    }

    [Fact]
    public async Task IsRequestAllowedAsync_RequestDenied_ReturnsRateLimitExceeded()
    {
        // Arrange
        var requestHistory = new Dictionary<DateTime, RateLimitRequestModel>
        {
            { DateTime.UtcNow.AddMinutes(-1), new RateLimitRequestModel { Path = DefaultResource, StatusCode = StatusCode200 } }
        };

        var mockValidator = new Mock<IRequestValidator>();
        mockValidator
            .Setup(v => v.Check(DefaultResource, DefaultRegion, requestHistory))
            .Returns((false, StatusCode429, "Rate limit exceeded"));

        var mockRepository = new Mock<IRateLimitRepository>();
        mockRepository
            .Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(requestHistory);

        var manager = new RateLimitingManager(mockValidator.Object, mockRepository.Object);

        // Act
        var result = await manager.IsRequestAllowedAsync(DefaultResource, DefaultAccessToken);

        // Assert
        Assert.Equal(StatusCode429, result.StatusCode);
        Assert.Equal("Rate limit exceeded", result.Message);
    }

    [Fact]
    public async Task IsRequestAllowedAsync_SavesRequestHistory()
    {
        // Arrange
        var requestHistory = new Dictionary<DateTime, RateLimitRequestModel>();

        var mockValidator = new Mock<IRequestValidator>();
        mockValidator
            .Setup(v => v.Check(DefaultResource, DefaultRegion, requestHistory))
            .Returns((true, StatusCode200, string.Empty));

        var mockRepository = new Mock<IRateLimitRepository>();
        mockRepository
            .Setup(r => r.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(requestHistory);
        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<RateLimitRequestModel>(), It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var manager = new RateLimitingManager(mockValidator.Object, mockRepository.Object);

        // Act
        await manager.IsRequestAllowedAsync(DefaultResource, DefaultAccessToken);

        // Assert
        mockRepository.Verify(r => r.AddAsync(It.Is<RateLimitRequestModel>(model =>
            model.AccessToken == DefaultAccessToken &&
            model.Path == DefaultResource &&
            model.Region == DefaultRegion &&
            model.StatusCode == StatusCode200
        ), It.IsAny<TimeSpan?>()), Times.Once);
    }
}