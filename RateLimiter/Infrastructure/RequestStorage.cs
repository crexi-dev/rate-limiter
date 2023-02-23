using System;
using System.Runtime.Caching;
using RateLimiter.Models;

namespace RateLimiter.Infrastructure
{
	public class RequestStorage
	{
		private readonly MemoryCache _memoryCache = new MemoryCache("RequestStorage");

		public RequestsData GetRequestByKey(string key)
		{
			RequestsData requests;
			if (_memoryCache.Contains(key))
			{
				requests = _memoryCache.Get(key) as RequestsData ?? new RequestsData();
				requests.Requests.Add(DateTime.UtcNow);
			}
			else
			{
				requests = new RequestsData();
				requests.Requests.Add(DateTime.UtcNow);
				_memoryCache.Add(key, requests, new CacheItemPolicy());
			}

			return requests;
		}
	}
}