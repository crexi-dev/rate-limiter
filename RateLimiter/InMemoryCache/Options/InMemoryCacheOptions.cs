using System;
using System.Diagnostics.CodeAnalysis;

namespace RateLimiter.InMemoryCache.Options
{
    [ExcludeFromCodeCoverage]
    public class InMemoryCacheOptions
    {
        public TimeSpan ExpirationTimeSpan { get; set; }
    }
}
