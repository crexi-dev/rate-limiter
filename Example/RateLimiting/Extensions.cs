using Example.RateLimiting.ContextProviders;

namespace Example.RateLimiting;

public static class Extensions
{
    public static string? GetClaimValue(this HttpContext httpContext, string claimType)
    {
        return httpContext.User.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    public static bool IsSite(this HttpContext? httpContext, string site)
    {
        return httpContext?.GetClaimValue("site") == site;
    }

    public static string? GetClientId(this HttpContext? httpContext)
    {
        return httpContext?.GetClaimValue("client_id");
    }
    
    public static IServiceCollection AddCustomRateLimitingServices(this IServiceCollection services)
    {
        services.AddTransient<EUContextProvider>();
        services.AddTransient<USContextProvider>();

        return services;
    }
}