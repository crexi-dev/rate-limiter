using System;

namespace RateLimiter.Contracts
{
    public interface IClient
    {
        Guid Token { get; }
        string Name { get; }
        int RegionId { get; }
    }
}
