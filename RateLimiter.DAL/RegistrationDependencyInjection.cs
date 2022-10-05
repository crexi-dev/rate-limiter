using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.DAL.Repositories;

namespace RateLimiter.DAL;

public static class RegistrationDependencyInjection
{
    public static void RegisterDaLayer(this IServiceCollection collection)
    {
        collection.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase("RateLimiter"));
        collection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        collection.AddScoped<IHistoryRepository, HistoryRepository>();
    }
}