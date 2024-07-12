using System.Threading;
using System.Threading.Tasks;
using RateLimiter.Interfaces;

namespace RateLimiter.Implementations;

public class EmptyContextProvider : IRuleContextProvider
{
    public Task<bool> IsAppliableAsync(CancellationToken ct) => Task.FromResult(true);

    public Task<string> GetClientIdAsync(CancellationToken ct) => Task.FromResult("");
}