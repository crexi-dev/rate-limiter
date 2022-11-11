using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace RateLimiter.AspNetCore;

public static class RateLimitingServiceCollection
{
    public static RateLimitingBuilder AddRateLimiting(this IServiceCollection services,  Action<RateLimitingOptions>? setupAction = null)
    {
        services.AddOptions();
        services.AddSingleton<IRateLimiterStorage, RateLimiterStorage>();

        if (setupAction is not null)
        {
            services.Configure(setupAction);    
        }

        return new RateLimitingBuilder(services);
    }
}

public class RateLimitingBuilder
{
    private readonly IServiceCollection _services;
    
    public RateLimitingBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public RateLimitingBuilder AddParameterProvider(string parameterName, Func<HttpContext, string> resolver)
    {
        _services.Configure<RateLimitingOptions>(options =>
        {
            options.ParameterProviders[parameterName] = new DelegateParameterProvider(resolver);
        });

        return this;
    }

    public RateLimitingBuilder AddUserProvider(Func<HttpContext, string> resolver)
    {
        return AddParameterProvider(RateLimitingParameters.User, resolver);
    }

    public RateLimitingBuilder AddRegionProvider(Func<HttpContext, string> resolver)
    {
        return AddParameterProvider(RateLimitingParameters.Region, resolver);
    }
}