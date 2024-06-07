using Moq;
using RateLimiter.Models;
using RateLimiter.Repositories.Interfaces;
using RateLimiter.Services;

namespace RateLimiter.Tests.Services;

public class RequestsPerTimespanValidationTests
{
    private const int StatusCode429 = 429;
    private const int StatusCode200 = 200;
    private const Region DefaultRegion = Region.US;
    private const string DefaultEndpoint = "get:/api/test";
    private const string DefaultAccessToken = "US-test-access-token";
    private readonly Mock<IRateLimitRuleRepository> _rateLimitRuleRepositoryMock = new();

    private static RequestsPerTimespanRuleModel RequestsPerTimespanRuleModel(double limit = 5, string period = "1m")
    {
        return new RequestsPerTimespanRuleModel
        {
            Endpoint = DefaultEndpoint,
            Period = period,
            Limit = limit,
            Regions = [DefaultRegion]
        };
    }

    private static Dictionary<DateTime, RateLimitRequestModel> RequestHistory(int countItems = 1)
    {
        var requestHistory = new Dictionary<DateTime, RateLimitRequestModel>();

        for (int i = 1; i <= countItems; i++)
        {
            var date = DateTime.UtcNow.AddSeconds(-5 * i);

            requestHistory.Add(
                date,
                new RateLimitRequestModel
                {
                    Path = DefaultEndpoint,
                    StatusCode = StatusCode200,
                    DateTime = date,
                    Region = DefaultRegion,
                    AccessToken = DefaultAccessToken
                });
        }

        return requestHistory;
    }

    [Fact]
    public void Check_DisableRateLimiting_ReturnStatusCode200()
    {
        // Arrange
        var ruleOptions = new RequestsPerTimespanRuleOptions
        {
            EnableRateLimiting = false
        };

        var requestHistory = RequestHistory(1);

        // Act
        _rateLimitRuleRepositoryMock.Setup(x => x.RequestsPerTimespanRule()).Returns(ruleOptions);
        var validation = new RequestsPerTimespanValidation(_rateLimitRuleRepositoryMock.Object);
        var result = validation.Check(DefaultEndpoint, DefaultRegion, requestHistory);

        // Assert
        Assert.True(result.isAllowed);
        Assert.Equal(StatusCode200, result.statusCode);
    }

    [Theory]
    [InlineData(null, "Period cannot be null or empty (Parameter 'period')")]
    [InlineData("", "Period cannot be null or empty (Parameter 'period')")]
    [InlineData("  ", "Period cannot be null or empty (Parameter 'period')")]
    [InlineData("\t", "Period cannot be null or empty (Parameter 'period')")]
    [InlineData("10x", "Invalid period unit 'x' (Parameter 'period')")]
    [InlineData("10y", "Invalid period unit 'y' (Parameter 'period')")]
    public void Check_ParsePeriod_ShouldThrowArgumentException_WhenPeriodIsNullOrInvalid(string period, string message)
    {
        // Arrange
        var ruleOptions = new RequestsPerTimespanRuleOptions
        {
            EnableRateLimiting = true,
            StatusCode = StatusCode429,
            Message = "Too many requests! Maximum allowed: {0}. Please try again in {1} second(s).",
            Rules =
            [
                RequestsPerTimespanRuleModel(5, period)
            ]
        };

        var requestHistory = RequestHistory();

        // Act
        _rateLimitRuleRepositoryMock.Setup(x => x.RequestsPerTimespanRule()).Returns(ruleOptions);
        var validation = new RequestsPerTimespanValidation(_rateLimitRuleRepositoryMock.Object);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => validation.Check(DefaultEndpoint, DefaultRegion, requestHistory));
        Assert.Equal(message, ex.Message);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    [InlineData(0)]
    public void Check_ShouldDenyRequest_WhenOverLimit(double limit)
    {
        // Arrange
        var ruleOptions = new RequestsPerTimespanRuleOptions
        {
            EnableRateLimiting = true,
            StatusCode = StatusCode429,
            Message = "Too many requests! Maximum allowed: {0} per {1}. Please try again in {2} second(s).",
            Rules =
            [
                RequestsPerTimespanRuleModel(limit)
            ]
        };

        var requestHistory = RequestHistory(3);

        // Act
        _rateLimitRuleRepositoryMock.Setup(x => x.RequestsPerTimespanRule()).Returns(ruleOptions);
        var validation = new RequestsPerTimespanValidation(_rateLimitRuleRepositoryMock.Object);
        var result = validation.Check(DefaultEndpoint, DefaultRegion, requestHistory);

        // Assert
        Assert.False(result.isAllowed);
        Assert.Equal(StatusCode429, result.statusCode);
        Assert.Contains("Too many requests!", result.message);
    }
        
    [Theory]
    [InlineData(5)]
    [InlineData(4)]
    [InlineData(3)]
    [InlineData(2)]
    public void Check_ShouldAllowRequest_WhenUnderLimit(double limit)
    {
        // Arrange
        var ruleOptions = new RequestsPerTimespanRuleOptions
        {
            EnableRateLimiting = true,
            StatusCode = StatusCode429,
            Message = "Too many requests! Maximum allowed: {0} per {1}. Please try again in {2} second(s).",
            Rules =
            [
                RequestsPerTimespanRuleModel(limit)
            ]
        };

        var requestHistory = RequestHistory(1);

        // Act
        _rateLimitRuleRepositoryMock.Setup(x => x.RequestsPerTimespanRule()).Returns(ruleOptions);
        var validation = new RequestsPerTimespanValidation(_rateLimitRuleRepositoryMock.Object);
        var result = validation.Check(DefaultEndpoint, DefaultRegion, requestHistory);

        // Assert
        Assert.True(result.isAllowed);
        Assert.Equal(StatusCode200, result.statusCode);
    }
}