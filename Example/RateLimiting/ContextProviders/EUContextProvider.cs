using RateLimiter.Interfaces;

namespace Example.RateLimiting.ContextProviders;

public class EUContextProvider(IHttpContextAccessor httpContextAccessor) : IRuleContextProvider
{
    public Task<bool> IsAppliableAsync(CancellationToken ct)
    {
        var result = httpContextAccessor.HttpContext.IsSite("EU");
        return Task.FromResult(result);
    }

    public Task<string> GetClientIdAsync(CancellationToken ct)
    {
        var result = httpContextAccessor.HttpContext.GetClientId();
        return Task.FromResult(result ?? "");
    }
}