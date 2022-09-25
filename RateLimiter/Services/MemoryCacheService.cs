using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace RateLimiter.Services
{
    public class MemoryCacheService<T> : ICacheService<T>
	{
		private readonly IMemoryCache _memoryCache;
		public MemoryCacheService(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}
		public async Task<bool> ExistsAsync(string cacheKey)
		{
			return await Task.FromResult(_memoryCache.TryGetValue(cacheKey, out _));
		}

		public async Task<T> GetAsync(string cacheKey)
		{
			if (_memoryCache.TryGetValue(cacheKey, out T countEntity))
			{
				return await Task.FromResult(countEntity);
			}

			return await Task.FromResult(default(T));
		}

		public async Task AddAsync(string cacheKey, T entity)
		{
			var cacheOptions = new MemoryCacheEntryOptions
			{
				Priority = CacheItemPriority.NeverRemove
			};
			_memoryCache.Set(cacheKey, entity, cacheOptions);

			await Task.CompletedTask;
		}

		public async Task RemoveAsync(string cacheKey)
		{
			_memoryCache.Remove(cacheKey);
			await Task.CompletedTask;
		}
	}
}
