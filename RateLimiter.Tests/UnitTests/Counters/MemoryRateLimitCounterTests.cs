using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using RateLimiter.Infrastructure.Counters;

namespace RateLimiter.UnitTests.Counters;

public class MemoryRateLimitCounterTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<ILogger<MemoryRateLimitCounter>> _loggerMock;
    private readonly MemoryRateLimitCounter _counter;

    public MemoryRateLimitCounterTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<MemoryRateLimitCounter>>();
        _counter = new MemoryRateLimitCounter(_memoryCache, _loggerMock.Object);
    }

    [Fact]
    public async Task GetCountAsync_WhenKeyExists_ShouldReturnValue()
    {
        // Arrange
        const string key = "test-key";
        const long value = 42;
        
        _memoryCache.Set(key, value);

        // Act
        var result = await _counter.GetCountAsync(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public async Task GetCountAsync_WhenKeyDoesNotExist_ShouldReturnZero()
    {
        // Arrange
        const string key = "non-existent-key";

        // Act
        var result = await _counter.GetCountAsync(key);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task SetCountAsync_ShouldSetValueWithExpiration()
    {
        // Arrange
        const string key = "test-key";
        const long value = 42;
        var expiry = TimeSpan.FromSeconds(1);

        // Act
        await _counter.SetCountAsync(key, value, expiry);
        var result = await _counter.GetCountAsync(key);

        // Assert
        Assert.Equal(value, result);
        
        // Verify the value expires
        await Task.Delay(expiry.Add(TimeSpan.FromMilliseconds(100))); // Add a little buffer
        var expiredResult = await _counter.GetCountAsync(key);
        Assert.Equal(0, expiredResult);
    }

    [Fact]
    public async Task IncrementAsync_ShouldIncrementValue()
    {
        // Arrange
        const string key = "test-key";
        const long initialValue = 10;
        const long increment = 5;
        var expiry = TimeSpan.FromMinutes(1);
        
        // Set initial value
        await _counter.SetCountAsync(key, initialValue, expiry);

        // Act
        await _counter.IncrementAsync(key, increment, expiry);
        var result = await _counter.GetCountAsync(key);

        // Assert
        Assert.Equal(initialValue + increment, result);
    }

    [Fact]
    public async Task IncrementAsync_WhenKeyDoesNotExist_ShouldCreateKey()
    {
        // Arrange
        const string key = "new-key";
        const long increment = 5;
        var expiry = TimeSpan.FromMinutes(1);

        // Act
        await _counter.IncrementAsync(key, increment, expiry);
        var result = await _counter.GetCountAsync(key);

        // Assert
        Assert.Equal(increment, result);
    }

    [Fact]
    public async Task DecrementAsync_ShouldDecrementValue()
    {
        // Arrange
        const string key = "test-key";
        const long initialValue = 10;
        const long decrement = 3;
        var expiry = TimeSpan.FromMinutes(1);
        
        // Set initial value
        await _counter.SetCountAsync(key, initialValue, expiry);

        // Act
        await _counter.DecrementAsync(key, decrement);
        var result = await _counter.GetCountAsync(key);

        // Assert
        Assert.Equal(initialValue - decrement, result);
    }

    [Fact]
    public async Task DecrementAsync_ShouldNotGoNegative()
    {
        // Arrange
        const string key = "test-key";
        const long initialValue = 5;
        const long decrement = 10; // More than initial value
        var expiry = TimeSpan.FromMinutes(1);
        
        // Set initial value
        await _counter.SetCountAsync(key, initialValue, expiry);

        // Act
        await _counter.DecrementAsync(key, decrement);
        var result = await _counter.GetCountAsync(key);

        // Assert
        Assert.Equal(0, result); // Should be clamped to 0
    }

    [Fact]
    public async Task ResetAsync_ShouldRemoveKeysForClient()
    {
        // Arrange
        const string clientId = "test-client";
        const string key1 = "rule1:test-client:endpoint1";
        const string key2 = "rule2:test-client:endpoint2";
        const string otherKey = "rule3:other-client:endpoint3";
        
        // Set the keys in our counter implementation first to make them tracked
        await _counter.SetCountAsync(key1, 10, TimeSpan.FromMinutes(1));
        await _counter.SetCountAsync(key2, 20, TimeSpan.FromMinutes(1));
        await _counter.SetCountAsync(otherKey, 30, TimeSpan.FromMinutes(1));

        // Verify values were actually set in the cache
        Assert.Equal(10, await _counter.GetCountAsync(key1));
        Assert.Equal(20, await _counter.GetCountAsync(key2));
        Assert.Equal(30, await _counter.GetCountAsync(otherKey));

        // Act - reset the counters for the specific client
        await _counter.ResetAsync(clientId);
        
        // Assert - check that the MemoryCache entries are removed
        Assert.Equal(0, await _counter.GetCountAsync(key1));
        Assert.Equal(0, await _counter.GetCountAsync(key2));
        Assert.Equal(30, await _counter.GetCountAsync(otherKey));
    }
}
