using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces;

public interface IRuleContextProvider
{
    Task<bool> IsAppliableAsync(CancellationToken ct);

    Task<string> GetClientIdAsync(CancellationToken ct);
}