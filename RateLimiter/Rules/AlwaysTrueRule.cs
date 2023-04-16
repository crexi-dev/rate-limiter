using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Rules;

public class AlwaysTrueRule<T> : IRule<T>
{
    public Task<bool> CheckAsync(T obj, CancellationToken cancellationToken) => Task.FromResult(true);
    public string Description => "Always True Rule";
}