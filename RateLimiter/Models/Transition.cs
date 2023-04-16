using System.Threading;
using System.Threading.Tasks;
using RateLimiter.Rules;

namespace RateLimiter.Models;

internal class Transition<T>
{
    private readonly IRule<T> _rule;
    public State NextState { get; }

    public Transition(IRule<T> rule, State state)
    {
        _rule = rule;
        NextState = state;
    }

    public async Task<bool> TryExecuteAsync(T obj, CancellationToken cancellationToken) =>
        await _rule.CheckAsync(obj, cancellationToken).ConfigureAwait(false);
}