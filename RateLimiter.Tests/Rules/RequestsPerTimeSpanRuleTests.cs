using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RateLimiter.Rules;
using RateLimiter.Storage;

namespace RateLimiter.Tests.Rules;

[TestFixture]
public class RequestsPerTimeSpanRuleTests
{
    private const string Token = "testToken";
    private RateLimitOptions options = new() { CallsCountLimit = 5 };
    private Mock<IStorage<RateLimitEntry>> mockStorage;
    private RequestsPerTimeSpanRule underTest;

    [SetUp]
    public void Setup()
    {
        mockStorage = new Mock<IStorage<RateLimitEntry>>();
        underTest = new RequestsPerTimeSpanRule(mockStorage.Object);
    }

    [Test]
    public async Task IsAllowed_WhenStorageDoesNotContainEntry_ThenReturnsTrue_AndEntryPersisted()
    {
        mockStorage
            .Setup(s => s.GetAsync(Token))
            .ReturnsAsync((RateLimitEntry)null);

        var result = await underTest.IsAllowed(Token, options);

        Assert.IsTrue(result);
        mockStorage.Verify(s => s.SetAsync(Token, It.Is<RateLimitEntry>(e => e.CallsCount == 1)), Times.Once);
    }

    [Test]
    public async Task IsAllowed_WhenCallsCountExceedsLimit_ThenReturnsFalse()
    {
        mockStorage
            .Setup(s => s.GetAsync(Token))
            .ReturnsAsync(new RateLimitEntry { CallsCount = 6 });

        var result = await underTest.IsAllowed(Token, options);

        Assert.IsFalse(result);
    }

    [Test]
    public async Task IsAllowed_WhenCallsCountDoesNotExceedLimit_ThenReturnsTrue_AndUpdatesEntry()
    {
        mockStorage
            .Setup(s => s.GetAsync(Token))
            .ReturnsAsync(new RateLimitEntry { CallsCount = 4 });

        var result = await underTest.IsAllowed(Token, options);

        Assert.IsTrue(result);
        mockStorage.Verify(s => s.SetAsync(Token, It.Is<RateLimitEntry>(e => e.CallsCount == 5)), Times.Once);
    }
}