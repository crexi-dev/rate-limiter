using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Storage;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class CertainTimeSpanSinceLastCallRuleTests
{
    private const string Token = "testToken";
    private RateLimitOptions options = new() { SinceLastCall = TimeSpan.FromSeconds(20) };

    private Mock<IStorage<RateLimitEntry>> mockStorage;
    private CertainTimeSpanSinceLastCallRule underTest;

    [SetUp]
    public void Setup()
    {
        mockStorage = new Mock<IStorage<RateLimitEntry>>();
        underTest = new CertainTimeSpanSinceLastCallRule(mockStorage.Object);
    }

    [Test]
    public async Task IsAllowed_WhenStorageDoesNotContainEntry_ThenReturnsTrue_AndEntryPersisted()
    {
        mockStorage
            .Setup(s => s.GetAsync(Token))
            .ReturnsAsync((RateLimitEntry)null);

        var result = await underTest.IsAllowed(Token, options);

        Assert.IsTrue(result);
        mockStorage.Verify(
            s => s.SetAsync(Token,
                It.Is<RateLimitEntry>(e => (DateTime.UtcNow.TimeOfDay - e.LastCall) < TimeSpan.FromSeconds(1))),
            Times.Once);
    }

    [Test]
    public async Task IsAllowed_WhenSinceLastCallHasNotExceeded_ThenReturnsFalse()
    {
        var lastCall = DateTime.UtcNow.AddSeconds(-10);
        mockStorage
            .Setup(s => s.GetAsync(Token))
            .ReturnsAsync(new RateLimitEntry { LastCall = lastCall.TimeOfDay });

        var result = await underTest.IsAllowed(Token, options);

        Assert.IsFalse(result);
    }


    [Test]
    public async Task IsAllowed_WhenWhenSinceLastCallHasNotExceeded_ReturnsTrue_AndUpdatesLastCall()
    {
        var lastCall = DateTime.UtcNow.AddSeconds(-30);
        mockStorage
            .Setup(s => s.GetAsync(Token))
            .ReturnsAsync(new RateLimitEntry { LastCall = lastCall.TimeOfDay });

        var result = await underTest.IsAllowed(Token, options);

        Assert.IsTrue(result);
        mockStorage.Verify(
            s => s.SetAsync(Token,
                It.Is<RateLimitEntry>(e => (DateTime.UtcNow.TimeOfDay - e.LastCall) < TimeSpan.FromSeconds(1))),
            Times.Once);
    }
}