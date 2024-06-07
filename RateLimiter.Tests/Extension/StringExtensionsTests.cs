using RateLimiter.Extension;
using RateLimiter.Models;

namespace RateLimiter.Tests.Extension;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("1s", 1)]
    [InlineData("10s", 10)]
    [InlineData("15s", 15)]
    public void ParsePeriod_ValidSecondsPeriod_ReturnsCorrectTimeSpan(
        string period,
        int totalSeconds)
    {
        // Act
        var result = period.ParsePeriod();

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(totalSeconds), result);
    }

    [Theory]
    [InlineData("1m", 1)]
    [InlineData("10m", 10)]
    [InlineData("15m", 15)]
    public void ParsePeriod_ValidMinutesPeriod_ReturnsCorrectTimeSpan(
        string period,
        int minutes)
    {
        // Act
        var result = period.ParsePeriod();

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(minutes), result);
    }

    [Theory]
    [InlineData("1h", 1)]
    [InlineData("10h", 10)]
    [InlineData("15h", 15)]
    public void ParsePeriod_ValidHoursPeriod_ReturnsCorrectTimeSpan(
        string period,
        int hours)
    {
        // Act
        var result = period.ParsePeriod();

        // Assert
        Assert.Equal(TimeSpan.FromHours(hours), result);
    }

    [Theory]
    [InlineData("1d", 1)]
    [InlineData("10d", 10)]
    [InlineData("15d", 15)]
    public void ParsePeriod_ValidDaysPeriod_ReturnsCorrectTimeSpan(
        string period,
        int days)
    {
        // Act
        var result = period.ParsePeriod();

        // Assert
        Assert.Equal(TimeSpan.FromDays(days), result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ParsePeriod_NullOrEmptyPeriod_ThrowsArgumentException(string period)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => period.ParsePeriod());
        Assert.Equal("Period cannot be null or empty (Parameter 'period')", exception.Message);
    }

    [Theory]
    [InlineData("10x")]
    [InlineData("5y")]
    [InlineData("1w")]
    public void ParsePeriod_InvalidUnit_ThrowsArgumentException(string period)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => period.ParsePeriod());
        Assert.Equal($"Invalid period unit '{period[^1]}' (Parameter 'period')", exception.Message);
    }

    [Theory]
    [InlineData("s")]
    [InlineData("m")]
    [InlineData("h")]
    [InlineData("d")]
    public void ParsePeriod_NoNumericValue_ThrowsFormatException(string period)
    {
        // Act & Assert
        Assert.Throws<FormatException>(() => period.ParsePeriod());
    }

    [Theory]
    [InlineData("US-123", Region.US)]
    [InlineData("EU-456", Region.EU)]
    [InlineData("RU-789", Region.RU)]
    public void GetRegionFromToken_ValidAccessToken_ReturnsCorrectRegion(string accessToken, Region expectedRegion)
    {
        // Act
        var result = accessToken.GetRegionFromToken();

        // Assert
        Assert.Equal(expectedRegion, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void GetRegionFromToken_NullOrEmptyAccessToken_ThrowsArgumentException(string accessToken)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => accessToken.GetRegionFromToken());
        Assert.Equal("AccessToken cannot be null or empty (Parameter 'accessToken')", exception.Message);
    }

    [Theory]
    [InlineData("PL-123")]
    [InlineData("12345")]
    [InlineData("LT-")]
    public void GetRegionFromToken_InvalidAccessToken_ThrowsArgumentException(string accessToken)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => accessToken.GetRegionFromToken());
        Assert.Equal($"Invalid accessToken '{accessToken}' (Parameter 'accessToken')", exception.Message);
    }
}