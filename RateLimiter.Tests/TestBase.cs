using NUnit.Framework;
using Microsoft.Extensions.Caching.Memory;

namespace RateLimiter.Tests;

public class TestBase
{
    protected IMemoryCache _cache;

    [SetUp]
    public void Setup()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
    }
}
