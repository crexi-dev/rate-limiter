using System;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter;

public interface ICheckConstraints
{
    Task<bool> AccessGranted(Guid tokenId, CancellationToken token);
}