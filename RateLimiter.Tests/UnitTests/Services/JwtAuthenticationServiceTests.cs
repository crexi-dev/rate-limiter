using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using RateLimiter.Core.Configuration;
using RateLimiter.Core.Services;

namespace RateLimiter.UnitTests.Services;

public class JwtAuthenticationServiceTests
{
    private readonly Mock<ILogger<JwtAuthenticationService>> _loggerMock;
    private readonly JwtAuthenticationOptions _options;
    private readonly JwtAuthenticationService _service;
    private readonly string _secretKey = "this-is-a-very-long-secret-key-for-testing-only-it-must-be-at-least-256-bits-long";

    public JwtAuthenticationServiceTests()
    {
        _loggerMock = new Mock<ILogger<JwtAuthenticationService>>();
        _options = new JwtAuthenticationOptions
        {
            SecretKey = _secretKey,
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            Enabled = true
        };

        var optionsMock = new Mock<IOptions<JwtAuthenticationOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        _service = new JwtAuthenticationService(optionsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateJwtTokenAsync_ValidToken_ShouldReturnUser()
    {
        // Arrange
        var token = CreateValidJwtToken("test-user", "test@example.com");

        // Act
        var result = await _service.ValidateJwtTokenAsync(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-user", result.UserId);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task ValidateJwtTokenAsync_InvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var result = await _service.ValidateJwtTokenAsync(invalidToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateJwtTokenAsync_ExpiredToken_ShouldReturnNull()
    {
        // Arrange
        var expiredToken = CreateExpiredJwtToken("test-user");

        // Act
        var result = await _service.ValidateJwtTokenAsync(expiredToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateJwtTokenAsync_BearerPrefix_ShouldHandleCorrectly()
    {
        // Arrange
        var token = CreateValidJwtToken("test-user", "test@example.com");
        var bearerToken = $"Bearer {token}";

        // Act
        var result = await _service.ValidateJwtTokenAsync(bearerToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test-user", result.UserId);
    }

    [Fact]
    public async Task ValidateApiKeyAsync_ShouldReturnNull()
    {
        // Arrange & Act
        var result = await _service.ValidateApiKeyAsync("some-api-key");

        // Assert
        Assert.Null(result); // JWT service doesn't handle API keys
    }

    private string CreateValidJwtToken(string userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", userId),
            new Claim("email", email),
            new Claim("region", "US"),
            new Claim("tier", "premium")
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string CreateExpiredJwtToken(string userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", userId)
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(-1), // Expired
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
