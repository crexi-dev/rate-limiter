using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RateLimiter.Implementations;

namespace RateLimiter.Tests;

[TestFixture]
public class InMemoryStoreTests
{
    private InMemoryStore _store;

    [SetUp]
    public void Setup()
    {
        _store = new InMemoryStore();
    }

    [Test]
    public async Task GetAsync_ReturnsValueCorrectly_AfterSetAsync()
    {
        const string key = "testKey";
        const string value = "testValue";
        await _store.SetAsync("testKey", "testValue", CancellationToken.None);
        var result = await _store.GetAsync<string>(key, CancellationToken.None);

        result.Should().Be(value);
    }

    [Test]
    public async Task GetAsync_ReturnsNullForNonExistentKey()
    {
        var result = await _store.GetAsync<string>("nonExistentKey", CancellationToken.None);

        result.Should().BeNull();
    }

    [Test]
    public async Task LockAsync_LocksKeyCorrectly()
    {
        const string key = "testKey";
        var lock1 = await _store.LockAsync(key, CancellationToken.None);

        Task lockTask = _store.LockAsync(key, CancellationToken.None);
        lockTask.IsCompleted.Should().BeFalse();

        await lock1.DisposeAsync();

        lockTask.IsCompleted.Should().BeTrue();
    }
}