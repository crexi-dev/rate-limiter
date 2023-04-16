using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Rules;

public class ExpressionRule<T> : IRule<T>
{
    private readonly Expression<Func<T, CancellationToken, Task<bool>>> _expression;

    public ExpressionRule(Expression<Func<T, CancellationToken, Task<bool>>> expression)
    {
        _expression = expression;
    }

    public async Task<bool> CheckAsync(T input, CancellationToken cancellationToken)
    {
        return await _expression.Compile().Invoke(input, cancellationToken).ConfigureAwait(false);
    }

    public string Description => "Expression Rule";
}