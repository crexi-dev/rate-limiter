using Microsoft.Extensions.DependencyInjection;

namespace RateLimiter.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRateLimiter<TReqService>(this IServiceCollection services) where TReqService : class, IRateLimiterRequestService
        {
            return services.AddScoped<IRateLimiterRequestService, TReqService>();
        }
    }
}
