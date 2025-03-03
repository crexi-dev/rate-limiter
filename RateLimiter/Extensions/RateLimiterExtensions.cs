using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using RateLimiter.Rules;
using RateLimiter.Rules.FixedWindowRule;
using RateLimiter.Rules.TimeBasedRule;

namespace RateLimiter.Extensions;
public static class RateLimiterExtensions
{
    private static IServiceCollection AddRateLimitRule<TOptions, TRule>(
        this IServiceCollection services,
        Action<TOptions> configure,
        Func<IMemoryCache, TOptions, TRule> ruleFactory) where TRule : class, IRateLimitRule/*, new()*/ where TOptions : IRateLimitRuleOptions, new()
    {
        services.AddSingleton<IRateLimitRule>(sp =>
        {
            var cache = sp.GetRequiredService<IMemoryCache>();
            var options = new TOptions();
            configure(options);
            return ruleFactory(cache, options);
        });
        return services;
    }

    public static IServiceCollection AddFixedWindow(this IServiceCollection services, Action<FixedWindowRuleOptions> configure)
    {
        return services.AddRateLimitRule(configure, (cache, options) => new FixedWindowRule(options, cache));
    }

    public static IServiceCollection AddTimeBasedRateLimiting(this IServiceCollection services, Action<TimeBasedRuleOptions> configure)
    {
        return services.AddRateLimitRule(configure, (cache, options) => new TimeBasedRule(options, cache));
    }

    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitMiddleware>();
    }
}



