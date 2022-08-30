using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Services;
using RateLimiter.Services.Interfaces;
using MediatR;
using RateLimiter.InMemoryCache;
using RateLimiter.InMemoryCache.Interfaces;
using RateLimiter.Services.Handlers.Validators;
using RateLimiter.UtilityServices;

namespace RateLimiter.Extensions
{
    /// <summary>
    /// Call from Startup file to register RateLimiter services
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddRateLimiterServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<IRateLimiterService, RateLimiterService>();
            services.AddSingleton<IInMemoryCacheProxy, InMemoryCacheProxy>();
            services.AddTransient<IBaseHandlerModelValidator, BaseHandlerModelValidator>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        }
    }
}
