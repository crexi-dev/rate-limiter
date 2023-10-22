using System.Threading.Tasks;

namespace RateLimiter.Storage;

public interface IStorage<T>
{
    public Task<T?> GetAsync(string id);
    public Task SetAsync(string id, T entry);
}