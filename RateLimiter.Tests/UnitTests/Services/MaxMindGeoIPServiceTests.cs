using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RateLimiter.Core.Configuration;
using RateLimiter.Infrastructure.Services;

namespace RateLimiter.UnitTests.Services;

public class MaxMindGeoIPServiceTests
{
    private readonly Mock<ILogger<MaxMindGeoIPService>> _loggerMock;
    private readonly GeoIPOptions _options;

    public MaxMindGeoIPServiceTests()
    {
        _loggerMock = new Mock<ILogger<MaxMindGeoIPService>>();
        _options = new GeoIPOptions
        {
            DatabasePath = "", // Empty path for testing without actual database
            DefaultRegion = "UNKNOWN",
            Enabled = false
        };
    }

    [Fact]
    public async Task GetLocationAsync_NoDatabaseConfigured_ShouldReturnNull()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<GeoIPOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);
        
        using var service = new MaxMindGeoIPService(optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetLocationAsync("8.8.8.8");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLocationAsync_InvalidIpAddress_ShouldReturnNull()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<GeoIPOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);
        
        using var service = new MaxMindGeoIPService(optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetLocationAsync("invalid-ip");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRegionAsync_NoDatabaseConfigured_ShouldReturnDefaultRegion()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<GeoIPOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);
        
        using var service = new MaxMindGeoIPService(optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetRegionAsync("8.8.8.8");

        // Assert
        Assert.Equal("UNKNOWN", result);
    }

    [Fact]
    public async Task GetRegionAsync_EmptyIpAddress_ShouldReturnDefaultRegion()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<GeoIPOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);
        
        using var service = new MaxMindGeoIPService(optionsMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetRegionAsync("");

        // Assert
        Assert.Equal("UNKNOWN", result);
    }
}
