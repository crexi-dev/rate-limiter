using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Extensions;

namespace RateLimiter.InMemory.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryRateLimiter(this IServiceCollection services)
        {
            return services.AddRateLimiter<InMemoryRateLimiterRequestService>();
        }
    }
}
