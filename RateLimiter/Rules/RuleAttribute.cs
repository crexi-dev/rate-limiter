using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Interfaces;

namespace RateLimiter.Rules;

public abstract class RuleAttribute : Attribute
{
    public abstract Task<bool> IsAllowedAsync(IServiceProvider sp, CancellationToken ct);
}

public abstract class RuleAttribute<T>: RuleAttribute
    where T : IRuleContextProvider
{
    public sealed override async Task<bool> IsAllowedAsync(IServiceProvider sp, CancellationToken ct)
    {
        var contextProvider = sp.GetRequiredService<T>();
        if (!await contextProvider.IsAppliableAsync(ct))
        {
            return true;
        }

        var clientId = await contextProvider.GetClientIdAsync(ct);
        return await IsAllowedAsync(clientId, sp, ct);
    }

    protected abstract Task<bool> IsAllowedAsync(string? clientId, IServiceProvider sp, CancellationToken ct);
}