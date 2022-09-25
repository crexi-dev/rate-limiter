using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public interface ICacheService<T>
	{
		Task<bool> ExistsAsync(string cacheKey);
		Task<T> GetAsync(string cacheKey);
		Task AddAsync(string cacheKey, T entity);
		Task RemoveAsync(string cacheKey);
	}
}
