using Moq;
using RateLimiter.Core;
using RateLimiter.Rules.Implementations.RequestPerPeriodRule;
using System;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using Xunit;

public class RequestsPerPeriodRuleUnitTests
{
    [Fact]
    public async Task IsRequestAllowedAsync_RequestWithinLimit_ReturnsTrue()
    {
        // Arrange
        var limit = 5;
        var period = TimeSpan.FromMinutes(1);
        var mockRepository = new Mock<IRequestPerPeriodRuleRepository>();

        mockRepository.Setup(repo => repo.GetOrAdd(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                     .Returns((DateTime.UtcNow, 0));        // Whenever GetOrAdd is called with any string, DateTime, and int, it returns (DateTime.UtcNow, 0)

        var rule = new RequestsPerPeriodRule(limit, period, mockRepository.Object);

        var context = new ClientRequestContext("client1", "/api/resource");

        // Act
        var result = await rule.IsRequestAllowedAsync(context);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsRequestAllowedAsync_RequestExceedsLimit_ReturnsFalse()
    {
        // Arrange
        var limit = 5;
        var period = TimeSpan.FromMinutes(1);
        var mockRepository = new Mock<IRequestPerPeriodRuleRepository>();
        mockRepository.Setup(repo => repo.GetOrAdd(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                      .Returns((DateTime.UtcNow, limit));   // Whenever GetOrAdd is called with any string, DateTime, and int, it returns (DateTime.UtcNow, limit)

        var rule = new RequestsPerPeriodRule(limit, period, mockRepository.Object);
        var context = new ClientRequestContext("client1", "/api/resource");

        // Act
        for (int i = 0; i < limit; i++)
        {
            await rule.IsRequestAllowedAsync(context);
        }
        var result = await rule.IsRequestAllowedAsync(context);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRequestAllowedAsync_RequestAfterPeriod_ReturnsTrue()
    {
        // Arrange
        var limit = 5;
        var period = TimeSpan.FromSeconds(1);
        var mockRepository = new Mock<IRequestPerPeriodRuleRepository>();
        
        mockRepository.SetupSequence(repo => repo.GetOrAdd(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                      .Returns((DateTime.UtcNow, limit))          //First call: Simulates the user has already hit the limit.
                      .Returns((DateTime.UtcNow.Add(period), 0)); //Second call: Simulates that a new time window has started, and the counter is reset.

        var rule = new RequestsPerPeriodRule(limit, period, mockRepository.Object);
        var context = new ClientRequestContext("client1", "/api/resource");

        // Act
        for (int i = 0; i < limit; i++)
        {
            await rule.IsRequestAllowedAsync(context);
        }
        await Task.Delay(period);
        var result = await rule.IsRequestAllowedAsync(context);

        // Assert
        Assert.True(result);
    }
}
