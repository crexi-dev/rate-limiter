using Moq;
using RateLimiter.Models;
using RateLimiter.Repositories.Interfaces;
using RateLimiter.Services;

namespace RateLimiter.Tests.Services;

public class RequestsPerPeriodValidationTests
{
    private const int StatusCode429 = 429;
    private const int StatusCode200 = 200;
    private const Region DefaultRegion = Region.US;
    private const string DefaultEndpoint = "get:/api/test";
    private const string DefaultAccessToken = "US-test-access-token";
    private readonly Mock<IRateLimitRuleRepository> _rateLimitRuleRepositoryMock = new();

    private static RequestsPerPeriodRuleModel RequestsPerPeriodRuleModel(string period = "10s")
    {
        return new RequestsPerPeriodRuleModel
        {
            Endpoint = DefaultEndpoint,
            Period = period,
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
        var ruleOptions = new RequestsPerPeriodRuleOptions
        {
            EnableRateLimiting = false
        };

        var requestHistory = RequestHistory(1);

        // Act
        _rateLimitRuleRepositoryMock.Setup(x => x.RequestsPerPeriodRule()).Returns(ruleOptions);
        var validation = new RequestsPerPeriodValidation(_rateLimitRuleRepositoryMock.Object);
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
        var ruleOptions = new RequestsPerPeriodRuleOptions
        {
            EnableRateLimiting = true,
            StatusCode = StatusCode429,
            Message = "Too many requests! Maximum allowed: {0}. Please try again in {1} second(s).",
            Rules =
            [
                RequestsPerPeriodRuleModel(period)
            ]
        };

        var requestHistory = RequestHistory();

        // Act
        _rateLimitRuleRepositoryMock.Setup(x => x.RequestsPerPeriodRule()).Returns(ruleOptions);
        var validation = new RequestsPerPeriodValidation(_rateLimitRuleRepositoryMock.Object);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => validation.Check(DefaultEndpoint, DefaultRegion, requestHistory));
        Assert.Equal(message, ex.Message);
    }

    [Theory]
    [InlineData(1, true, StatusCode200, "")]
    [InlineData(2, true, StatusCode200, "")]
    [InlineData(3, true, StatusCode200, "")]
    [InlineData(4, true, StatusCode200, "")]
    [InlineData(5, true, StatusCode200, "")]
    [InlineData(6, false, StatusCode429, "Too many requests!")]
    [InlineData(7, false, StatusCode429, "Too many requests!")]
    [InlineData(8, false, StatusCode429, "Too many requests!")]
    [InlineData(9, false, StatusCode429, "Too many requests!")]
    [InlineData(10, false, StatusCode429, "Too many requests!")]
    public void Check_ShouldDenyRequest_WhenPeriodHasNotPassed(int period, bool isAllowed, int? statusCode, string message)
    {
        // Arrange
        var ruleOptions = new RequestsPerPeriodRuleOptions
        {
            EnableRateLimiting = true,
            StatusCode = StatusCode429,
            Message = "Too many requests! Maximum allowed: {0}. Please try again in {1} second(s).",
            Rules =
            [
                RequestsPerPeriodRuleModel($"{period}s")
            ]
        };

        var requestHistory = RequestHistory(1);

        // Act
        _rateLimitRuleRepositoryMock.Setup(x => x.RequestsPerPeriodRule()).Returns(ruleOptions);
        var validation = new RequestsPerPeriodValidation(_rateLimitRuleRepositoryMock.Object);
        var result = validation.Check(DefaultEndpoint, DefaultRegion, requestHistory);

        // Assert
        Assert.Equal(isAllowed, result.isAllowed);
        Assert.Equal(statusCode, result.statusCode);
        Assert.Contains(message, result.message);
    }
}