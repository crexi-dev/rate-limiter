using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Rules;

public interface IRule<in T>
{
    Task<bool> CheckAsync(T obj, CancellationToken cancellationToken);

    string Description { get; }
}