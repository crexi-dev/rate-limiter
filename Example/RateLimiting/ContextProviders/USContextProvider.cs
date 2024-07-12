using RateLimiter.Interfaces;

namespace Example.RateLimiting.ContextProviders;

public class USContextProvider(IHttpContextAccessor httpContextAccessor) : IRuleContextProvider
{
    public Task<bool> IsAppliableAsync(CancellationToken ct)
    {
        var result = httpContextAccessor.HttpContext.IsSite("US");
        return Task.FromResult(result);
    }

    public Task<string> GetClientIdAsync(CancellationToken ct)
    {
        var result = httpContextAccessor.HttpContext.GetClientId();
        return Task.FromResult(result ?? "");
    }
}