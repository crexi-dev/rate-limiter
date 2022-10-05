using Microsoft.Extensions.DependencyInjection;
using RateLimiter.DAL;

namespace RateLimiter;

public static class RegistrationDependencyInjection
{
    public static void RegisterBlLayer(this IServiceCollection collection)
    {
        collection.RegisterDaLayer();
        collection.AddScoped<ICheckConstraints, CheckConstraints>();
    }
}