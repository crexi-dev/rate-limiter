using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Machine;

public interface IRuleStateMachine<in T>
{
    Task<bool> RunAsync(T input, CancellationToken cancellationToken);
}