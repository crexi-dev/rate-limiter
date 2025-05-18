using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RateLimiter.Common.Abstractions;
using RateLimiter.Common.Models;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Services;

namespace RateLimiter.UnitTests.Services;

public class SecureClientIdentifierProviderTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IGeoIPService> _geoIPServiceMock;
    private readonly Mock<ILogger<SecureClientIdentifierProvider>> _loggerMock;
    private readonly RateLimitOptions _options;
    private readonly SecureClientIdentifierProvider _provider;

    public SecureClientIdentifierProviderTests()
    {
        _authServiceMock = new Mock<IAuthenticationService>();
        _geoIPServiceMock = new Mock<IGeoIPService>();
        _loggerMock = new Mock<ILogger<SecureClientIdentifierProvider>>();
        
        _options = new RateLimitOptions
        {
            ClientIdHeaderName = "X-ClientId",
            RegionHeaderName = "X-Region"
        };

        var optionsMock = new Mock<IOptions<RateLimitOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        _provider = new SecureClientIdentifierProvider(
            optionsMock.Object,
            _authServiceMock.Object,
            _geoIPServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetClientIdentifierAsync_AuthenticatedUser_ShouldReturnUserInfo()
    {
        // Arrange
        var context = CreateHttpContext("Bearer valid-token", "127.0.0.1");
        var authenticatedUser = new AuthenticatedUser
        {
            UserId = "user123",
            Email = "test@example.com",
            Region = "US",
            Tier = "premium"
        };

        _authServiceMock
            .Setup(s => s.ValidateJwtTokenAsync("Bearer valid-token"))
            .ReturnsAsync(authenticatedUser);

        // Act
        var result = await _provider.GetClientIdentifierAsync(context);

        // Assert
        Assert.Equal("user123", result.Id);
        Assert.Equal("US", result.Region);
        Assert.Equal("127.0.0.1", result.IpAddress); // Fixed: Should match the IP from CreateHttpContext
        Assert.Equal("premium", result.Attributes["tier"]);
        Assert.Equal("test@example.com", result.Attributes["email"]);
    }

    [Fact]
    public async Task GetClientIdentifierAsync_ApiClient_ShouldReturnClientInfo()
    {
        // Arrange
        var context = CreateHttpContextWithApiKey("api-key-123", "127.0.0.1");
        var apiClient = new ApiClient
        {
            ClientId = "client123",
            Name = "Test Client",
            Region = "EU",
            Tier = "standard"
        };

        _authServiceMock
            .Setup(s => s.ValidateJwtTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((AuthenticatedUser?)null);

        _authServiceMock
            .Setup(s => s.ValidateApiKeyAsync("api-key-123"))
            .ReturnsAsync(apiClient);

        // Act
        var result = await _provider.GetClientIdentifierAsync(context);

        // Assert
        Assert.Equal("client123", result.Id);
        Assert.Equal("EU", result.Region);
        Assert.Equal("127.0.0.1", result.IpAddress);
        Assert.Equal("standard", result.Attributes["tier"]);
        Assert.Equal("Test Client", result.Attributes["name"]);
    }

    [Fact]
    public async Task GetClientIdentifierAsync_NoAuthentication_ShouldUseGeoIP()
    {
        // Arrange
        var context = CreateHttpContext(null, "8.8.8.8"); // Using 8.8.8.8 for this test

        _authServiceMock
            .Setup(s => s.ValidateJwtTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((AuthenticatedUser?)null);

        _authServiceMock
            .Setup(s => s.ValidateApiKeyAsync(It.IsAny<string>()))
            .ReturnsAsync((ApiClient?)null);

        _geoIPServiceMock
            .Setup(s => s.GetRegionAsync("8.8.8.8"))
            .ReturnsAsync("US");

        // Act
        var result = await _provider.GetClientIdentifierAsync(context);

        // Assert
        Assert.Equal("8.8.8.8", result.Id); // Fixed: Should match the IP used in test setup
        Assert.Equal("US", result.Region);
        Assert.Equal("8.8.8.8", result.IpAddress);
    }

    [Fact]
    public async Task GetClientIdentifierAsync_GeoIPFailure_ShouldSetUnknownRegion()
    {
        // Arrange
        var context = CreateHttpContext(null, "192.168.1.1"); // Using different IP for this test

        _authServiceMock
            .Setup(s => s.ValidateJwtTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((AuthenticatedUser?)null);

        _authServiceMock
            .Setup(s => s.ValidateApiKeyAsync(It.IsAny<string>()))
            .ReturnsAsync((ApiClient?)null);

        _geoIPServiceMock
            .Setup(s => s.GetRegionAsync("192.168.1.1"))
            .ThrowsAsync(new Exception("GeoIP service unavailable"));

        // Act
        var result = await _provider.GetClientIdentifierAsync(context);

        // Assert
        Assert.Equal("192.168.1.1", result.Id);
        Assert.Equal("UNKNOWN", result.Region);
        Assert.Equal("192.168.1.1", result.IpAddress);
    }

    [Fact]
    public async Task GetClientIdentifierAsync_SuspiciousRegionClaim_ShouldFlagSuspiciousActivity()
    {
        // Arrange - Create context with suspicious region claim
        var context = CreateHttpContextWithRegionHeader("10.0.0.1", "MARS"); // Impossible region

        _authServiceMock
            .Setup(s => s.ValidateJwtTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((AuthenticatedUser?)null);

        _authServiceMock
            .Setup(s => s.ValidateApiKeyAsync(It.IsAny<string>()))
            .ReturnsAsync((ApiClient?)null);

        _geoIPServiceMock
            .Setup(s => s.GetRegionAsync("10.0.0.1"))
            .ReturnsAsync("US"); // GeoIP says US, but header claims MARS

        // Act
        var result = await _provider.GetClientIdentifierAsync(context);

        // Assert
        Assert.Equal("10.0.0.1", result.Id);
        Assert.Equal("US", result.Region); // Should use GeoIP region, not claimed
        Assert.Equal("true", result.Attributes["suspicious_region_claim"]);
        Assert.Equal("MARS", result.Attributes["claimed_region"]);
    }

    [Fact]
    public async Task GetClientIdentifierAsync_EmptyIpAddress_ShouldHandleGracefully()
    {
        // Arrange
        var context = CreateHttpContextWithNullIp();

        _authServiceMock
            .Setup(s => s.ValidateJwtTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((AuthenticatedUser?)null);

        _authServiceMock
            .Setup(s => s.ValidateApiKeyAsync(It.IsAny<string>()))
            .ReturnsAsync((ApiClient?)null);

        // Act
        var result = await _provider.GetClientIdentifierAsync(context);

        // Assert
        Assert.Equal("unknown", result.Id);
        Assert.Null(result.IpAddress);
        // No GeoIP call should be made for null IP
        _geoIPServiceMock.Verify(s => s.GetRegionAsync(It.IsAny<string>()), Times.Never);
    }

    private static HttpContext CreateHttpContext(string? authHeader, string ipAddress)
    {
        var context = new DefaultHttpContext();
        
        if (!string.IsNullOrEmpty(authHeader))
        {
            context.Request.Headers.Append("Authorization", authHeader);
        }
        
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ipAddress);
        context.Request.Headers.Append("User-Agent", "Test Agent");
        context.Request.Headers.Append("Accept-Language", "en-US");
        
        return context;
    }

    private static HttpContext CreateHttpContextWithApiKey(string apiKey, string ipAddress)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Append("X-API-Key", apiKey);
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ipAddress);
        context.Request.Headers.Append("User-Agent", "Test Agent");
        context.Request.Headers.Append("Accept-Language", "en-US");
        
        return context;
    }

    private static HttpContext CreateHttpContextWithRegionHeader(string ipAddress, string region)
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ipAddress);
        context.Request.Headers.Append("X-Region", region);
        context.Request.Headers.Append("User-Agent", "Test Agent");
        context.Request.Headers.Append("Accept-Language", "en-US");
        
        return context;
    }

    private static HttpContext CreateHttpContextWithNullIp()
    {
        var context = new DefaultHttpContext();
        // Don't set RemoteIpAddress - it will be null
        context.Request.Headers.Append("User-Agent", "Test Agent");
        context.Request.Headers.Append("Accept-Language", "en-US");
        
        return context;
    }
}
